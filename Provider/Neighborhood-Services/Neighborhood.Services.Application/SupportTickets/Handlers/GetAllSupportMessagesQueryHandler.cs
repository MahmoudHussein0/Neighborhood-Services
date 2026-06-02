using MediatR;
using Neighborhood.Services.Application.Shared.Mappers;
using Neighborhood.Services.Application.SupportTickets.DTOs;
using Neighborhood.Services.Application.SupportTickets.Interfaces;
using Neighborhood.Services.Application.SupportTickets.Queries;

namespace Neighborhood.Services.Application.SupportTickets.Handlers
{
    public class GetAllSupportMessagesQueryHandler : IRequestHandler<GetAllSupportMessagesQuery, IReadOnlyList<SupportMessageDto>>
    {
        private readonly ISupportMessageRepository _repository;

        public GetAllSupportMessagesQueryHandler(ISupportMessageRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<SupportMessageDto>> Handle(GetAllSupportMessagesQuery request, CancellationToken cancellationToken)
        {
            var messages = await _repository.GetByTicketIdAsync(request.TicketId, cancellationToken);
            return messages.Select(SupportMapper.MapMessageToDto).ToList();
        }
    }
}
