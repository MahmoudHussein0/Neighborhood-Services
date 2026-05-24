using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Domain.SupportTickets
{
    public enum SupportTicketStatus
    {
        Open = 1,
        InProgress = 2,
        WaitingOnCustomer = 3,
        Resolved = 4
    }
}
