namespace Neighborhood.Services.Application.Offers.DTOs
{
    public class CreateOfferResultDto
    {
        public int OfferId { get; set; }
        // Non-blocking advisories (e.g. proposed time is outside the technician's usual hours)
        public List<string> Warnings { get; set; } = new();
    }
}
