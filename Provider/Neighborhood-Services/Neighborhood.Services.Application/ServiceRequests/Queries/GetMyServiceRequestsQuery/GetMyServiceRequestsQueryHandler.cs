using MediatR;
using Neighborhood.Services.Application.Customers.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.ServiceRequests.DTOs;
using Neighborhood.Services.Application.ServiceRequests.Interfaces;
using Neighborhood.Services.Application.Shared;

namespace Neighborhood.Services.Application.ServiceRequests.Queries.GetMyServiceRequestsQuery
{
    public class GetMyServiceRequestsQueryHandler : IRequestHandler<GetMyServiceRequestsQuery, PagedResult<ServiceRequestSummaryDto>>
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

        public async Task<PagedResult<ServiceRequestSummaryDto>> Handle(GetMyServiceRequestsQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("User is not authenticated.");

            var customer = await _customerRepository.GetByUserIdAsync(userId)
                ?? throw new ForbiddenException("Only customers can view service requests.");

            // Normalize paging (guard against bad input)
            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize is < 1 or > 100 ? 10 : request.PageSize;

            var paged = await _serviceRequestRepository.GetCustomerServiceRequestsPagedAsync(
                customer.Id, request.Status, request.Search, page, pageSize);

            return new PagedResult<ServiceRequestSummaryDto>(
                paged.Items.Select(sr => new ServiceRequestSummaryDto
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
                }).ToList(),
                paged.TotalCount,
                paged.Page,
                paged.PageSize);
        }
    }
}
