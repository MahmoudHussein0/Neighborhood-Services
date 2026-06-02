using MediatR;
using Neighborhood.Services.Application.Disputes.DTOs;
using Neighborhood.Services.Application.Disputes.Interfaces;
using Neighborhood.Services.Application.Disputes.Queries;
using Neighborhood.Services.Application.Shared.Mappers;

namespace Neighborhood.Services.Application.Disputes.Handlers
{
    public class GetAllDisputesQueryHandler : IRequestHandler<GetAllDisputesQuery, IReadOnlyList<DisputeDto>>
    {
        private readonly IDisputeRepository _repository;

        public GetAllDisputesQueryHandler(IDisputeRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<DisputeDto>> Handle(GetAllDisputesQuery request, CancellationToken cancellationToken)
        {
            var disputes = await _repository.GetAllAsync(cancellationToken);
            return disputes.Select(DisputeMapper.MapToDto).ToList();
        }
    }

}
