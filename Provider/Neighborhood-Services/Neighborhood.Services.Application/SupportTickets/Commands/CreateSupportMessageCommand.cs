using MediatR;
using Neighborhood.Services.Application.SupportTickets.DTOs;
using Neighborhood.Services.Domain.SupportTickets;

namespace Neighborhood.Services.Application.SupportTickets.Commands
{
    public class CreateSupportMessageCommand : IRequest<SupportMessageDto>
    {
        public int TicketId { get; set; }

        public string? Message { get; set; }

        public MessageChannel Channel { get; set; }

        public List<CreateAttachmentDto>? Attachments { get; set; }
    }
}
