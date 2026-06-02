using MediatR;
using Neighborhood.Services.Application.Disputes.Commands;
using Neighborhood.Services.Application.Disputes.DTOs;
using Neighborhood.Services.Application.Disputes.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Disputes;
using Neighborhood.Services.Application.Shared.Mappers;


namespace Neighborhood.Services.Application.Disputes.Handlers
{
    public class CreateDisputeCommandHandler : IRequestHandler<CreateDisputeCommand, DisputeDto>
    {
        private readonly IDisputeRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateDisputeCommandHandler(IDisputeRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<DisputeDto> Handle(CreateDisputeCommand request, CancellationToken cancellationToken)
        {
            var dispute = new Dispute
            {
                BookingId = request.BookingId,
                RaisedBy = request.RaisedBy,
                DisputeType = request.DisputeType,
                Reason = request.Reason,
                Status = DisputeStatus.Open,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(dispute);
            await _unitOfWork.SaveChangesAsync();

            return DisputeMapper.MapToDto(dispute);
        }
    }
}
