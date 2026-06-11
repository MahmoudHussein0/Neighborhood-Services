using MediatR;
using Microsoft.Extensions.Logging;
using Neighborhood.Services.Application.AgentLogs.Commands;
using Neighborhood.Services.Application.AI.DTOs;
using Neighborhood.Services.Application.AI.Interfaces;
using Neighborhood.Services.Application.Matching.DTOs;
using Neighborhood.Services.Application.ProblemTypes.Interface;
using Neighborhood.Services.Application.Technicians.DTOs;
using Neighborhood.Services.Application.Technicians.Interfaces;
using Neighborhood.Services.Domain.AgentLogs;
using Neighborhood.Services.Domain.Technicians;
using System.Text;
using System.Text.Json;

namespace Neighborhood.Services.Application.Matching.Queries
{
    public class GetTechnicianMatchesQueryHandler
        : IRequestHandler<GetTechnicianMatchesQuery, TechnicianMatchResultDto>
    {
        private readonly ITechnicianRepository _technicianRepository;
        private readonly IProblemTypeRepository _problemTypeRepository;
        private readonly IAiClient _aiClient;
        private readonly IMediator _mediator;
        private readonly ILogger<GetTechnicianMatchesQueryHandler> _logger;

        // How many rule-filtered candidates we hand to the LLM (keeps the call cheap & grounded).
        private const int ShortlistSize = 15;

        public GetTechnicianMatchesQueryHandler(
            ITechnicianRepository technicianRepository,
            IProblemTypeRepository problemTypeRepository,
            IAiClient aiClient,
            IMediator mediator,
            ILogger<GetTechnicianMatchesQueryHandler> logger)
        {
            _technicianRepository = technicianRepository;
            _problemTypeRepository = problemTypeRepository;
            _aiClient = aiClient;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<TechnicianMatchResultDto> Handle(GetTechnicianMatchesQuery request, CancellationToken cancellationToken)
        {
            bool hasCustomerLocation = request.Latitude.HasValue && request.Longitude.HasValue;

            // 1) RULES — hard-filter the candidate pool.
            var candidates = await _technicianRepository.GetActiveForBrowseAsync();

            var scored = candidates
                // must serve the chosen category
                .Where(c => c.Categories.Any(cat => cat.Id == request.CategoryId))
                .Select(c =>
                {
                    double? distanceKm = (hasCustomerLocation && c.Latitude.HasValue && c.Longitude.HasValue)
                        ? HaversineKm(request.Latitude!.Value, request.Longitude!.Value, c.Latitude.Value, c.Longitude.Value)
                        : (double?)null;
                    var (score, reason) = RuleScore(c, distanceKm);
                    return new Scored(c, score, reason, distanceKm);
                })
                // drop technicians clearly outside their own travel range (lenient if unknown)
                .Where(s => s.DistanceKm is null || s.Card.MaxTravelDistance <= 0 || s.DistanceKm <= s.Card.MaxTravelDistance)
                .OrderByDescending(s => s.Score)
                .ToList();

            if (scored.Count == 0)
                return new TechnicianMatchResultDto { RankedByAi = false, Matches = new() };

            var shortlist = scored.Take(ShortlistSize).ToList();

            // What the LLM reasons about: the customer's own words if given, else the problem type.
            var problemText = await BuildProblemTextAsync(request);

            // 2) AI — let the LLM rank/explain the shortlist for this specific problem.
            var ordered = shortlist;          // default = rules order
            bool rankedByAi = false;

            if (!string.IsNullOrWhiteSpace(problemText) && shortlist.Count > 1)
            {
                try
                {
                    var raw = await _aiClient.CompleteAsync(
                        SystemPrompt,
                        BuildCandidatePrompt(problemText, shortlist),
                        null,
                        new AiCallContext
                        {
                            AgentType = AgentType.Matching,
                            Action = "MatchTechnicians",
                            ReferenceType = AgentLogReferenceType.Match,
                            ReferenceId = request.ProblemTypeId
                        });

                    var aiItems = ParseAiRanking(raw);
                    if (aiItems.Count == 0)
                        throw new InvalidOperationException("AI returned no usable ranking.");

                    ordered = ReorderByAi(shortlist, aiItems);
                    rankedByAi = true;
                }
                catch (Exception ex)
                {
                    // 3) FALLBACK — pure rules, and record WHY in AgentLog so it's visible.
                    _logger.LogWarning(ex, "Matchmaking agent failed; falling back to rule-based ranking (category {Category}).", request.CategoryId);
                    await LogFallbackAsync(request.ProblemTypeId, problemText, ex);
                    // ordered stays rules-ordered; rankedByAi stays false
                }
            }

            var matches = ordered
                .Take(request.TopN < 1 ? 2 : request.TopN)
                .Select((s, i) => new TechnicianMatchDto
                {
                    Rank = i + 1,
                    Score = s.Score,
                    Reason = s.Reason,
                    Technician = s.Card
                })
                .ToList();

            return new TechnicianMatchResultDto { RankedByAi = rankedByAi, Matches = matches };
        }

        // ---- problem context ----

        private async Task<string> BuildProblemTextAsync(GetTechnicianMatchesQuery request)
        {
            if (!string.IsNullOrWhiteSpace(request.Description))
                return request.Description.Trim();

            if (request.ProblemTypeId.HasValue)
            {
                var problemType = await _problemTypeRepository.GetByIdAsync(request.ProblemTypeId.Value);
                if (problemType != null)
                    return $"{problemType.NameEn} / {problemType.NameAr}";
            }

            return string.Empty;
        }

        // ---- AI prompt ----

        private const string SystemPrompt = """
            You are a matchmaking agent for a home-services marketplace. Given a customer's
            problem and a list of ELIGIBLE technicians, rank them best-first for THIS specific
            problem. Weigh expertise/specialty fit to the problem, then rating, distance, and
            verification. Only use technicianIds from the provided list — never invent one.
            If the problem is written in Arabic, write the reason in Arabic.
            Return ONLY a JSON array, no extra text, each item exactly:
            {"technicianId": <int>, "reason": "<one short sentence on why this tech fits>"}
            """;

        private static string BuildCandidatePrompt(string problemText, List<Scored> shortlist)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Customer problem: {problemText}");
            sb.AppendLine();
            sb.AppendLine("Eligible technicians (recommend only from these, by technicianId):");
            foreach (var s in shortlist)
            {
                var c = s.Card;
                var categories = string.Join(", ", c.Categories.Select(x => x.NameEn));
                var distance = s.DistanceKm.HasValue ? $"{s.DistanceKm.Value:0.#} km away" : "distance unknown";
                var verified = c.VerificationStatus == TechnicianVerificationStatus.Approved ? "verified" : "not verified";
                var experience = string.IsNullOrWhiteSpace(c.Experience) ? "n/a" : c.Experience;
                sb.AppendLine($"- technicianId={c.Id} | {c.FullName} | rating {c.Rating:0.0}/5 | {distance} | {verified} | categories: {categories} | experience: {experience}");
            }
            return sb.ToString();
        }

        private static List<AiMatchItem> ParseAiRanking(string raw)
        {
            var json = raw.Replace("```json", "").Replace("```", "").Trim();
            var items = JsonSerializer.Deserialize<List<AiMatchItem>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return items ?? new List<AiMatchItem>();
        }

        // Reorder the shortlist to follow the AI's ranking, attaching its per-tech reason.
        // Any candidate the AI omitted is appended in rule order so nobody silently vanishes.
        private static List<Scored> ReorderByAi(List<Scored> shortlist, List<AiMatchItem> aiItems)
        {
            var byId = shortlist.ToDictionary(s => s.Card.Id);
            var used = new HashSet<int>();
            var result = new List<Scored>();

            foreach (var item in aiItems)
            {
                if (byId.TryGetValue(item.TechnicianId, out var s) && used.Add(item.TechnicianId))
                {
                    var reason = string.IsNullOrWhiteSpace(item.Reason) ? s.Reason : item.Reason!.Trim();
                    result.Add(s with { Reason = reason });
                }
            }

            foreach (var s in shortlist)
                if (used.Add(s.Card.Id))
                    result.Add(s);

            return result;
        }

        private async Task LogFallbackAsync(int? problemTypeId, string problemText, Exception ex)
        {
            try
            {
                await _mediator.Send(new CreateAgentLogCommand
                {
                    AgentType = AgentType.Matching,
                    Action = "MatchFallback",
                    Input = problemText,
                    Output = $"LLM ranking failed ({ex.GetType().Name}: {ex.Message}). Fell back to rule-based ranking.",
                    ReferenceType = AgentLogReferenceType.Match,
                    ReferenceId = problemTypeId
                });
            }
            catch
            {
                // Logging the fallback must itself never break matching.
            }
        }

        // ---- Rule-based scoring (always computed; also the fallback ranking) ----

        private static (double Score, string Reason) RuleScore(TechnicianCardDTO c, double? distanceKm)
        {
            double ratingScore = (double)c.Rating / 5.0;
            double distanceScore = distanceKm.HasValue ? Math.Max(0, 1 - distanceKm.Value / 50.0) : 0.5;
            double verifyScore = c.VerificationStatus == TechnicianVerificationStatus.Approved ? 1 : 0;
            double availableScore = c.IsAvailable ? 1 : 0;

            double composite = ratingScore * 0.40 + distanceScore * 0.30 + verifyScore * 0.20 + availableScore * 0.10;

            var parts = new List<string> { $"★{c.Rating:0.0}" };
            if (distanceKm.HasValue) parts.Add($"{distanceKm.Value:0.#} km away");
            if (c.VerificationStatus == TechnicianVerificationStatus.Approved) parts.Add("verified");
            if (c.IsAvailable) parts.Add("available now");

            return (Math.Round(composite * 100, 1), string.Join(" · ", parts));
        }

        private static double HaversineKm(double lat1, double lng1, double lat2, double lng2)
        {
            const double earthRadiusKm = 6371.0;
            double dLat = ToRad(lat2 - lat1);
            double dLng = ToRad(lng2 - lng1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                     + Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2))
                     * Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
            return earthRadiusKm * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }

        private static double ToRad(double degrees) => degrees * Math.PI / 180.0;

        private sealed record Scored(TechnicianCardDTO Card, double Score, string Reason, double? DistanceKm);

        private sealed class AiMatchItem
        {
            public int TechnicianId { get; set; }
            public string? Reason { get; set; }
        }
    }
}
