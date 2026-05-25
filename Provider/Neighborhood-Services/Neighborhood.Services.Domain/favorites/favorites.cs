using Microsoft.Identity.Client;
using Neighborhood.Services.Domain.Shared;
using Neighborhood.Services.Domain.ApplicationUser;
using Neighborhood.Services.Domain.Technicians;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Domain.favorites
{
    public class Favorites :BaseEntity<int>
    {
        public int UserId;
        public int TechnicianId;
        public DateTime addedAt = DateTime.Now;

        //Nav probs
        public ApplicationUser.ApplicationUser User { get; set; } = new ApplicationUser.ApplicationUser();
        //public User
        //public Technician
        public Technician Technician { get; set; } = new Technician();

    }
}
