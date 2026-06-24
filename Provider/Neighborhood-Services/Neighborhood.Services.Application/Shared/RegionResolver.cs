using Microsoft.Extensions.Logging;
using Neighborhood.Services.Application.AI.Interfaces;
using System.Text;

namespace Neighborhood.Services.Application.Shared
{
    // Turns a location signal (GPS coords and/or free text) into one of the price service's
    // region keys. Strategy, in order:
    //   1. Explicit valid region override.
    //   2. GPS -> reverse-geocode -> DETERMINISTIC match on the structured address fields
    //      (county/city catch neighborhood-level coords where the formatted line omits the city,
    //      e.g. county "Awel Tanta" -> tanta). This never hallucinates: it only matches when the
    //      real address literally names one of our cities.
    //   3. LLM fallback (only if the above found nothing) — maps coords/text to the nearest known
    //      city. Guarded by a hard haversine distance check so a far-away point (e.g. Luxor) can
    //      NEVER be mapped to a known city no matter what the model says.
    // Returns null on anything uncertain; callers treat null as a general (non-localized) average.
    public class RegionResolver : IRegionResolver
    {
        private readonly IGeocodingService _geocodingService;
        private readonly IAiClient _aiClient;
        private readonly ILogger<RegionResolver> _logger;

        public RegionResolver(
            IGeocodingService geocodingService,
            IAiClient aiClient,
            ILogger<RegionResolver> logger)
        {
            _geocodingService = geocodingService;
            _aiClient = aiClient;
            _logger = logger;
        }

        // The region keys the price service understands, each with its city center (lat, lng) and
        // the name aliases (English + Arabic) we look for in an address or message. SOURCE OF TRUTH
        // for the keys: PriceEstimationService.GetRegionMultiplier — keep this list in sync.
        private static readonly RegionInfo[] Regions =
        {
            new("cairo",   30.0444, 31.2357, new[] { "cairo", "القاهرة", "قاهرة" }),
            new("giza",    30.0131, 31.2089, new[] { "giza", "الجيزة", "الجيزه", "جيزة" }),
            new("alex",    31.2001, 29.9187, new[] { "alexandria", "alexandrie", "alex", "الإسكندرية", "الاسكندرية", "اسكندرية", "إسكندرية" }),
            new("tanta",   30.7865, 31.0004, new[] { "tanta", "طنطا" }),
            new("mahalla", 30.9700, 31.1669, new[] { "mahalla", "mehalla", "محلة", "المحلة" }),
        };

        // Max distance (km) a GPS point may be from a city's center for the LLM fallback to be
        // trusted. Generous enough to cover a metro area, small enough to reject other governorates.
        private const double MaxDistanceKm = 40;

        public async Task<string?> ResolveAsync(
            double? latitude,
            double? longitude,
            string? text = null,
            string? regionOverride = null,
            CancellationToken cancellationToken = default)
        {
            // 1. Explicit override wins if it's already a known key.
            if (!string.IsNullOrWhiteSpace(regionOverride))
            {
                var key = regionOverride.Trim().ToLowerInvariant();
                if (Regions.Any(r => r.Key == key))
                    return key;
            }

            var hasCoords = latitude.HasValue && longitude.HasValue;

            // 2. GPS path: reverse-geocode and deterministically match the structured fields.
            if (hasCoords)
            {
                GeocodingResultDto? geo = null;
                try
                {
                    geo = await _geocodingService.GetAddressAsync(latitude!.Value, longitude!.Value);
                }
                catch (Exception ex)
                {
                    // Geocoding is best-effort (e.g. Geoapify down) — fall through to the LLM.
                    _logger.LogWarning(ex, "RegionResolver: reverse-geocode failed.");
                }

                if (geo is not null)
                {
                    var matched = MatchByName(geo.City, geo.County, geo.State, geo.FormattedAddress);
                    _logger.LogInformation(
                        "RegionResolver: city='{City}' county='{County}' state='{State}' deterministic='{Matched}'",
                        geo.City, geo.County, geo.State, matched ?? "(none)");
                    if (matched is not null)
                        return matched;
                }

                // Deterministic miss — LLM fallback, but only accept a city the coords are actually near.
                return await LlmResolveAsync(latitude, longitude, text);
            }

            // 3. No coords — match the text, then LLM fallback (no distance guard without coords).
            if (!string.IsNullOrWhiteSpace(text))
            {
                var matched = MatchByName(text);
                if (matched is not null)
                    return matched;
                return await LlmResolveAsync(null, null, text);
            }

            return null;
        }

        // Returns the first known region whose alias appears in any of the candidate strings, or null.
        private static string? MatchByName(params string?[] candidates)
        {
            foreach (var candidate in candidates)
            {
                if (string.IsNullOrWhiteSpace(candidate))
                    continue;

                var text = candidate.ToLowerInvariant();
                foreach (var region in Regions)
                {
                    if (region.Aliases.Any(a => text.Contains(a)))
                        return region.Key;
                }
            }

            return null;
        }

        private async Task<string?> LlmResolveAsync(double? lat, double? lng, string? text)
        {
            var hasCoords = lat.HasValue && lng.HasValue;
            if (!hasCoords && string.IsNullOrWhiteSpace(text))
                return null;

            var userPrompt = new StringBuilder();
            if (hasCoords)
                userPrompt.AppendLine($"GPS coordinates: lat={lat!.Value}, lng={lng!.Value}");
            if (!string.IsNullOrWhiteSpace(text))
                userPrompt.AppendLine($"User message: {text}");

            const string systemPrompt =
                "You identify which Egyptian city a user is in and map it to exactly ONE key from: " +
                "cairo, giza, alex, tanta, mahalla. " +
                "Reference coordinates (lat, lng): cairo=30.04,31.24; giza=30.01,31.21; " +
                "alex=31.20,29.92; tanta=30.79,31.00; mahalla=30.97,31.17. " +
                "If GPS coordinates are given, choose the nearest reference city ONLY if it is within " +
                "about 25 km; otherwise answer none. " +
                "If a city name appears in the text (English or Arabic, e.g. \"طنطا\" -> tanta, " +
                "\"الإسكندرية\" -> alex), use it. " +
                "Do NOT guess or default to the biggest city when unsure. " +
                "Reply with ONLY the single lowercase key, or exactly: none.";

            string raw;
            try
            {
                raw = await _aiClient.CompleteAsync(systemPrompt, userPrompt.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "RegionResolver: LLM fallback failed; treating region as unknown.");
                return null;
            }

            var key = raw?.Trim().ToLowerInvariant() ?? string.Empty;
            var region = Regions.FirstOrDefault(r => r.Key == key);
            if (region is null)
            {
                _logger.LogInformation("RegionResolver LLM: raw='{Raw}' resolved='(none)'", raw);
                return null;
            }

            // Hard safety net: if we have coords, the LLM's pick must be physically close to that
            // city. This makes it impossible for e.g. Luxor coords to be mapped to Cairo, no matter
            // how the model reasons about "nearest".
            if (hasCoords)
            {
                var distanceKm = HaversineKm(lat!.Value, lng!.Value, region.Lat, region.Lng);
                if (distanceKm > MaxDistanceKm)
                {
                    _logger.LogInformation(
                        "RegionResolver LLM: rejected '{Key}' — {Distance:0}km from its center (> {Max}km).",
                        region.Key, distanceKm, MaxDistanceKm);
                    return null;
                }
            }

            _logger.LogInformation("RegionResolver LLM: raw='{Raw}' resolved='{Result}'", raw, region.Key);
            return region.Key;
        }

        // Great-circle distance between two lat/lng points, in kilometres.
        private static double HaversineKm(double lat1, double lng1, double lat2, double lng2)
        {
            const double earthRadiusKm = 6371.0;
            var dLat = ToRadians(lat2 - lat1);
            var dLng = ToRadians(lng2 - lng1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                  + Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2))
                  * Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
            return earthRadiusKm * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }

        private static double ToRadians(double degrees) => degrees * Math.PI / 180.0;

        private sealed record RegionInfo(string Key, double Lat, double Lng, string[] Aliases);
    }
}
