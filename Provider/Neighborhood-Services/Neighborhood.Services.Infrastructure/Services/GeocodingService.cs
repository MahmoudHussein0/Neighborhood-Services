using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Neighborhood.Services.Application.Shared;

namespace Neighborhood.Services.Infrastructure.Services
{
    public class GeocodingService(IHttpClientFactory httpClientFactory, IConfiguration configuration) : IGeocodingService
    {
        private const string GeocodingUrl = "https://maps.googleapis.com/maps/api/geocode/json";
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly IConfiguration _configuration = configuration;

        public async Task<GeocodingResultDto?> GeocodeAsync(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                return null;
            }

            var apiKey = _configuration["GoogleMaps:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("Google Maps API key is not configured.");
            }

            var requestUrl = $"{GeocodingUrl}?address={WebUtility.UrlEncode(address)}&key={WebUtility.UrlEncode(apiKey)}";
            var httpClient = _httpClientFactory.CreateClient();

            using var response = await httpClient.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            await using var responseStream = await response.Content.ReadAsStreamAsync();
            var geocodingResponse = await JsonSerializer.DeserializeAsync<GoogleGeocodingResponse>(
                responseStream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (geocodingResponse == null ||
                geocodingResponse.Status != "OK" ||
                geocodingResponse.Results.Count == 0)
            {
                return null;
            }

            var result = geocodingResponse.Results[0];
            return new GeocodingResultDto
            {
                FormattedAddress = result.FormattedAddress,
                Latitude = result.Geometry.Location.Lat,
                Longitude = result.Geometry.Location.Lng
            };
        }

        private sealed class GoogleGeocodingResponse
        {
            public string Status { get; set; } = string.Empty;

            public List<GoogleGeocodingResult> Results { get; set; } = [];

            [JsonPropertyName("error_message")]
            public string? ErrorMessage { get; set; }
        }

        private sealed class GoogleGeocodingResult
        {
            [JsonPropertyName("formatted_address")]
            public string FormattedAddress { get; set; } = string.Empty;

            public GoogleGeometry Geometry { get; set; } = new();
        }

        private sealed class GoogleGeometry
        {
            public GoogleLocation Location { get; set; } = new();
        }

        private sealed class GoogleLocation
        {
            public double Lat { get; set; }
            public double Lng { get; set; }
        }
    }
}
