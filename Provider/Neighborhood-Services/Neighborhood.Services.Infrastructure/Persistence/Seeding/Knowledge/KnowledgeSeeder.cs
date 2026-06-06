using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.AI.Interfaces;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using System.Text.Json;

namespace Neighborhood.Services.Infrastructure.Persistence.Seeding.Knowledge
{
    /// <summary>
    /// Seeds Qdrant from the database (categories + problem types, which carry real IDs)
    /// and from Faqs.json. Runs on startup AFTER DbSeeder. Safe to re-run — deterministic
    /// IDs mean existing vectors are just overwritten.
    ///
    /// Collections:
    ///   - "platform-knowledge" : categories, problem types, FAQs, platform/booking/payment docs.
    ///                            Text-only. Used by the chatbot for general Q&A.
    ///   - "problem-types"      : one vector per problem type WITH its real problemTypeId in the
    ///                            payload. Used to classify free text -> ProblemTypeId.
    /// </summary>
    public class KnowledgeSeeder
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

        public async Task SeedAsync()
        {
            await SeedCatalogFromDbAsync();
            await SeedStaticDocsAsync();
            await SeedFaqsAsync();
        }

        // Reads categories + problem types FROM THE DB (real IDs) and embeds them.
        private async Task SeedCatalogFromDbAsync()
        {
            var categories = await _context.Categories.AsNoTracking().ToListAsync();
            var problemTypes = await _context.ProblemTypes
                .Include(p => p.Category)
                .AsNoTracking()
                .ToListAsync();

            // 1. One doc per category -> platform-knowledge
            foreach (var cat in categories)
            {
                var text = $"{cat.NameEn} ({cat.NameAr}) is a service category on Neighborhood Services. " +
                           $"It covers home services provided by professional technicians.";
                await _memory.UpsertAsync(KnowledgeCollection, $"category-{cat.Id}", text);
            }

            // 2. One doc per problem type -> BOTH collections
            foreach (var pt in problemTypes)
            {
                var categoryName = pt.Category is not null
                    ? $"{pt.Category.NameEn} ({pt.Category.NameAr})"
                    : "General";

                var text = $"{pt.NameEn} ({pt.NameAr}) — Category: {categoryName}. " +
                           $"{pt.DescriptionEn} Arabic: {pt.DescriptionAr}. " +
                           $"Price range: {pt.MinPrice} to {pt.MaxPrice} EGP.";

                // a) platform-knowledge: text only, for chatbot "what do you offer / how much"
                await _memory.UpsertAsync(KnowledgeCollection, $"problemtype-{pt.Id}", text);

                // b) problem-types: same text + the real id in the payload, for classification
                await _memory.UpsertAsync(
                    ProblemTypesCollection,
                    $"pt-{pt.Id}",
                    text,
                    new Dictionary<string, string> { ["problemTypeId"] = pt.Id.ToString() });
            }
        }

        // Hardcoded platform/booking/payment docs -> platform-knowledge.
        private async Task SeedStaticDocsAsync()
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
            await _memory.UpsertAsync(KnowledgeCollection, "platform-overview", platformDoc);

            var bookingDoc = "How to book on Neighborhood Services: " +
                             "Option 1 (Direct Booking) — Browse technicians, choose one, pick a time, and book directly. " +
                             "The technician reviews and accepts your booking. Once accepted, payment is held in escrow. " +
                             "Option 2 (Service Request) — Post a service request describing your problem and budget. " +
                             "Technicians submit offers with their price and proposed time. " +
                             "You review the offers and accept the best one. A booking is created automatically. " +
                             "After the job is done, you confirm completion and payment is released to the technician.";
            await _memory.UpsertAsync(KnowledgeCollection, "booking-flow", bookingDoc);

            var paymentDoc = "Payment on Neighborhood Services works through a secure escrow system. " +
                             "When a booking is confirmed, the customer's payment is held in escrow. " +
                             "After the technician completes the job and the customer confirms, the payment is released to the technician's wallet. " +
                             "If a booking is cancelled, the full amount is refunded to the customer. " +
                             "Customers can top up their wallet using Paymob payment gateway. " +
                             "All prices are in Egyptian Pounds (EGP).";
            await _memory.UpsertAsync(KnowledgeCollection, "payment-info", paymentDoc);
        }

        // Reads Faqs.json (copied next to the running DLLs) and embeds one doc per FAQ.
        private async Task SeedFaqsAsync()
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
                await _memory.UpsertAsync(KnowledgeCollection, $"faq-{index + 1}", text);
            }
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
