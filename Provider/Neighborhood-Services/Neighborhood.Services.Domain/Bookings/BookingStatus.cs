using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Domain.Bookings
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
