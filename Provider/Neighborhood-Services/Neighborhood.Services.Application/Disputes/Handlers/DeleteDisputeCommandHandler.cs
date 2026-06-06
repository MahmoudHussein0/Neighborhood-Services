using MediatR;
using Neighborhood.Services.Application.Disputes.Commands;
using Neighborhood.Services.Application.Disputes.Interfaces;
using Neighborhood.Services.Application.Shared;

namespace Neighborhood.Services.Application.Disputes.Handlers
{
    public class DeleteDisputeCommandHandler : IRequestHandler<DeleteDisputeCommand, bool>
    {
        private readonly IDisputeRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteDisputeCommandHandler(IDisputeRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteDisputeCommand request, CancellationToken cancellationToken)
        {
            var dispute = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (dispute is null)
                throw new Exception($"Dispute with id {request.Id} not found.");

            dispute.IsDeleted = true;
            await _repository.UpdateAsync(dispute);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }

}
