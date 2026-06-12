using MediatR;
using Neighborhood.Services.Application.SupportTickets.DTOs;
using Neighborhood.Services.Domain.SupportTickets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.SupportTickets.Commands
{
    public class updateTicketPriorityCommand : IRequest<SupportTicketDto>
    {
        public int Id { get; set; }
        public SupportTicketPriority Priority { get; set; }
    }

}
