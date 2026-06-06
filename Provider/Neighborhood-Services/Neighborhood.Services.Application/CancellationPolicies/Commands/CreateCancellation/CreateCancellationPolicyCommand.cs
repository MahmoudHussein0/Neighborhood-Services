using MediatR;
using Neighborhood.Services.Application.CancellationPolicies.DTOs;
using Neighborhood.Services.Domain.CancellationPolicies;

namespace Neighborhood.Services.Application.CancellationPolicies.Commands.CreateCancellation
{
    public class CreateCancellationPolicyCommand : IRequest<CancellationPolicyDto>
    {
        public int HoursBeforeBooking { get; set; }
        public decimal PenaltyPct { get; set; }
        public CancellationPolicyTarget AppliesTo { get; set; }
    }
}
