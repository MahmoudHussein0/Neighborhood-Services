using Neighborhood.Services.Domain.ApplicationUsers;
using Neighborhood.Services.Domain.Technicians;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Favorites.DTOs
{
    public class FavoriteDto
    {
        public int FavoriteId { get; set; }

        public string UserId { set; get; }
        public int TechnicianId { get; set; }

        public string TechnicianName { set; get; }
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