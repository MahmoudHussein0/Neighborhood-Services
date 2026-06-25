using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Neighborhood.Services.Application.AI.Interfaces;
using Neighborhood.Services.Application.Bookings.Services;
using Neighborhood.Services.Application.Shared;
using System.ComponentModel;

namespace Neighborhood.Services.Application.Chatbot.Tools
{
    // A Semantic Kernel tool the chatbot LLM can call on its own to price a service.
    // It encapsulates the two steps pricing needs: (1) classify the user's free-text problem
    // into a known problemTypeId, and (2) resolve the region (from a city the model passes, or
    // the GPS coords captured this request) — falling back to null = general average.
    //
    // Built per-request because it carries this request's coords. The model fills in
    // serviceDescription / city; everything else is internal.
    public class PricingTool
    {
        private readonly IVectorMemory _memory;
        private readonly IPriceEstimationService _priceService;
        private readonly IRegionResolver _regionResolver;
        private readonly ILogger _logger;
        private readonly double? _latitude;
        private readonly double? _longitude;

        // Pricing needs the EXACT problem type (each has its own price range), so a wrong match
        // means a wrong price — worse than asking for clarification. Keep this STRICT. If Arabic/
        // colloquial questions fail to match, the right fix is normalizing the query before the
        // search (improves accuracy), NOT loosening this (which would mis-price).
        private const float ClassifierConfidenceThreshold = 0.5f;

        public PricingTool(
            IVectorMemory memory,
            IPriceEstimationService priceService,
            IRegionResolver regionResolver,
            ILogger logger,
            double? latitude,
            double? longitude)
        {
            _memory = memory;
            _priceService = priceService;
            _regionResolver = regionResolver;
            _logger = logger;
            _latitude = latitude;
            _longitude = longitude;
        }

        [KernelFunction("estimate_price")]
        [Description("Estimate the approximate price in EGP for a home service or repair. " +
            "Call this whenever the user asks how much a service costs.")]
        public async Task<string> EstimatePrice(
            [Description("Short description of the problem or service the user needs, " +
                "e.g. 'leaking tap', 'AC not cooling', 'paint a room'.")]
            string serviceDescription,
            [Description("The user's city if they have stated it (Cairo, Giza, Alexandria, Tanta, " +
                "or Mahalla). Leave empty if the user has not told you their city.")]
            string? city = null)
        {
            _logger.LogInformation(
                "PricingTool: estimate_price CALLED — desc='{Desc}' city='{City}' coords=({Lat},{Lng})",
                serviceDescription, city ?? "(none)", _latitude, _longitude);

            // 1. Classify the free-text problem into a known problemTypeId.
            var hits = await _memory.SearchDetailedAsync("problem-types", serviceDescription, topK: 1);
            var top = hits.FirstOrDefault();

            if (top is null
                || top.Score < ClassifierConfidenceThreshold
                || !top.Fields.TryGetValue("problemTypeId", out var idStr)
                || !int.TryParse(idStr, out var problemTypeId))
            {
                _logger.LogInformation(
                    "PricingTool: NO_MATCH — topScore={Score} (threshold {Threshold})",
                    top?.Score, ClassifierConfidenceThreshold);
                // Signal the model to ask for a clearer description instead of inventing a price.
                return "NO_MATCH: Could not confidently identify a specific service from that " +
                       "description. Ask the user to describe the problem more specifically.";
            }

            // 2. Resolve the region: prefer a city the user stated (passed by the model), else the
            //    GPS coords captured this request. Null is fine => general (non-localized) average.
            var region = await _regionResolver.ResolveAsync(
                _latitude, _longitude, text: city, regionOverride: city);

            // 3. Grounded estimate from history/rules, region-adjusted.
            var estimate = await _priceService.EstimateAsync(problemTypeId, region);

            _logger.LogInformation(
                "PricingTool: result — problemTypeId={ProblemTypeId} region='{Region}' price={Price:0.##} EGP",
                problemTypeId, region ?? "(general)", estimate);

            // Prefix the matched service id so the model can carry it to create_booking (skips
            // re-classifying the problem at booking time).
            var prefix = $"matched service #{problemTypeId}. ";
            return prefix + (string.IsNullOrWhiteSpace(region)
                ? $"Estimated price: approximately {estimate:0.##} EGP (general average — the user's " +
                  "city is unknown, so ask which city they're in for a more accurate figure)."
                : $"Estimated price: approximately {estimate:0.##} EGP (based on {region}).");
        }
    }
}
