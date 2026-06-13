using Neighborhood.Services.Domain.ProblemTypes;
using Neighborhood.Services.Domain.Shared;
using Neighborhood.Services.Domain.TechnicianCategories;
using Neighborhood.Services.Domain.ServiceRequests;
namespace Neighborhood.Services.Domain.Categories
{
    public class Category :BaseEntity<int>
    {
        public string NameEn { get; set; } = null!;
        public string NameAr { get; set; } = null!;
        public string Icon { get; set; } = null!;
        public string? Image { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public ICollection<ProblemType> ProblemTypes { get; set; }
        public ICollection<TechnicianCategory> TechnicianCategories { get; set; }
        public Category()
        {
            ProblemTypes = new HashSet<ProblemType>();
            TechnicianCategories =  new HashSet<TechnicianCategory>();
        }


        public ICollection<ServiceRequest> ServiceRequests { get; set; } = new HashSet<ServiceRequest>();
    }
}
