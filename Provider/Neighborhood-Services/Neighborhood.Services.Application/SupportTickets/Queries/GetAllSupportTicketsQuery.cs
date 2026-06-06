using MediatR;
using Neighborhood.Services.Application.SupportTickets.DTOs;

namespace Neighborhood.Services.Application.SupportTickets.Queries
{
    public class GetAllSupportTicketsQuery : IRequest<IReadOnlyList<SupportTicketDto>>
    {
    }
}
