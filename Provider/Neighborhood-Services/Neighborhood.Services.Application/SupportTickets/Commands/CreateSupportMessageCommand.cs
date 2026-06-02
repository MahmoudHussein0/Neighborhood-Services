using MediatR;
using Neighborhood.Services.Application.SupportTickets.DTOs;
using Neighborhood.Services.Domain.SupportTickets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.SupportTickets.Commands
{
    public class CreateSupportMessageCommand : IRequest<SupportMessageDto>
    {
        public int TicketId { get; set; }
        public int SenderId { get; set; }
        public string Message { get; set; }
        public MessageChannel Channel { get; set; }
    }
}
