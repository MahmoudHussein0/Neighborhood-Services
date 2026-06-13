using Neighborhood.Services.Domain.Shared;
using Neighborhood.Services.Domain.Technicians;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Domain.TechniciansAvailability
{
    public class TechnicianAvailability :BaseEntity<int>
    {
        public int  TechnicianId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public Technician Technician { get; set; }


    }
}
