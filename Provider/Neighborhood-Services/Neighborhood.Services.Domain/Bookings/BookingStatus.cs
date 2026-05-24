using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Domain.Booking
{
    public  enum BookingStatus
    {
        Pending,
        Confirmed,
        Completed,
        Cancelled,
        Disputed
    }
}
