using MediatR;
using Neighborhood.Services.Application.CancellationPolicies.DTOs;

namespace Neighborhood.Services.Application.CancellationPolicies.Queries.GetAllCancellationPoliciesQuery
{
    public class GetAllCancellationPoliciesQuery : IRequest<IEnumerable<CancellationPolicyDto>>
    {
    }
}
