using MediatR;
using Neighborhood.Services.Application.CancellationPolicies.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.CancellationPolicies;

namespace Neighborhood.Services.Application.CancellationPolicies.Commands.DeleteCancellation
{
    public class DeleteCancellationPolicyCommandHandler : IRequestHandler<DeleteCancellationPolicyCommand, bool>
    {
        private readonly ICancellationPolicyRepository _policyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCancellationPolicyCommandHandler(ICancellationPolicyRepository policyRepository, IUnitOfWork unitOfWork)
        {
            _policyRepository = policyRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteCancellationPolicyCommand request, CancellationToken cancellationToken)
        {
            var policy = await _policyRepository.GetByIdAsync(request.Id);

            if (policy is null)
                throw new NotFoundException(nameof(CancellationPolicy), request.Id);

            await _policyRepository.DeleteAsync(request.Id);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
