using Neighborhood.Services.Domain.ApplicationUsers;
using Neighborhood.Services.Domain.Shared;
using Neighborhood.Services.Domain.Technicians;

namespace Neighborhood.Services.Domain.favorites
{
    public class Favorite : BaseEntity<int>
    {
        public int UserId;
        public int TechnicianId;
        public DateTime addedAt = DateTime.Now;

        public ApplicationUser User { get; set; } = new ApplicationUser();
        public Technician Technician { get; set; } = new Technician();
    }
}
