using MediatR;
using Neighborhood.Services.Application.ServiceRequests.DTOs;
using Neighborhood.Services.Application.ServiceRequests.Interfaces;

namespace Neighborhood.Services.Application.ServiceRequests.Queries.GetOpenServiceRequestsQuery
{
    public class GetOpenServiceRequestsQueryHandler : IRequestHandler<GetOpenServiceRequestsQuery, IEnumerable<ServiceRequestSummaryDto>>
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;

        public GetOpenServiceRequestsQueryHandler(IServiceRequestRepository serviceRequestRepository)
        {
            _serviceRequestRepository = serviceRequestRepository;
        }

        public async Task<IEnumerable<ServiceRequestSummaryDto>> Handle(GetOpenServiceRequestsQuery request, CancellationToken cancellationToken)
        {
            var requests = await _serviceRequestRepository.GetOpenServiceRequestsAsync(
                request.Latitude,
                request.Longitude,
                request.RadiusInMeters);
            return requests.Select(sr => new ServiceRequestSummaryDto
            {
                Id = sr.Id,
                Description = sr.Description,
                Address = sr.Address,
                Budget = sr.Budget,
                Status = sr.Status,
                ScheduledAt = sr.ScheduledAt,
                CategoryId = sr.CategoryId,
                ProblemTypeId = sr.ProblemTypeId,
                CreatedAt = sr.CreatedAt,
                ExpiresAt = sr.ExpiresAt,
                Latitude = sr.Location.Y,
                Longitude = sr.Location.X,
                CustomerId = sr.CustomerId,
                OfferCount = sr.Offers?.Count ?? 0
            });
        }
    }
}
