using MediatR;
using Neighborhood.Services.Application.Disputes.Commands;
using Neighborhood.Services.Application.Disputes.DTOs;
using Neighborhood.Services.Application.Disputes.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Disputes;
using Neighborhood.Services.Application.Shared.Mappers;

namespace Neighborhood.Services.Application.Disputes.Handlers
{
    public class UpdateDisputeCommandHandler : IRequestHandler<UpdateDisputeCommand, DisputeDto>
    {
        private readonly IDisputeRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDisputeCommandHandler(IDisputeRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<DisputeDto> Handle(UpdateDisputeCommand request, CancellationToken cancellationToken)
        {
            var dispute = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (dispute is null)
                throw new Exception($"Dispute with id {request.Id} not found.");

            dispute.Status = request.Status;
            dispute.ResolvedByStaffId = request.ResolvedByStaffId;
            dispute.Resolution = request.Resolution;

            if (request.Status == DisputeStatus.Resolved)
                dispute.ResolvedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(dispute);
            await _unitOfWork.SaveChangesAsync();

            return DisputeMapper.MapToDto(dispute);
        }
    }
}
