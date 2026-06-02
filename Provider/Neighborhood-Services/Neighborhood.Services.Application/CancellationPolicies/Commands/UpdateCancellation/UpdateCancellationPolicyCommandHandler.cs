using MediatR;
using Neighborhood.Services.Application.CancellationPolicies.DTOs;
using Neighborhood.Services.Application.CancellationPolicies.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.CancellationPolicies;

namespace Neighborhood.Services.Application.CancellationPolicies.Commands.UpdateCancellation
{
    public class UpdateCancellationPolicyCommandHandler : IRequestHandler<UpdateCancellationPolicyCommand, CancellationPolicyDto>
    {
        private readonly ICancellationPolicyRepository _policyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCancellationPolicyCommandHandler(ICancellationPolicyRepository policyRepository, IUnitOfWork unitOfWork)
        {
            _policyRepository = policyRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CancellationPolicyDto> Handle(UpdateCancellationPolicyCommand request, CancellationToken cancellationToken)
        {
            var policy = await _policyRepository.GetByIdAsync(request.Id);
            if (policy is null)
                throw new NotFoundException(nameof(CancellationPolicy), request.Id);
            // checking if its valid 
            if (request.PenaltyPct < 0 || request.PenaltyPct > 100)
                throw new ValidationException("Penalty percentage must be between 0 and 100");

            if (request.HoursBeforeBooking < 0)
                throw new ValidationException("Hours before booking must be positive");

            // checking if its already exists 
            var existing = await _policyRepository
            .GetPolicyAsync(request.HoursBeforeBooking, request.AppliesTo);

            if (existing != null && existing.Id != request.Id)
                throw new ConflictException("A policy already exists for this hours and target combination");

            policy.HoursBeforeBooking = request.HoursBeforeBooking;
            policy.PenaltyPct = request.PenaltyPct;
            policy.AppliesTo = request.AppliesTo;

            await _policyRepository.UpdateAsync(policy);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

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
