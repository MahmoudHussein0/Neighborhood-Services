using Neighborhood.Services.Domain.CancellationPolicies;

namespace Neighborhood.Services.Application.CancellationPolicies.DTOs
{
    public class CancellationPolicyDto
    {
        public int Id { get; set; }
        public int HoursBeforeBooking { get; set; }
        public decimal PenaltyPct { get; set; }
        public CancellationPolicyTarget AppliesTo { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
