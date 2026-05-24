using Neighborhood.Services.Domain.Technicians;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Domain.TechnicianPhotos
{
    public class TechnicianPhoto
    {
        public int Id { get; set; }
        public string PhotoUrl { get; set; } = string.Empty;
        public string Caption { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }


        public string ApplicationUserId { get; set; } = string.Empty;

        public int TechnicianId { get; set; }
        public Technician Technician { get; set; } = null!;
    }
}