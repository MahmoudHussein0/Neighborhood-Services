using Neighborhood.Services.Application.AI.Interfaces;
using System.Text.Json;

namespace Neighborhood.Services.Infrastructure.Persistence.Seeding.Knowledge
{
    /// <summary>
    /// Seeds the Qdrant 'platform-knowledge' collection from the JSON files in SeedData/Knowledge/.
    /// Runs on startup. Safe to re-run — uses deterministic IDs so existing docs are just overwritten.
    /// </summary>
    public class KnowledgeSeeder
    {
        private readonly IVectorMemory _memory;
        private const string Collection = "platform-knowledge";

        public KnowledgeSeeder(IVectorMemory memory)
        {
            _memory = memory;
        }

        public async Task SeedAsync(string basePath)
        {
            var knowledgeDir = Path.Combine(basePath, "SeedData", "Knowledge");

            // FAQs are independent — seed them even if the category files aren't there yet.
            await SeedFaqsAsync(Path.Combine(knowledgeDir, "Faqs.json"));

            var categoriesPath = Path.Combine(knowledgeDir, "Categories.json");
            var problemTypesPath = Path.Combine(knowledgeDir, "ProblemTypes.json");

            if (!File.Exists(categoriesPath) || !File.Exists(problemTypesPath))
                return; // category files not there yet — skip the rest silently

            var categoriesJson = await File.ReadAllTextAsync(categoriesPath);
            var problemTypesJson = await File.ReadAllTextAsync(problemTypesPath);

            var categories = JsonSerializer.Deserialize<List<CategoryDoc>>(categoriesJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

            var problemTypes = JsonSerializer.Deserialize<List<ProblemTypeDoc>>(problemTypesJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

            // Build category lookup by index (CategoryId in JSON is 1-based index)
            var categoryById = categories
                .Select((c, i) => new { Id = i + 1, Category = c })
                .ToDictionary(x => x.Id, x => x.Category);

            // 1. Upsert one doc per category
            foreach (var (id, cat) in categoryById)
            {
                var text = $"{cat.NameEn} ({cat.NameAr}) is a service category on Neighborhood Services platform. " +
                           $"It covers home services provided by professional technicians.";

                await _memory.UpsertAsync(Collection, $"category-{id}", text);
            }

            // 2. Upsert one doc per problem type
            foreach (var (index, pt) in problemTypes.Select((p, i) => (i, p)))
            {
                var categoryName = categoryById.TryGetValue(pt.CategoryId, out var cat)
                    ? $"{cat.NameEn} ({cat.NameAr})"
                    : "General";

                var text = $"{pt.NameEn} ({pt.NameAr}) — Category: {categoryName}. " +
                           $"{pt.DescriptionEn} " +
                           $"Arabic description: {pt.DescriptionAr}. " +
                           $"Price range: {pt.MinPrice} to {pt.MaxPrice} EGP.";

                await _memory.UpsertAsync(Collection, $"problemtype-{index + 1}", text);
            }

            // 3. Upsert a general platform overview doc
            var categoryList = string.Join(", ", categories.Select(c => $"{c.NameEn} ({c.NameAr})"));
            var platformDoc = $"Neighborhood Services is a home services marketplace platform in Egypt. " +
                              $"We connect customers with professional technicians for home repairs and maintenance. " +
                              $"Available service categories: {categoryList}. " +
                              $"Customers can post a service request or book a technician directly. " +
                              $"Payment is handled securely via escrow — money is held until the job is confirmed complete. " +
                              $"The platform supports both Arabic and English.";

            await _memory.UpsertAsync(Collection, "platform-overview", platformDoc);

            // 4. Upsert a booking flow doc
            var bookingDoc = "How to book on Neighborhood Services: " +
                             "Option 1 (Direct Booking) — Browse technicians, choose one, pick a time, and book directly. " +
                             "The technician reviews and accepts your booking. Once accepted, payment is held in escrow. " +
                             "Option 2 (Service Request) — Post a service request describing your problem and budget. " +
                             "Technicians submit offers with their price and proposed time. " +
                             "You review the offers and accept the best one. A booking is created automatically. " +
                             "After the job is done, you confirm completion and payment is released to the technician.";

            await _memory.UpsertAsync(Collection, "booking-flow", bookingDoc);

            // 5. Upsert a payment doc
            var paymentDoc = "Payment on Neighborhood Services works through a secure escrow system. " +
                             "When a booking is confirmed, the customer's payment is held in escrow (not paid to the technician yet). " +
                             "After the technician completes the job and the customer confirms, the payment is released to the technician's wallet. " +
                             "If a booking is cancelled, the full amount is refunded to the customer. " +
                             "Customers can top up their wallet using Paymob payment gateway. " +
                             "All prices are in Egyptian Pounds (EGP).";

            await _memory.UpsertAsync(Collection, "payment-info", paymentDoc);
        }

        // Reads Faqs.json and upserts one doc per FAQ (bilingual text so it matches Arabic or English queries).
        private async Task SeedFaqsAsync(string faqsPath)
        {
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

                await _memory.UpsertAsync(Collection, $"faq-{index + 1}", text);
            }
        }

        // Local DTOs for JSON deserialization — only used inside this seeder
        private class FaqDoc
        {
            public string QuestionEn { get; set; } = string.Empty;
            public string QuestionAr { get; set; } = string.Empty;
            public string AnswerEn { get; set; } = string.Empty;
            public string AnswerAr { get; set; } = string.Empty;
        }

        private class CategoryDoc
        {
            public string NameEn { get; set; } = string.Empty;
            public string NameAr { get; set; } = string.Empty;
            public string Icon { get; set; } = string.Empty;
        }

        private class ProblemTypeDoc
        {
            public string NameEn { get; set; } = string.Empty;
            public string NameAr { get; set; } = string.Empty;
            public string DescriptionEn { get; set; } = string.Empty;
            public string DescriptionAr { get; set; } = string.Empty;
            public decimal MinPrice { get; set; }
            public decimal MaxPrice { get; set; }
            public int CategoryId { get; set; }
        }
    }
}
