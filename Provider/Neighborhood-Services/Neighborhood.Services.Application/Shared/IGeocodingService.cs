namespace Neighborhood.Services.Application.Shared
{
    public interface IGeocodingService
    {
        Task<GeocodingResultDto?> GeocodeAsync(string address);
    }
}
