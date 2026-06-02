using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.RecurringBookings.Commands.SetRecurringPrice
{
    public class SetRecurringPriceCommand : IRequest<bool>
    {
        public int RecurringBookingId { get; set; }
        public decimal Price { get; set; }
    }
}
