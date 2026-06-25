using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Neighborhood.Services.Application.Bookings.Queries.GetTechnicianAvailableSlots;
using Neighborhood.Services.Application.Technicians.Queries; // GetTechniciansForBrowseQuery
using System.ComponentModel;
using System.Globalization;

namespace Neighborhood.Services.Application.Chatbot.Tools
{
    // Semantic Kernel tools that let the chatbot LLM look up technicians and their free time.
    // Because a name like "Ali" isn't unique, find_technicians returns a SHORT candidate list
    // (with id + distinguishing info) and the model asks the user to pick; once it has an id it
    // calls check_availability. Both wrap the same MediatR queries the controllers use.
    public class TechnicianTool
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        // Cap how many candidates we hand the model — never dump 50 rows into the context.
        private const int MaxCandidates = 5;

        public TechnicianTool(IMediator mediator, ILogger logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        // Words the model commonly appends that aren't part of a real name — stripped so a search
        // like "خالد فني" still matches the technician "خالد".
        private static readonly HashSet<string> NameNoise = new(StringComparer.OrdinalIgnoreCase)
        {
            "فني", "الفني", "مهندس", "م", "الاستاذ", "استاذ", "أ",
            "eng", "engineer", "mr", "mrs", "ms", "dr", "the"
        };

        // Splits a search name into meaningful tokens (drops honorifics and 1-char fragments),
        // falling back to the whole trimmed string if nothing useful remains.
        private static List<string> NameTokens(string name)
        {
            var tokens = (name ?? string.Empty)
                .Split(new[] { ' ', '/', '-', '.', ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .Where(t => t.Length >= 2 && !NameNoise.Contains(t))
                .ToList();

            if (tokens.Count == 0 && !string.IsNullOrWhiteSpace(name))
                tokens.Add(name.Trim());

            return tokens;
        }

        [KernelFunction("find_technicians")]
        [Description("Find technicians by (partial) name, optionally narrowed by service category. " +
            "Returns a short candidate list with their id, rating, and categories. Use this FIRST " +
            "when the user names a technician, then ask them to pick if more than one is returned. " +
            "You need a technician's id from here before you can check their availability.")]
        public async Task<string> FindTechnicians(
            [Description("The technician's name or part of it, e.g. 'Ali'.")] string name,
            [Description("Optional service category to narrow the search, e.g. 'Plumbing'. Leave empty if not given.")]
            string? category = null)
        {
            _logger.LogInformation(
                "TechnicianTool: find_technicians CALLED — name='{Name}' category='{Category}'",
                name, category ?? "(none)");

            var all = await _mediator.Send(new GetTechniciansForBrowseQuery());

            // Match on name TOKENS, not the whole string — the model often re-searches with extra
            // words like the honorific "فني" or "Eng." appended (e.g. "خالد فني"), which a literal
            // Contains would miss. We rank by how many tokens hit, so the best name match wins.
            var tokens = NameTokens(name);

            var matches = all
                .Select(t => new
                {
                    Tech = t,
                    Hits = tokens.Count(tok => t.FullName.Contains(tok, StringComparison.OrdinalIgnoreCase))
                })
                .Where(x => x.Hits > 0)
                .Where(x => string.IsNullOrWhiteSpace(category)
                            || x.Tech.Categories.Any(c =>
                                c.NameEn.Contains(category.Trim(), StringComparison.OrdinalIgnoreCase)
                                || c.NameAr.Contains(category.Trim(), StringComparison.OrdinalIgnoreCase)))
                .OrderByDescending(x => x.Hits)
                .ThenByDescending(x => x.Tech.Rating)
                .Take(MaxCandidates)
                .Select(x => x.Tech)
                .ToList();

            _logger.LogInformation("TechnicianTool: find_technicians → {Count} match(es)", matches.Count);

            if (matches.Count == 0)
                return $"NO_MATCH: No technician found named '{name}'" +
                       (string.IsNullOrWhiteSpace(category) ? "." : $" in category '{category}'.") +
                       " Ask the user to check the name or try a different one.";

            // Compact, model-readable lines. The model uses the id to call check_availability,
            // and the category/rating to disambiguate with the user when there's more than one.
            var lines = matches.Select(t =>
            {
                var cats = t.Categories.Count == 0
                    ? "no categories"
                    : string.Join(", ", t.Categories.Select(c => c.NameEn));
                return $"id={t.Id} | {t.FullName} | rating={t.Rating:0.0} | {cats} | "
                     + (t.IsAvailable ? "available" : "currently unavailable");
            });

            return (matches.Count == 1
                ? "Found 1 technician:\n"
                : $"Found {matches.Count} technicians — ask the user which one they mean:\n")
                + string.Join("\n", lines);
        }

        [KernelFunction("check_availability")]
        [Description("Get a technician's free start-times on a specific date. Requires the " +
            "technician's numeric id (get it from find_technicians first) and the date as YYYY-MM-DD.")]
        public async Task<string> CheckAvailability(
            [Description("The technician's numeric id, as returned by find_technicians.")] int technicianId,
            [Description("The date to check, in YYYY-MM-DD format (e.g. 2026-07-02).")] string date)
        {
            _logger.LogInformation(
                "TechnicianTool: check_availability CALLED — technicianId={Id} date='{Date}'",
                technicianId, date);

            if (!DateTime.TryParse(date, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                return "INVALID_DATE: Could not read the date. Provide it as YYYY-MM-DD, e.g. 2026-07-02.";

            var slots = (await _mediator.Send(new GetTechnicianAvailableSlotsQuery
            {
                TechnicianId = technicianId,
                Date = parsedDate.Date
            })).ToList();

            _logger.LogInformation(
                "TechnicianTool: check_availability → {Count} free slot(s) on {Date:yyyy-MM-dd}",
                slots.Count, parsedDate);

            if (slots.Count == 0)
                return $"No free slots for technician {technicianId} on {parsedDate:yyyy-MM-dd} " +
                       "(they may not work that day, or are fully booked). Suggest another day.";

            var times = string.Join(", ", slots.Select(s => s.ToString("HH:mm")));
            return $"Free start-times for technician {technicianId} on {parsedDate:yyyy-MM-dd}: {times}.";
        }
    }
}
