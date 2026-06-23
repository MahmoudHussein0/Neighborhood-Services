using MediatR;
using Neighborhood.Services.Domain.RecurringBookings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.RecurringBookings.Commands.CreateRecurring
{
    public class CreateRecurringBookingCommand : IRequest<int>
    {
        // CustomerId resolved from current user service
        public int TechnicianId { get; set; }
        public int ProblemTypeId { get; set; }
        // What the job is + an optional reference photo — copied onto each generated booking.
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string Address { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public RecurringPattern Pattern { get; set; }
        public DayOfWeek? DayOfWeek { get; set; }
        public int? DayOfMonth { get; set; }
        public TimeOnly TimeOfDay { get; set; }
        public int DurationMinutes { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}
