using MediatR;
using Neighborhood.Services.Application.CancellationPolicies.DTOs;
using Neighborhood.Services.Application.CancellationPolicies.Interfaces;

namespace Neighborhood.Services.Application.CancellationPolicies.Queries.GetAllCancellationPoliciesQuery
{
    public class GetAllCancellationPoliciesQueryHandler : IRequestHandler<GetAllCancellationPoliciesQuery, IEnumerable<CancellationPolicyDto>>
    {
        private readonly ICancellationPolicyRepository _policyRepository;

        public GetAllCancellationPoliciesQueryHandler(ICancellationPolicyRepository policyRepository)
        {
            _policyRepository = policyRepository;
        }

        public async Task<IEnumerable<CancellationPolicyDto>> Handle(GetAllCancellationPoliciesQuery request, CancellationToken cancellationToken)
        {
            var policies = await _policyRepository.GetByConditionAsync(P => !P.IsDeleted);

            return policies.Select(p => new CancellationPolicyDto
            {
                Id = p.Id,
                HoursBeforeBooking = p.HoursBeforeBooking,
                PenaltyPct = p.PenaltyPct,
                AppliesTo = p.AppliesTo,
                CreatedAt = p.CreatedAt
            });
        }
    }
}
