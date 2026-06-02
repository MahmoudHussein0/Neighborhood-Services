using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.RecurringBookings.Commands.ApproveRecurring
{
    public class ApproveRecurringPriceCommand : IRequest<bool>
    {
        public int RecurringBookingId { get; set; }
    }
}
