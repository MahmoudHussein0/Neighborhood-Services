using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Domain.RecurringBookings
{
    public enum RecurringBookingStatus
    {
        PendingApproval,          // waiting for technician to set price
        PendingCustomerApproval,  // waiting for customer to approve price
        Active,                   // both agreed, generating bookings
        Paused,                   // temporarily paused by customer
        Cancelled                 // cancelled by either side
    }
}
