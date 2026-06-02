using Neighborhood.Services.Domain.Bookings;
using Neighborhood.Services.Domain.Customers;
using Neighborhood.Services.Domain.ProblemTypes;
using Neighborhood.Services.Domain.Shared;
using Neighborhood.Services.Domain.Technicians;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Domain.RecurringBookings
{
    public class RecurringBooking :BaseEntity<int>
    {
        //------------------------ Self Prop 
        public string Address { get; set; } = string.Empty;
        public RecurringPattern Pattern { get; set; }
        public DayOfWeek? DayOfWeek { get; set; }
        public int? DayOfMonth { get; set; }
        // Time of day each occurrence takes place, and how long it lasts
        public TimeOnly TimeOfDay { get; set; }
        public int DurationMinutes { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        // public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal? AgreedPrice { get; set; }        // technician sets this
        public RecurringBookingStatus Status { get; set; }
        public string? CancelledBy { get; set; }
        public DateTime? CancelledAt { get; set; }
        //--------------- Foreign Keys 
        public int CustomerId { get; set; }
        public int TechnicianId { get; set; }
        public int ProblemTypeId { get; set; }

        // Nav 
        public Customer Customer { get; set; } = null!;
        public Technician Technician { get; set; } = null!;
        public ProblemType ProblemType { get; set; } = null!;
        public ICollection<Booking> Bookings { get; set; } = new HashSet<Booking>();
    }
}
