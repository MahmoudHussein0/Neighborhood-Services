using Microsoft.Identity.Client;
using Neighborhood.Services.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Domain.favorites
{
    public class favorites :BaseEntity<int>
    {
        public int UserId;
        public int TechnicianId;
        public DateTime addedAt = DateTime.Now;

        //Nav probs
        //public User
        //public Technician

    }
}
