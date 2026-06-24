namespace Neighborhood.Services.Application.Shared
{
    // Resolves a location to one of the price service's region keys (cairo/giza/alex/tanta/mahalla),
    // or null when none applies / it's uncertain. Callers treat null as a general (non-localized)
    // average. Used by the chatbot and the booking price-estimate flows so the coords->city logic
    // lives in exactly one place.
    public interface IRegionResolver
    {
        Task<string?> ResolveAsync(
            double? latitude,
            double? longitude,
            string? text = null,
            string? regionOverride = null,
            CancellationToken cancellationToken = default);
    }
}
