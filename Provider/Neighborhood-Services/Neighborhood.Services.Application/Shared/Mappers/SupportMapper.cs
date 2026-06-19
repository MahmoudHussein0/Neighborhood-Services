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
            BookingId = ticket.BookingId,
            SenderName = ticket.SenderName,
            SenderEmail = ticket.SenderEmail,
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

            SenderType = message.SenderType.ToString(),

            Message = message.Message,

            Channel = message.Channel.ToString(),

            ReadAt = message.ReadAt,

            CreatedAt = message.CreatedAt,

            Attachments = message.Attachments?
         .Select(a => new AttachmentDto
         {
             Id = a.Id,
             Url = a.Url,
             PublicId = a.PublicId,
             Type = a.Type.ToString()
         })
         .ToList() ?? new()
        };
    }
}
