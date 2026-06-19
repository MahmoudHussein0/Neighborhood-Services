using MediatR;
using Neighborhood.Services.Application.SupportTickets.DTOs;

namespace Neighborhood.Services.Application.SupportTickets.Queries
{
    public class GetSupportMessageByIdQuery : IRequest<SupportMessageDto>
    {
        public int Id { get; set; }
        public GetSupportMessageByIdQuery(int id) => Id = id;
    }
}
