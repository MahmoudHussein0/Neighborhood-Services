using Neighborhood.Services.Domain.TechnicianPhotos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Domain.Technicians
{
    public class Technician
    {
        public int Id { get; set; }
        public string NationalId { get; set; } = string.Empty;
        public string Experience { get; set; } = string.Empty;
        public Decimal Rating { get; set; }
        public int MaxTravelDistance { get; set; }
        public TechnicianVerificationStatus VerificationStatus { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string ApplicationUserId { get; set; } = string.Empty;


        public ICollection<TechnicianPhoto> TechnicianPhotos { get; set; } 
            = new List<TechnicianPhoto>();
    }
}
