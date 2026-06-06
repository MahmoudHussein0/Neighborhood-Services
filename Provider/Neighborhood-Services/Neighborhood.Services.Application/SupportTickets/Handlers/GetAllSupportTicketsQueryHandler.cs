using MediatR;
using Neighborhood.Services.Application.Shared.Mappers;
using Neighborhood.Services.Application.SupportTickets.DTOs;
using Neighborhood.Services.Application.SupportTickets.Interfaces;
using Neighborhood.Services.Application.SupportTickets.Queries;

namespace Neighborhood.Services.Application.SupportTickets.Handlers
{
    public class GetAllSupportTicketsQueryHandler : IRequestHandler<GetAllSupportTicketsQuery, IReadOnlyList<SupportTicketDto>>
    {
        private readonly ISupportTicketRepository _repository;

        public GetAllSupportTicketsQueryHandler(ISupportTicketRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<SupportTicketDto>> Handle(GetAllSupportTicketsQuery request, CancellationToken cancellationToken)
        {
            var tickets = await _repository.GetAllAsync(cancellationToken);
            return tickets.Select(SupportMapper.MapTicketToDto).ToList();
        }
    }
}
