using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.RecurringBookings.Commands.ResumeRecurring
{
    public class ResumeRecurringBookingCommand: IRequest<bool>
    {
        public int RecurringBookingId { get; set; }
    }
}
