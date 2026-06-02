using MediatR;
using Neighborhood.Services.Application.Customers.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.ServiceRequests.DTOs;
using Neighborhood.Services.Application.ServiceRequests.Interfaces;
using Neighborhood.Services.Application.Shared;

namespace Neighborhood.Services.Application.ServiceRequests.Queries.GetMyServiceRequestsQuery
{
    public class GetMyServiceRequestsQueryHandler : IRequestHandler<GetMyServiceRequestsQuery, IEnumerable<ServiceRequestSummaryDto>>
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetMyServiceRequestsQueryHandler(
            IServiceRequestRepository serviceRequestRepository,
            ICustomerRepository customerRepository,
            ICurrentUserService currentUserService)
        {
            _serviceRequestRepository = serviceRequestRepository;
            _customerRepository = customerRepository;
            _currentUserService = currentUserService;
        }

        public async Task<IEnumerable<ServiceRequestSummaryDto>> Handle(GetMyServiceRequestsQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("User is not authenticated.");

            var customer = await _customerRepository.GetByUserIdAsync(userId)
                ?? throw new ForbiddenException("Only customers can view service requests.");

            var requests = await _serviceRequestRepository.GetCustomerServiceRequestsAsync(customer.Id);

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
                CustomerId = sr.CustomerId,
                Latitude = sr.Location.Y,
                Longitude = sr.Location.X,
                CreatedAt = sr.CreatedAt,
                ExpiresAt = sr.ExpiresAt,
                OfferCount = sr.Offers?.Count ?? 0
            });
        }
    }
}
