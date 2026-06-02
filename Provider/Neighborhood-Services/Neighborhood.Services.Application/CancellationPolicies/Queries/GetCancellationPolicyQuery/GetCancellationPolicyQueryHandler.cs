using MediatR;
using Neighborhood.Services.Application.CancellationPolicies.DTOs;
using Neighborhood.Services.Application.CancellationPolicies.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Domain.CancellationPolicies;

namespace Neighborhood.Services.Application.CancellationPolicies.Queries.GetCancellationPolicyQuery
{
    public class GetCancellationPolicyQueryHandler : IRequestHandler<GetCancellationPolicyQuery, CancellationPolicyDto>
    {
        private readonly ICancellationPolicyRepository _policyRepository;

        public GetCancellationPolicyQueryHandler(ICancellationPolicyRepository policyRepository)
        {
            _policyRepository = policyRepository;
        }

        public async Task<CancellationPolicyDto> Handle(GetCancellationPolicyQuery request, CancellationToken cancellationToken)
        {
            var policy = await _policyRepository.GetPolicyAsync(request.HoursBeforeBooking, request.AppliesTo);

            if (policy is null)
                throw new NotFoundException(nameof(CancellationPolicy), $"{request.HoursBeforeBooking}h/{request.AppliesTo}");

            return new CancellationPolicyDto
            {
                Id = policy.Id,
                HoursBeforeBooking = policy.HoursBeforeBooking,
                PenaltyPct = policy.PenaltyPct,
                AppliesTo = policy.AppliesTo,
                CreatedAt = policy.CreatedAt
            };
        }
    }
}
