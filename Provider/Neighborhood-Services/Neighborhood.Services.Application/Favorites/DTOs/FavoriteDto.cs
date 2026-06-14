using Neighborhood.Services.Domain.ApplicationUsers;
using Neighborhood.Services.Domain.Technicians;
using System;
using System.Collections.Generic;
using Neighborhood.Services.Application.Technicians.DTOs;
using System.Text;

namespace Neighborhood.Services.Application.Favorites.DTOs
{
    public class FavoriteDto
    {
        public int favoriteId { get; set; }

        public string userId { set; get; }

       
        public int technicianId { get; set; }

        public int customerId { set; get; }

        public ComprehensiveTechDTO technician { set; get; }

        public string technicianName { set; get; }
        public string imageURL { set; get; } = "https://www.flaticon.com/free-icon/technician_1085421";

        public DateTime addedAt { get; set; } = DateTime.Now;


    }
}

//public string UserId { set; get; }
//public int TechnicianId { set; get; }
//public DateTime addedAt { get; } = DateTime.Now;

////Nav probs
//public ApplicationUser User { get; set; } = null;
////public User
////public Technician
//public Technician Technician { get; set; } = null;