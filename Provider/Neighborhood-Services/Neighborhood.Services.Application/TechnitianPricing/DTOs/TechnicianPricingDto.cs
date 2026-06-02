namespace Neighborhood.Services.Application.TechnitianPricing.DTOs
{
    public class TechnicianPricingDto
    {
        public string ProblemTypeName { get; set; }
        public decimal TechPriceMinPrice { get; set; }
        public decimal TechPriceMaxPrice { get; set; }
    }
}
