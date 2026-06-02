using MediatR;
using Neighborhood.Services.Application.Disputes.DTOs;
using Neighborhood.Services.Application.Disputes.Interfaces;
using Neighborhood.Services.Application.Disputes.Queries;
using Neighborhood.Services.Application.Shared.Mappers;

namespace Neighborhood.Services.Application.Disputes.Handlers
{
    public class GetDisputeByIdQueryHandler : IRequestHandler<GetDisputeByIdQuery, DisputeDto>
    {
        private readonly IDisputeRepository _repository;

        public GetDisputeByIdQueryHandler(IDisputeRepository repository)
        {
            _repository = repository;
        }

        public async Task<DisputeDto> Handle(GetDisputeByIdQuery request, CancellationToken cancellationToken)
        {
            var dispute = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (dispute is null)
                throw new Exception($"Dispute with id {request.Id} not found.");

            return DisputeMapper.MapToDto(dispute);
        }
    }
}
