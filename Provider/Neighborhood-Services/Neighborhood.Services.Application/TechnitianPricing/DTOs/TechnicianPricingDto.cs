using Neighborhood.Services.Domain.Technicians;

namespace Neighborhood.Services.Application.TechnitianPricing.DTOs
{
    public class TechnicianPricingDto
    {
        public int  Id { get; set; }
        public string NationalId { get; set; } = string.Empty;
        public string Experience { get; set; } = string.Empty;
        public decimal Rating { get; set; }
        public int MaxTravelDistance { get; set; }
        public TechnicianVerificationStatus VerificationStatus { get; set; }
        public string ProblemTypeName { get; set; }
        public string ProblemTypeDescription { get; set; }
        public int ProblemTypeId { get; set; }
        public decimal TechMinPrice { get; set; }
        public decimal TechMaxPrice { get; set; }
    }
}
