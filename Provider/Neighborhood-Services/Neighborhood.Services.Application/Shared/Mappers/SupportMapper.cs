using Neighborhood.Services.Application.SupportTickets.DTOs;
using Neighborhood.Services.Domain.SupportTickets;

namespace Neighborhood.Services.Application.Shared.Mappers
{
    public static class SupportMapper
    {
        public static SupportTicketDto MapTicketToDto(SupportTicket ticket) => new()
        {
            Id = ticket.Id,
            UserId = ticket.UserId,
            Subject = ticket.Subject,
            Description = ticket.Description,
            Status = ticket.Status.ToString(),
            Priority = ticket.Priority.ToString(),
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt
        };

        public static SupportMessageDto MapMessageToDto(SupportMessage message) => new()
        {
            Id = message.Id,
            TicketId = message.TicketId,
            SenderId = message.SenderId,
            Message = message.Message,
            Channel = message.Channel.ToString(),
            ReadAt = message.ReadAt,
            CreatedAt = message.CreatedAt
        };
    }
}
