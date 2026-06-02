using MediatR;
using Neighborhood.Services.Application.SupportTickets.DTOs;
using Neighborhood.Services.Domain.SupportTickets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.SupportTickets.Commands
{
    public class CreateSupportTicketCommand : IRequest<SupportTicketDto>
    {
        
        
        public int? BookingId { get; set; }
        public string Subject { get; set; }
    }

    

    
}
