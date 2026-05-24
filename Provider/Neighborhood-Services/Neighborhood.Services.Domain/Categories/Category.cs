using Neighborhood.Services.Domain.ProblemTypes;
using Neighborhood.Services.Domain.TechnicionCategories;
namespace Neighborhood.Services.Domain.Categories
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<ProblemType> ProblemTypes { get; set; }
        public ICollection<TechnicianCategory> TechnicianCategory { get; set; }
        public Category()
        {
            ProblemTypes = new HashSet<ProblemType>();
            TechnicianCategory =  new HashSet<TechnicianCategory>();
        }
    }
}
