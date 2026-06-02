using Neighborhood.Services.Domain.ApplicationUsers;
using Neighborhood.Services.Domain.Shared;
using Neighborhood.Services.Domain.Technicians;

namespace Neighborhood.Services.Domain.favorites
{
    public class Favorite : BaseEntity<int>
    {
        public string UserId { set; get; }
        public int TechnicianId { set; get; }
        public DateTime addedAt { get; } 

        //Nav probs
        public ApplicationUser User { get; set; } = null;
        //public User
        //public Technician
        public Technician Technician { get; set; } = null;

    }
}
