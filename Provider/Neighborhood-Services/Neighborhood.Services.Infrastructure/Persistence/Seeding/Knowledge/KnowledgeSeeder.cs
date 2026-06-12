using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.AI.Interfaces;
using Neighborhood.Services.Domain.Categories;
using Neighborhood.Services.Domain.ProblemTypes;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Neighborhood.Services.Infrastructure.Persistence.Seeding.Knowledge
{
    /// <summary>
    /// Builds and maintains the Qdrant knowledge index from the database (categories + problem
    /// types, which carry real IDs) plus static docs and Faqs.json.
    ///
    /// NOT run on startup — triggered deliberately via <see cref="ReindexAllAsync"/> (CLI / admin
    /// button / CI-CD). The single-item hooks keep it live as catalog rows change. Cheap to re-run:
    /// each doc carries a content hash, so only new/changed docs are re-embedded.
    ///
    /// Collections:
    ///   - "platform-knowledge" : categories, problem types, FAQs, platform/booking/payment docs.
    ///                            Text-only. Used by the chatbot for general Q&A.
    ///   - "problem-types"      : one vector per problem type WITH its real problemTypeId in the
    ///                            payload. Used to classify free text -> ProblemTypeId.
    /// </summary>
    public class KnowledgeSeeder : IKnowledgeIndexer
    {
        private readonly IVectorMemory _memory;
        private readonly ApplicationDbContext _context;

        private const string KnowledgeCollection = "platform-knowledge";
        private const string ProblemTypesCollection = "problem-types";

        public KnowledgeSeeder(IVectorMemory memory, ApplicationDbContext context)
        {
            _memory = memory;
            _context = context;
        }

        // Bulk rebuild of the entire index from the DB. Deliberate trigger only.
        public async Task ReindexAllAsync()
        {
            var knowledge = new SeedBatch(KnowledgeCollection);
            var problemTypes = new SeedBatch(ProblemTypesCollection);

            // Phase 1 — build the desired docs and queue only the changed ones (no embedding yet).
            await SeedCatalogFromDbAsync(knowledge, problemTypes);
            await SeedStaticDocsAsync(knowledge);
            await SeedFaqsAsync(knowledge);

            // Phase 2 — batch-embed + upsert the changed docs, then prune orphans.
            await FlushAsync(knowledge);
            await FlushAsync(problemTypes);
        }

        // Batch-embeds the queued (changed) docs in one shot, then deletes any vector whose id
        // we no longer produce (e.g. a category/problem type removed from the DB).
        private async Task FlushAsync(SeedBatch batch)
        {
            if (batch.Changed.Count > 0)
                await _memory.UpsertManyAsync(batch.Collection, batch.Changed);

            await _memory.RemoveExceptAsync(batch.Collection, batch.KeepIds);
        }

        // Reads categories + problem types FROM THE DB (real IDs) and embeds them.
        private async Task SeedCatalogFromDbAsync(SeedBatch knowledge, SeedBatch problemTypes)
        {
            var categories = await _context.Categories.AsNoTracking().ToListAsync();
            var problemTypeList = await _context.ProblemTypes
                .Include(p => p.Category)
                .AsNoTracking()
                .ToListAsync();

            // 1. One doc per category -> platform-knowledge
            foreach (var cat in categories)
                await AddAsync(knowledge, CategoryDocId(cat.Id), BuildCategoryText(cat));

            // 2. One doc per problem type -> BOTH collections
            foreach (var pt in problemTypeList)
            {
                var text = BuildProblemTypeText(pt);

                // a) platform-knowledge: text only, for chatbot "what do you offer / how much"
                await AddAsync(knowledge, ProblemTypeKnowledgeDocId(pt.Id), text);

                // b) problem-types: same text + the real id in the payload, for classification
                await AddAsync(problemTypes, ProblemTypeClassifyDocId(pt.Id), text, ProblemTypeFields(pt.Id));
            }
        }

        // --- Doc id + text builders (shared by the bulk reindex and the single-item hooks,
        //     so both produce byte-identical docs and the hash-skip stays consistent) ---

        private static string CategoryDocId(int id) => $"category-{id}";
        private static string ProblemTypeKnowledgeDocId(int id) => $"problemtype-{id}";
        private static string ProblemTypeClassifyDocId(int id) => $"pt-{id}";
        private static Dictionary<string, string> ProblemTypeFields(int id) =>
            new() { ["problemTypeId"] = id.ToString() };

        private static string BuildCategoryText(Category cat) =>
            $"{cat.NameEn} ({cat.NameAr}) is a service category on Neighborhood Services. " +
            $"It covers home services provided by professional technicians.";

        private static string BuildProblemTypeText(ProblemType pt)
        {
            var categoryName = pt.Category is not null
                ? $"{pt.Category.NameEn} ({pt.Category.NameAr})"
                : "General";

            return $"{pt.NameEn} ({pt.NameAr}) — Category: {categoryName}. " +
                   $"{pt.DescriptionEn} Arabic: {pt.DescriptionAr}. " +
                   $"Price range: {pt.MinPrice} to {pt.MaxPrice} EGP.";
        }

        // Hardcoded platform/booking/payment docs -> platform-knowledge.
        private async Task SeedStaticDocsAsync(SeedBatch knowledge)
        {
            var categoryNames = await _context.Categories
                .AsNoTracking()
                .Select(c => c.NameEn + " (" + c.NameAr + ")")
                .ToListAsync();
            var categoryList = string.Join(", ", categoryNames);

            var platformDoc = $"Neighborhood Services is a home services marketplace platform in Egypt. " +
                              $"We connect customers with professional technicians for home repairs and maintenance. " +
                              $"Available service categories: {categoryList}. " +
                              $"Customers can post a service request or book a technician directly. " +
                              $"Payment is handled securely via escrow — money is held until the job is confirmed complete. " +
                              $"The platform supports both Arabic and English.";
            await AddAsync(knowledge, "platform-overview", platformDoc);

            var bookingDoc = "How to book on Neighborhood Services: " +
                             "Option 1 (Direct Booking) — Browse technicians, choose one, pick a time, and book directly. " +
                             "The technician reviews and accepts your booking. Once accepted, payment is held in escrow. " +
                             "Option 2 (Service Request) — Post a service request describing your problem and budget. " +
                             "Technicians submit offers with their price and proposed time. " +
                             "You review the offers and accept the best one. A booking is created automatically. " +
                             "After the job is done, you confirm completion and payment is released to the technician.";
            await AddAsync(knowledge, "booking-flow", bookingDoc);

            var paymentDoc = "Payment on Neighborhood Services works through a secure escrow system. " +
                             "When a booking is confirmed, the customer's payment is held in escrow. " +
                             "After the technician completes the job and the customer confirms, the payment is released to the technician's wallet. " +
                             "If a booking is cancelled, the full amount is refunded to the customer. " +
                             "Customers can top up their wallet using Paymob payment gateway. " +
                             "All prices are in Egyptian Pounds (EGP).";
            await AddAsync(knowledge, "payment-info", paymentDoc);
        }

        // Reads Faqs.json (copied next to the running DLLs) and embeds one doc per FAQ.
        private async Task SeedFaqsAsync(SeedBatch knowledge)
        {
            // CopyToOutputDirectory drops the file next to the DLLs = AppContext.BaseDirectory,
            // NOT the API project's ContentRootPath.
            var faqsPath = Path.Combine(AppContext.BaseDirectory, "SeedData", "Knowledge", "Faqs.json");
            if (!File.Exists(faqsPath))
                return;

            var faqsJson = await File.ReadAllTextAsync(faqsPath);
            var faqs = JsonSerializer.Deserialize<List<FaqDoc>>(faqsJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

            foreach (var (index, faq) in faqs.Select((f, i) => (i, f)))
            {
                var text = $"Q: {faq.QuestionEn} / {faq.QuestionAr}\n" +
                           $"A: {faq.AnswerEn}\n" +
                           $"الإجابة: {faq.AnswerAr}";
                await AddAsync(knowledge, $"faq-{index + 1}", text);
            }
        }

        // Payload field where we store the content hash of each embedded doc.
        private const string HashField = "_hash";

        /// <summary>
        /// Registers a doc with the batch: always marks the id as "still wanted" (so the cleanup
        /// pass keeps it), and queues it for (batched) embedding ONLY if its text changed since
        /// last seed — compared via a SHA-256 hash stored in the doc's payload. No embedding call
        /// happens here; the queued docs are embedded together in FlushAsync.
        /// </summary>
        private async Task AddAsync(
            SeedBatch batch, string id, string text, IReadOnlyDictionary<string, string>? fields = null)
        {
            batch.KeepIds.Add(id);

            var hash = ComputeHash(text);

            var existingHash = await _memory.GetFieldAsync(batch.Collection, id, HashField);
            if (existingHash == hash)
                return; // unchanged — don't queue for embedding

            var payload = fields is null
                ? new Dictionary<string, string>()
                : new Dictionary<string, string>(fields);
            payload[HashField] = hash;

            batch.Changed.Add(new VectorDoc(id, text, payload));
        }

        // --- Event-driven single-item hooks (wire into admin create/update/delete later) ---

        public async Task IndexCategoryAsync(int categoryId)
        {
            var cat = await _context.Categories.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == categoryId);

            if (cat is null) { await RemoveCategoryAsync(categoryId); return; }

            await UpsertDocAsync(KnowledgeCollection, CategoryDocId(cat.Id), BuildCategoryText(cat));
        }

        public async Task IndexProblemTypeAsync(int problemTypeId)
        {
            var pt = await _context.ProblemTypes
                .Include(p => p.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == problemTypeId);

            if (pt is null) { await RemoveProblemTypeAsync(problemTypeId); return; }

            var text = BuildProblemTypeText(pt);
            await UpsertDocAsync(KnowledgeCollection, ProblemTypeKnowledgeDocId(pt.Id), text);
            await UpsertDocAsync(ProblemTypesCollection, ProblemTypeClassifyDocId(pt.Id), text, ProblemTypeFields(pt.Id));
        }

        public Task RemoveCategoryAsync(int categoryId) =>
            _memory.RemoveAsync(KnowledgeCollection, CategoryDocId(categoryId));

        public async Task RemoveProblemTypeAsync(int problemTypeId)
        {
            await _memory.RemoveAsync(KnowledgeCollection, ProblemTypeKnowledgeDocId(problemTypeId));
            await _memory.RemoveAsync(ProblemTypesCollection, ProblemTypeClassifyDocId(problemTypeId));
        }

        // Embeds + stores ONE doc immediately, stamping the same content hash the bulk reindex
        // uses — so a later reindex sees it as unchanged and skips it.
        private async Task UpsertDocAsync(
            string collection, string id, string text, IReadOnlyDictionary<string, string>? fields = null)
        {
            var payload = fields is null
                ? new Dictionary<string, string>()
                : new Dictionary<string, string>(fields);
            payload[HashField] = ComputeHash(text);

            await _memory.UpsertAsync(collection, id, text, payload);
        }

        private static string ComputeHash(string text)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(text));
            return Convert.ToHexString(bytes);
        }

        // Accumulates, for one collection, all ids seen this run (for orphan cleanup) and the
        // subset whose text changed (to batch-embed in FlushAsync).
        private sealed class SeedBatch(string collection)
        {
            public string Collection { get; } = collection;
            public HashSet<string> KeepIds { get; } = new();
            public List<VectorDoc> Changed { get; } = new();
        }

        private class FaqDoc
        {
            public string QuestionEn { get; set; } = string.Empty;
            public string QuestionAr { get; set; } = string.Empty;
            public string AnswerEn { get; set; } = string.Empty;
            public string AnswerAr { get; set; } = string.Empty;
        }
    }
}
