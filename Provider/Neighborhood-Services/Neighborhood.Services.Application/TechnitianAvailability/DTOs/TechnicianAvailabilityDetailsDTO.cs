using Neighborhood.Services.Domain.Technicians;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.TechnitianAvailability.DTOs
{
    public class TechnicianAvailabilityDetailsDTO
    {

        public int  Id { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}
