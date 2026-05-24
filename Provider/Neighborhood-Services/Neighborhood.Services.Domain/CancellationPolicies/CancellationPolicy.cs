using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Domain.CancellationPolicies
{
    public class CancellationPolicy
    {
        public int Id { get; set; }
        public int HoursBeforeBooking { get; set; }
        public decimal PenaltyPct { get; set; }
        public CancellationPolicyTarget AppliesTo  { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
