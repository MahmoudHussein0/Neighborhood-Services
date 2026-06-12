namespace Neighborhood.Services.Application.Bookings.DTOs
{
    // Slim view of a single (TechnicianPricing) row used inside the booking flow.
    // Owned by the Bookings feature; doesn't depend on the TechnicianPricing DTO.
    public class TechnicianPricingRangeDto
    {
        public int TechnicianId { get; set; }
        public int ProblemTypeId { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
    }
}
