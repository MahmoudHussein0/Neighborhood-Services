using Neighborhood.Services.Domain.Categories;
using Neighborhood.Services.Domain.HistoricalPrices;
using Neighborhood.Services.Domain.ServiceRequests;
using Neighborhood.Services.Domain.TechnicionsPricing;

namespace Neighborhood.Services.Domain.ProblemTypes
{
    public class ProblemType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public ICollection<TechnicianPricing> TechnicionPricing { get; set; }
        public ICollection<HistoricalPrice> HistoricalPricing { get; set; }
        public ProblemType()
        {
            TechnicionPricing = new HashSet<TechnicianPricing>();
            HistoricalPricing = new HashSet<HistoricalPrice>();
        }
        //
        public ICollection<ServiceRequest> ServiceRequests { get; set; } = new HashSet<ServiceRequest>();
    }
}
