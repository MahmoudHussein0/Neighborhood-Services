using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Neighborhood.Services.Application.Shared;

namespace Neighborhood.Services.Infrastructure.Services
{
    public class GeoapifyGeocodingService(HttpClient httpClient, IConfiguration configuration) : IGeocodingService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly IConfiguration _configuration = configuration;

        public async Task<GeocodingResultDto?> GetCoordinatesAsync(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                return null;
            }

            var requestUrl = $"geocode/search?text={Uri.EscapeDataString(address)}&apiKey={Uri.EscapeDataString(GetApiKey())}";
            return await SendGeocodingRequestAsync(requestUrl);
        }

        public async Task<GeocodingResultDto?> GetAddressAsync(double lat, double lng)
        {
            var requestUrl = $"geocode/reverse?lat={lat}&lon={lng}&apiKey={Uri.EscapeDataString(GetApiKey())}";
            return await SendGeocodingRequestAsync(requestUrl);
        }

        private async Task<GeocodingResultDto?> SendGeocodingRequestAsync(string requestUrl)
        {
            using var response = await _httpClient.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            await using var responseStream = await response.Content.ReadAsStreamAsync();
            var geocodingResponse = await JsonSerializer.DeserializeAsync<GeoapifyGeocodingResponse>(
                responseStream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (geocodingResponse == null ||
                geocodingResponse.Features.Count == 0)
            {
                return null;
            }

            var result = geocodingResponse.Features[0];
            var longitude = result.Properties.Lon ?? GetCoordinate(result.Geometry.Coordinates, 0);
            var latitude = result.Properties.Lat ?? GetCoordinate(result.Geometry.Coordinates, 1);
            if (longitude == null || latitude == null)
            {
                return null;
            }

            return new GeocodingResultDto
            {
                FormattedAddress = result.Properties.Formatted,
                Latitude = latitude.Value,
                Longitude = longitude.Value,
                City = result.Properties.City,
                County = result.Properties.County,
                State = result.Properties.State
            };
        }

        private static double? GetCoordinate(List<double> coordinates, int index)
        {
            return coordinates.Count > index ? coordinates[index] : null;
        }

        private string GetApiKey()
        {
            var apiKey = _configuration["Geoapify:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("Geoapify API key is not configured.");
            }

            return apiKey;
        }

        private sealed class GeoapifyGeocodingResponse
        {
            public List<GeoapifyFeature> Features { get; set; } = [];
        }

        private sealed class GeoapifyFeature
        {
            public GeoapifyProperties Properties { get; set; } = new();
            public GeoapifyGeometry Geometry { get; set; } = new();
        }

        private sealed class GeoapifyProperties
        {
            public string Formatted { get; set; } = string.Empty;
            public double? Lat { get; set; }
            public double? Lon { get; set; }
            public string? City { get; set; }
            public string? County { get; set; }
            public string? State { get; set; }
        }

        private sealed class GeoapifyGeometry
        {
            public List<double> Coordinates { get; set; } = [];
        }
    }
}
