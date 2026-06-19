using MediatR;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Shared.Mappers;
using Neighborhood.Services.Application.Staffs.Interfaces;
using Neighborhood.Services.Application.SupportTickets.Commands;
using Neighborhood.Services.Application.SupportTickets.DTOs;
using Neighborhood.Services.Application.SupportTickets.Interfaces;
using Neighborhood.Services.Domain.SupportTickets;

namespace Neighborhood.Services.Application.SupportTickets.Handlers
{
    public class CreateSupportMessageCommandHandler
        : IRequestHandler<CreateSupportMessageCommand, SupportMessageDto>
    {
        private readonly ISupportMessageRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;
        private readonly ISupportTicketRepository _ticketRepository;
        private readonly IStaffRepository _staffRepository;

        public CreateSupportMessageCommandHandler(
            ISupportMessageRepository repository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser,
            ISupportTicketRepository ticketRepository,
            IStaffRepository staffRepository)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _ticketRepository = ticketRepository;
            _staffRepository = staffRepository;
        }

        public async Task<SupportMessageDto> Handle(
            CreateSupportMessageCommand request,
            CancellationToken cancellationToken)
        {
            var ticket = await _ticketRepository.GetByIdAsync(
                request.TicketId);

            if (ticket is null)
            {
                throw new NotFoundException(
                    $"SupportTicket with id {request.TicketId} not found.");
            }

            if (ticket.Status == SupportTicketStatus.Resolved)
            {
                throw new BadRequestException(
                    "Cannot send messages to a resolved ticket.");
            }

            if (string.IsNullOrWhiteSpace(request.Message)
                && (request.Attachments == null
                    || !request.Attachments.Any()))
            {
                throw new BadRequestException(
                    "Message or attachment is required.");
            }

            var senderId = _currentUser.UserId!;

            var staff = await _staffRepository
                .GetByUserIdAsync(senderId);

            var senderType =
                staff is null
                    ? MessageSenderType.user
                    : MessageSenderType.Staff;

            var message = new SupportMessage
            {
                TicketId = request.TicketId,

                SenderId = senderId,

                SenderType = senderType,

                Message = request.Message,

                Channel = request.Channel,

                CreatedAt = DateTime.UtcNow,

                Attachments = request.Attachments?
                    .Select(a => new MessageAttachment
                    {
                        Url = a.Url,
                        PublicId = a.PublicId,
                        Type = a.Type,
                        CreatedAt = DateTime.UtcNow
                    })
                    .ToList()
                    ?? new List<MessageAttachment>(),

                IsDeleted = false
            };

            await _repository.AddAsync(message);

            ticket.UpdatedAt = DateTime.UtcNow;

            var isCustomer =
                senderType == MessageSenderType.user;

            if (ticket.Status == SupportTicketStatus.WaitingOnCustomer
                && isCustomer)
            {
                ticket.Status = SupportTicketStatus.InProgress;
            }

            await _ticketRepository.UpdateAsync(ticket);

            await _unitOfWork.SaveChangesAsync();

            return SupportMapper.MapMessageToDto(message);
        }
    }
}