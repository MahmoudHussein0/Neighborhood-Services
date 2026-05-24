using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Domain.Disputes
{
    public enum DisputeType
    {
        PaymentIssue = 1,
        TechnicianBehavior = 2,
        PoorService = 3,
        Scam = 4,
        Other = 5
    }
}
