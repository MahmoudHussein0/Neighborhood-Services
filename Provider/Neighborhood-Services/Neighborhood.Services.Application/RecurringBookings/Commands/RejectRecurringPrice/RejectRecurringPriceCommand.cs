using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.RecurringBookings.Commands.RejectRecurringPrice
{
    public class RejectRecurringPriceCommand :IRequest<bool>
    {
        public int RecurringBookingId { get; set; }
        public string? RejectionReason { get; set; }
    }
}
