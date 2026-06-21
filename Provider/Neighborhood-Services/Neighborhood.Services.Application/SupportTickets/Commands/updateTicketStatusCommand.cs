using MediatR;
using Neighborhood.Services.Application.SupportTickets.DTOs;
using Neighborhood.Services.Domain.SupportTickets;

namespace Neighborhood.Services.Application.SupportTickets.Commands
{
    public class updateTicketStatusCommand : IRequest<SupportTicketDto>
    {
        public int Id { get; set; }
        public SupportTicketStatus Status { get; set; }
    }
}
