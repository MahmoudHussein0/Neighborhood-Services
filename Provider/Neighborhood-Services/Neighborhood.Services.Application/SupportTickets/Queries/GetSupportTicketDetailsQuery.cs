using MediatR;
using Neighborhood.Services.Application.SupportTickets.DTOs;

namespace Neighborhood.Services.Application.SupportTickets.Queries
{
    public class GetSupportTicketDetailsQuery
        : IRequest<SupportTicketDetailsDto>
    {
        public int Id { get; set; }
    }
}
