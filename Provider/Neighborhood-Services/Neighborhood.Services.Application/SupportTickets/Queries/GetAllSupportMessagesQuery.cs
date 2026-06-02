using MediatR;
using Neighborhood.Services.Application.SupportTickets.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.SupportTickets.Queries
{
    public class GetAllSupportMessagesQuery : IRequest<IReadOnlyList<SupportMessageDto>>
    {
        public int TicketId { get; set; }
        public GetAllSupportMessagesQuery(int ticketId) => TicketId = ticketId;
    }

}
