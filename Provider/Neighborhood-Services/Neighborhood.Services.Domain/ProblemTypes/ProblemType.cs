using Neighborhood.Services.Domain.Categories;
using Neighborhood.Services.Domain.HistoricalPrices;
using Neighborhood.Services.Domain.Shared;
using Neighborhood.Services.Domain.TechniciansPricing;
using Neighborhood.Services.Domain.ServiceRequests;

namespace Neighborhood.Services.Domain.ProblemTypes
{
    public class ProblemType :BaseEntity<int>
    {
        public string NameEn { get; set; } = null!;
        public string NameAr { get; set; } = null!;
        public string DescriptionEn { get; set; } = null!;
        public string DescriptionAr { get; set; } = null!;
        public string? ImageUrl { get; set; } = null!;

        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
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
