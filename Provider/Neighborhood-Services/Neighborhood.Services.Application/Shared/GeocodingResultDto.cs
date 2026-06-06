namespace Neighborhood.Services.Application.Shared
{
    public class GeocodingResultDto
    {
        public string FormattedAddress { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
