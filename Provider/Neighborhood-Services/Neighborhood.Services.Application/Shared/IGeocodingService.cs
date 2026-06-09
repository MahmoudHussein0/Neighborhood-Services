namespace Neighborhood.Services.Application.Shared
{
    public interface IGeocodingService
    {
        Task<GeocodingResultDto?> GetCoordinatesAsync(string address);
        Task<GeocodingResultDto?> GetAddressAsync(double lat, double lng);
    }
}
