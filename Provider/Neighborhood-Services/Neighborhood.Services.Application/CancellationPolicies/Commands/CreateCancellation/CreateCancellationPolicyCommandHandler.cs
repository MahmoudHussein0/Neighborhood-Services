using MediatR;
using Neighborhood.Services.Application.CancellationPolicies.DTOs;
using Neighborhood.Services.Application.CancellationPolicies.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.CancellationPolicies;

namespace Neighborhood.Services.Application.CancellationPolicies.Commands.CreateCancellation
{
    public class CreateCancellationPolicyCommandHandler : IRequestHandler<CreateCancellationPolicyCommand, CancellationPolicyDto>
    {
        private readonly ICancellationPolicyRepository _policyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateCancellationPolicyCommandHandler(ICancellationPolicyRepository policyRepository, IUnitOfWork unitOfWork)
        {
            _policyRepository = policyRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CancellationPolicyDto> Handle(CreateCancellationPolicyCommand request, CancellationToken cancellationToken)
        {
            // validate if it exists already 
            var existing = await _policyRepository
                 .GetPolicyAsync(request.HoursBeforeBooking, request.AppliesTo);

            if (existing != null)
                throw new ConflictException("A policy already exists for this hours and target combination");

            // validate if the penality is percent is okay.
            if (request.PenaltyPct < 0 || request.PenaltyPct > 100)
                throw new ValidationException("Penalty percentage must be between 0 and 100");

            if (request.HoursBeforeBooking < 0)
                throw new ValidationException("Hours before booking must be positive");


            var policy = new CancellationPolicy
            {
                HoursBeforeBooking = request.HoursBeforeBooking,
                PenaltyPct = request.PenaltyPct,
                AppliesTo = request.AppliesTo,
                CreatedAt = DateTime.UtcNow
            };

            await _policyRepository.AddAsync(policy);
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
