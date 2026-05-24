using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Domain.RecurringBookings
{
    public class RecurringBooking
    {
        //------------------------ Self Prop 
        public int Id { get; set; }
        public string Address { get; set; } = string.Empty;
        public RecurringPattern Pattern { get; set; }
        public int? DayOfWeek { get; set; }
        public int? DayOfMonth { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        //--------------- Foreign Keys 
        public int CustomerId { get; set; }
        public int TechnicianId { get; set; }
        public int ProblemTypeId { get; set; }
    }
}
