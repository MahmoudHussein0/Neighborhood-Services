using Neighborhood.Services.Application.SupportTickets.DTOs;
using Neighborhood.Services.Domain.SupportTickets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Shared.Mappers
{
    public static class SupportMapper
    {
        public static SupportTicketDto MapTicketToDto(SupportTicket ticket) => new()
        {
            Id = ticket.Id,
            UserId = ticket.UserId,
            BookingId = ticket.BookingId,
            Subject = ticket.Subject,
            Status = ticket.Status.ToString(),
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
