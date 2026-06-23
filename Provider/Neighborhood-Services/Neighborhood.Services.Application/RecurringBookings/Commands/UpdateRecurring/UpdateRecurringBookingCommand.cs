using MediatR;
using Neighborhood.Services.Domain.RecurringBookings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.RecurringBookings.Commands.UpdateRecurring
{
    public class UpdateRecurringBookingCommand : IRequest<bool>
    {
        public int RecurringBookingId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string Address { get; set; } = string.Empty;
        public RecurringPattern Pattern { get; set; }
        public DayOfWeek? DayOfWeek { get; set; }
        public int? DayOfMonth { get; set; }
        public TimeOnly TimeOfDay { get; set; }
        public int DurationMinutes { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}
