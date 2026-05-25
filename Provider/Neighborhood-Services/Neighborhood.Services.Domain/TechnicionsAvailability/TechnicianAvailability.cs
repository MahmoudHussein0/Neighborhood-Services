using Neighborhood.Services.Domain.Technicians;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Domain.TechnicionsAvailability
{
    public class TechnicianAvailability
    {
        public int  Id { get; set; }
        public int TechnicianId { get; set; }
        public int DayOfWeek { get; set; }
        public bool IsDeleted { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public Technician Technician { get; set; }


    }
}
