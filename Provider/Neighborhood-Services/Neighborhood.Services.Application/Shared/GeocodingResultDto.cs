namespace Neighborhood.Services.Application.Shared
{
    public class GeocodingResultDto
    {
        public string FormattedAddress { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        // Structured place fields from reverse-geocoding (any may be null). Needed because the
        // formatted line often omits the city at neighborhood-level coords (e.g. it reads
        // "Om Al Mou'menin" while county = "Awel Tanta"). Existing callers can ignore these.
        public string? City { get; set; }
        public string? County { get; set; }
        public string? State { get; set; }
    }
}
