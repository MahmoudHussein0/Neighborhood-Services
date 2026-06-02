using MediatR;
using Neighborhood.Services.Application.SupportTickets.DTOs;
using Neighborhood.Services.Domain.SupportTickets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.SupportTickets.Commands
{
    public class UpdateSupportTicketCommand : IRequest<SupportTicketDto>
    {
        public int Id { get; set; }
        public SupportTicketStatus Status { get; set; }
    }
}
