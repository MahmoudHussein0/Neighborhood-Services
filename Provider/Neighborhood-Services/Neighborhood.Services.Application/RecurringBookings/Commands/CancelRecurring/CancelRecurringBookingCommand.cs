using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.RecurringBookings.Commands.CancelRecurring
{
    public class CancelRecurringBookingCommand :IRequest<bool>
    {
        public int RecurringBookingId { get; set; }
        public string? CancellationReason { get; set; }
    }
}
