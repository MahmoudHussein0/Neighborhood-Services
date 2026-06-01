using MediatR;
using Neighborhood.Services.Application.ServiceRequests.DTOs;
using Neighborhood.Services.Application.ServiceRequests.Interfaces;

namespace Neighborhood.Services.Application.ServiceRequests.Queries.GetServiceRequestsByCustomerQuery
{
    public class GetServiceRequestsByCustomerQueryHandler : IRequestHandler<GetServiceRequestsByCustomerQuery, IEnumerable<ServiceRequestSummaryDto>>
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;

        public GetServiceRequestsByCustomerQueryHandler(IServiceRequestRepository serviceRequestRepository)
        {
            _serviceRequestRepository = serviceRequestRepository;
        }

        public async Task<IEnumerable<ServiceRequestSummaryDto>> Handle(GetServiceRequestsByCustomerQuery request, CancellationToken cancellationToken)
        {
            var requests = await _serviceRequestRepository.GetCustomerServiceRequestsAsync(request.CustomerId);

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
                 CustomerId = sr.CustomerId,
                 Latitude= sr.Location.Y,
                 Longitude=sr.Location.X,
                  OfferCount=sr.Offers?.Count??0
            });
        }
    }
}
