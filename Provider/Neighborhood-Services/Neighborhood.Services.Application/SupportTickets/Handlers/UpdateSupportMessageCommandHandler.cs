using MediatR;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Shared.Mappers;
using Neighborhood.Services.Application.SupportTickets.Commands;
using Neighborhood.Services.Application.SupportTickets.DTOs;
using Neighborhood.Services.Application.SupportTickets.Interfaces;

namespace Neighborhood.Services.Application.SupportTickets.Handlers
{
    public class UpdateSupportMessageCommandHandler : IRequestHandler<UpdateSupportMessageCommand, SupportMessageDto>
    {
        private readonly ISupportMessageRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateSupportMessageCommandHandler(ISupportMessageRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<SupportMessageDto> Handle(UpdateSupportMessageCommand request, CancellationToken cancellationToken)
        {
            var message = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (message is null)
                throw new Exception($"SupportMessage with id {request.Id} not found.");

            message.ReadAt = request.ReadAt;

            await _repository.UpdateAsync(message);
            await _unitOfWork.SaveChangesAsync();

            return SupportMapper.MapMessageToDto(message);
        }
    }
}
