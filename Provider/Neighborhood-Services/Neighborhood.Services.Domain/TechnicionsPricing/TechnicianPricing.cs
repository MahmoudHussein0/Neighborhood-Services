
using Neighborhood.Services.Domain.ProblemTypes;

namespace Neighborhood.Services.Domain.TechnicionsPricing
{
    public class TechnicianPricing
    {
        public int  Id { get; set; }
        public int TechnicionId { get; set; }
        public int ProblemTypeId { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ProblemType ProblemType { get; set; }

        //public Technicion Technicion { get; set; }
    }
}
