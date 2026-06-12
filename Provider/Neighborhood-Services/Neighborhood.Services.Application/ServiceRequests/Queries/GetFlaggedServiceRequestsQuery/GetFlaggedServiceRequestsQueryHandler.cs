using MediatR;
using Neighborhood.Services.Application.ServiceRequests.DTOs;
using Neighborhood.Services.Application.ServiceRequests.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.ServiceRequests;

namespace Neighborhood.Services.Application.ServiceRequests.Queries.GetFlaggedServiceRequestsQuery
{
    public class GetFlaggedServiceRequestsQueryHandler
        : IRequestHandler<GetFlaggedServiceRequestsQuery, PagedResult<FlaggedServiceRequestDto>>
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;

        public GetFlaggedServiceRequestsQueryHandler(IServiceRequestRepository serviceRequestRepository)
        {
            _serviceRequestRepository = serviceRequestRepository;
        }

        public async Task<PagedResult<FlaggedServiceRequestDto>> Handle(GetFlaggedServiceRequestsQuery request, CancellationToken cancellationToken)
        {
            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize is < 1 or > 100 ? 10 : request.PageSize;

            var paged = await _serviceRequestRepository.GetByStatusPagedAsync(
                ServiceRequestStatus.Flagged, page, pageSize);

            return new PagedResult<FlaggedServiceRequestDto>(
                paged.Items.Select(sr => new FlaggedServiceRequestDto
                {
                    Id = sr.Id,
                    Description = sr.Description,
                    Address = sr.Address,
                    Image = sr.Image,
                    Budget = sr.Budget,
                    CustomerId = sr.CustomerId,
                    ScheduledAt = sr.ScheduledAt,
                    CreatedAt = sr.CreatedAt
                }).ToList(),
                paged.TotalCount,
                paged.Page,
                paged.PageSize);
        }
    }
}
