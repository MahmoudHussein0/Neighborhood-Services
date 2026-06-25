using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Neighborhood.Services.Application.AI.Interfaces;
using Neighborhood.Services.Application.Matching.Queries;
using Neighborhood.Services.Application.ProblemTypes.Interface;
using System.ComponentModel;

namespace Neighborhood.Services.Application.Chatbot.Tools
{
    // Semantic Kernel tool that recommends technicians by NEED (not name): the user describes a
    // problem, and we run the existing matchmaking agent (GetTechnicianMatchesQuery — rules filter
    // → LLM rank → rules fallback) and return its ranked picks. We only CALL that query; the Find
    // Technician "Smart Match" path is untouched.
    //
    // The query needs a CategoryId, which the chatbot user doesn't give, so we classify the
    // free-text description into a problemTypeId (same classifier the pricing tool uses) and read
    // its CategoryId. Built per-request because it carries this request's coords (for proximity).
    public class MatchmakingTool
    {
        private readonly IMediator _mediator;
        private readonly IVectorMemory _memory;
        private readonly IProblemTypeRepository _problemTypeRepository;
        private readonly ILogger _logger;
        private readonly double? _latitude;
        private readonly double? _longitude;

        // Cosine similarity is modest for short, colloquial, or Arabic phrasings even when the
        // match is correct, so keep this fairly permissive — a wrong category is recoverable
        // (the user picks from the recommendations), a false NO_MATCH just frustrates them.
        private const float ClassifierConfidenceThreshold = 0.32f;

        public MatchmakingTool(
            IMediator mediator,
            IVectorMemory memory,
            IProblemTypeRepository problemTypeRepository,
            ILogger logger,
            double? latitude,
            double? longitude)
        {
            _mediator = mediator;
            _memory = memory;
            _problemTypeRepository = problemTypeRepository;
            _logger = logger;
            _latitude = latitude;
            _longitude = longitude;
        }

        [KernelFunction("recommend_technician")]
        [Description("Recommend the best-fit technicians for a problem the user DESCRIBES (by need, " +
            "not by name). Use this when the user explains an issue and wants a suitable technician, " +
            "e.g. 'who can fix my leaking AC near me'. Returns a short ranked list with each " +
            "technician's id, rating, and why they fit. (To look someone up by name instead, use " +
            "find_technicians.)")]
        public async Task<string> RecommendTechnician(
            [Description("Short description of the problem the user needs solved, " +
                "e.g. 'leaking AC', 'broken door lock', 'paint a bedroom'.")]
            string problemDescription)
        {
            _logger.LogInformation(
                "MatchmakingTool: recommend_technician CALLED — desc='{Desc}' coords=({Lat},{Lng})",
                problemDescription, _latitude, _longitude);

            // 1. Classify the free-text problem into a known problemTypeId.
            var hits = await _memory.SearchDetailedAsync("problem-types", problemDescription, topK: 1);
            var top = hits.FirstOrDefault();

            if (top is null
                || top.Score < ClassifierConfidenceThreshold
                || !top.Fields.TryGetValue("problemTypeId", out var idStr)
                || !int.TryParse(idStr, out var problemTypeId))
            {
                _logger.LogInformation("MatchmakingTool: NO_MATCH — topScore={Score}", top?.Score);
                // Break the clarification loop: do NOT keep asking for vague details. Ask the user
                // to name the service type, or send them to Find Technician.
                return "NO_MATCH: Could not auto-detect the exact service. Do NOT keep asking the " +
                       "user for more vague details. Instead, ask them to name the service type " +
                       "(e.g. plumbing/سباكة, electrical/كهرباء, carpentry/نجارة, cleaning/تنظيف, " +
                       "painting/دهان), or suggest they use the Find Technician page. Then call " +
                       "recommend_technician again with that service type.";
            }

            // 2. The matchmaking agent keys on CategoryId — read it from the classified problem type.
            var problemType = await _problemTypeRepository.GetByIdAsync(problemTypeId);
            if (problemType is null)
                return "NO_MATCH: Service not found. Ask the user to describe the problem differently.";

            // 3. Run the EXISTING matchmaking agent unchanged (rules + LLM rank + fallback).
            var result = await _mediator.Send(new GetTechnicianMatchesQuery
            {
                CategoryId = problemType.CategoryId,
                ProblemTypeId = problemTypeId,
                Latitude = _latitude,
                Longitude = _longitude,
                Description = problemDescription,
                TopN = 3
            });

            _logger.LogInformation(
                "MatchmakingTool: recommend_technician → {Count} match(es), rankedByAi={Ai}",
                result.Matches.Count, result.RankedByAi);

            if (result.Matches.Count == 0)
                return "No matching technicians were found for this service"
                     + (_latitude.HasValue ? " near the user's location." : ".")
                     + " Suggest the user broaden the request or browse the Find Technician page.";

            // Compact, model-readable lines. The id lets the model chain into check_availability.
            var lines = result.Matches.Select(m =>
            {
                var t = m.Technician;
                return $"id={t.Id} | {t.FullName} | rating={t.Rating:0.0} | fit={m.Score:0}/100 | "
                     + $"reason: {m.Reason}";
            });

            // Surface the matched service id so the model can pass it straight to create_booking
            // (skips re-classifying the problem at booking time).
            return $"matched service #{problemTypeId}: {problemType.NameEn}\n"
                 + "Recommended technicians (best first):\n" + string.Join("\n", lines);
        }
    }
}
