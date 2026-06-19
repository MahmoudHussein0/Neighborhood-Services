using MediatR;
using Neighborhood.Services.Application.Shared.Mappers;
using Neighborhood.Services.Application.SupportTickets.DTOs;
using Neighborhood.Services.Application.SupportTickets.Interfaces;
using Neighborhood.Services.Application.SupportTickets.Queries;

namespace Neighborhood.Services.Application.SupportTickets.Handlers
{
    public class GetSupportTicketDetailsQueryHandler
       : IRequestHandler<GetSupportTicketDetailsQuery, SupportTicketDetailsDto>
    {
        private readonly ISupportTicketRepository _repository;

        public GetSupportTicketDetailsQueryHandler(
            ISupportTicketRepository repository)
        {
            _repository = repository;
        }

        public async Task<SupportTicketDetailsDto> Handle(
            GetSupportTicketDetailsQuery request,
            CancellationToken cancellationToken)
        {
            var ticket = await _repository.GetByIdWithMessagesAsync(
                request.Id,
                cancellationToken);

            if (ticket is null)
            {
                throw new Exception(
                    $"SupportTicket with id {request.Id} not found.");
            }

            return new SupportTicketDetailsDto
            {
                Id = ticket.Id,
                UserId = ticket.UserId,
                BookingId = ticket.BookingId,
                Subject = ticket.Subject,
                Description = ticket.Description,
                SenderName = ticket.SenderName,
                SenderEmail = ticket.SenderEmail,
                Priority = ticket.Priority.ToString(),  
                Status = ticket.Status.ToString(),
                CreatedAt = ticket.CreatedAt,
                UpdatedAt = ticket.UpdatedAt,

                Messages = ticket.Messages
                    .OrderBy(m => m.CreatedAt)
                    .ThenBy(m => m.Id)
                    .Select(SupportMapper.MapMessageToDto)
                    .ToList()
            };
        }
    }
}
