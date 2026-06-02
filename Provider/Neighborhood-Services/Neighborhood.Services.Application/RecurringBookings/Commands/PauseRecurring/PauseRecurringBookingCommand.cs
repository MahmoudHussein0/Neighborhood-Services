using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.RecurringBookings.Commands.PauseRecurring
{
    public class PauseRecurringBookingCommand : IRequest<bool>
    {
        public int RecurringBookingId { get; set; }
    }
}
