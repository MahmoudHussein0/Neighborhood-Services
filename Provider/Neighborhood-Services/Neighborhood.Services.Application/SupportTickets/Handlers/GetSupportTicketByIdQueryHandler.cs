using MediatR;
using Neighborhood.Services.Application.Shared.Mappers;
using Neighborhood.Services.Application.SupportTickets.DTOs;
using Neighborhood.Services.Application.SupportTickets.Interfaces;
using Neighborhood.Services.Application.SupportTickets.Queries;

namespace Neighborhood.Services.Application.SupportTickets.Handlers
{
    public class GetSupportTicketByIdQueryHandler : IRequestHandler<GetSupportTicketByIdQuery, SupportTicketDto>
    {
        private readonly ISupportTicketRepository _repository;

        public GetSupportTicketByIdQueryHandler(ISupportTicketRepository repository)
        {
            _repository = repository;
        }

        public async Task<SupportTicketDto> Handle(GetSupportTicketByIdQuery request, CancellationToken cancellationToken)
        {
            var ticket = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (ticket is null)
                throw new Exception($"SupportTicket with id {request.Id} not found.");

            return SupportMapper.MapTicketToDto(ticket);
        }
    }

}
