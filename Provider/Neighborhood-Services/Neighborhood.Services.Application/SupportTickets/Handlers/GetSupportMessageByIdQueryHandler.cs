using MediatR;
using Neighborhood.Services.Application.Shared.Mappers;
using Neighborhood.Services.Application.SupportTickets.DTOs;
using Neighborhood.Services.Application.SupportTickets.Interfaces;
using Neighborhood.Services.Application.SupportTickets.Queries;

namespace Neighborhood.Services.Application.SupportTickets.Handlers
{
    public class GetSupportMessageByIdQueryHandler : IRequestHandler<GetSupportMessageByIdQuery, SupportMessageDto>
    {
        private readonly ISupportMessageRepository _repository;

        public GetSupportMessageByIdQueryHandler(ISupportMessageRepository repository)
        {
            _repository = repository;
        }

        public async Task<SupportMessageDto> Handle(GetSupportMessageByIdQuery request, CancellationToken cancellationToken)
        {
            var message = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (message is null)
                throw new Exception($"SupportMessage with id {request.Id} not found.");

            return SupportMapper.MapMessageToDto(message);
        }
    }

}
