using MediatR;
using Neighborhood.Services.Application.CancellationPolicies.DTOs;
using Neighborhood.Services.Domain.CancellationPolicies;

namespace Neighborhood.Services.Application.CancellationPolicies.Queries.GetCancellationPolicyQuery
{
    public class GetCancellationPolicyQuery : IRequest<CancellationPolicyDto>
    {
        public int HoursBeforeBooking { get; set; }
        public CancellationPolicyTarget AppliesTo { get; set; }
    }
}
