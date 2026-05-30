
using Neighborhood.Services.Domain.ProblemTypes;
using Neighborhood.Services.Domain.Shared;
using Neighborhood.Services.Domain.Technicians;

namespace Neighborhood.Services.Domain.TechniciansPricing
{
    public class TechnicianPricing :BaseEntity<int>
    {
        public int TechnicianId { get; set; }
        public int ProblemTypeId { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ProblemType ProblemType { get; set; }
        public Technician Technician { get; set; }
    }
}
