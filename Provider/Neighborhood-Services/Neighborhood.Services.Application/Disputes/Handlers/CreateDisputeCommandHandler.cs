using MediatR;
using Neighborhood.Services.Application.Disputes.Commands;
using Neighborhood.Services.Application.Disputes.DTOs;
using Neighborhood.Services.Application.Disputes.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Disputes;
using Neighborhood.Services.Application.Shared.Mappers;


namespace Neighborhood.Services.Application.Disputes.Handlers
{
    public class CreateDisputeCommandHandler: IRequestHandler<CreateDisputeCommand, DisputeDto>
    {
        private readonly IDisputeRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;
        public CreateDisputeCommandHandler(
            IDisputeRepository repository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<DisputeDto> Handle(
            CreateDisputeCommand request,
            CancellationToken cancellationToken)
        {
            var exists = await _repository.ExistsByBookingIdAsync(
                request.BookingId,
                cancellationToken);

            if (exists)
                throw new Exception("A dispute already exists for this booking.");

            var dispute = new Dispute
            {
                //BookingId = request.BookingId,
                BookingId = 1,
                //RaisedByUserId = _currentUser.UserId, // لما نشغل ال Auth نبقي نرجعها ونلغي اللي تحت
                RaisedByUserId = request.RaisedByUserId,
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
