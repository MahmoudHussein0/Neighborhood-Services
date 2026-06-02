using MediatR;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Shared.Mappers;
using Neighborhood.Services.Application.SupportTickets.Commands;
using Neighborhood.Services.Application.SupportTickets.DTOs;
using Neighborhood.Services.Application.SupportTickets.Interfaces;
using Neighborhood.Services.Domain.SupportTickets;

namespace Neighborhood.Services.Application.SupportTickets.Handlers
{
    public class CreateSupportMessageCommandHandler : IRequestHandler<CreateSupportMessageCommand, SupportMessageDto>
    {
        private readonly ISupportMessageRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateSupportMessageCommandHandler(ISupportMessageRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<SupportMessageDto> Handle(CreateSupportMessageCommand request, CancellationToken cancellationToken)
        {
            var message = new SupportMessage
            {
                TicketId = request.TicketId,
                SenderId = request.SenderId,
                Message = request.Message,
                Channel = request.Channel,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _repository.AddAsync(message);
            await _unitOfWork.SaveChangesAsync();

            return SupportMapper.MapMessageToDto(message);
        }
    }

}
