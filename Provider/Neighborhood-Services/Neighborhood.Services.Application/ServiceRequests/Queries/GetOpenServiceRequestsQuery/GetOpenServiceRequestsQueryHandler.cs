using MediatR;
using Neighborhood.Services.Application.Customers.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.ServiceRequests.DTOs;
using Neighborhood.Services.Application.ServiceRequests.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Technicians.Interfaces;

namespace Neighborhood.Services.Application.ServiceRequests.Queries.GetOpenServiceRequestsQuery
{
    public class GetOpenServiceRequestsQueryHandler : IRequestHandler<GetOpenServiceRequestsQuery, PagedResult<ServiceRequestSummaryDto>>
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ITechnicianRepository _technicianRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetOpenServiceRequestsQueryHandler(
            IServiceRequestRepository serviceRequestRepository,
            ICustomerRepository customerRepository,
            ITechnicianRepository technicianRepository,
            ICurrentUserService currentUserService)
        {
            _serviceRequestRepository = serviceRequestRepository;
            _customerRepository = customerRepository;
            _technicianRepository = technicianRepository;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResult<ServiceRequestSummaryDto>> Handle(GetOpenServiceRequestsQuery request, CancellationToken cancellationToken)
        {
            // Normalize paging (guard against bad input)
            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize is < 1 or > 100 ? 10 : request.PageSize;

            // Resolve the viewing technician + their categories. These gate what they can
            // offer on (CanOffer) and, in "my categories" mode, what the feed is filtered to.
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("User is not authenticated.");
            var technician = await _technicianRepository.GetByUserIdAsync(userId)
                ?? throw new ForbiddenException("Only technicians can browse open requests.");
            var myCategoryIds = await _technicianRepository.GetCategoryIdsAsync(technician.Id);
            var myCategorySet = myCategoryIds.ToHashSet();

            // "My categories" view filters to the technician's categories; whole-market view doesn't.
            var categoryFilter = request.OnlyMyCategories ? myCategoryIds : null;

            var paged = await _serviceRequestRepository.GetOpenServiceRequestsAsync(
                request.Latitude,
                request.Longitude,
                request.RadiusInMeters,
                categoryFilter,
                page,
                pageSize);

            // Resolve customer ids -> display name so cards can show who posted the request.
            var customerNames = await _customerRepository.GetNamesByIdsAsync(
                paged.Items.Select(sr => sr.CustomerId).Distinct().ToList());

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
                    CreatedAt = sr.CreatedAt,
                    ExpiresAt = sr.ExpiresAt,
                    Latitude = sr.Location.Y,
                    Longitude = sr.Location.X,
                    CustomerId = sr.CustomerId,
                    CustomerName = customerNames.TryGetValue(sr.CustomerId, out var name) ? name : string.Empty,
                    OfferCount = sr.Offers?.Count ?? 0,
                    CanOffer = myCategorySet.Contains(sr.CategoryId)
                }).ToList(),
                paged.TotalCount,
                paged.Page,
                paged.PageSize);
        }
    }
}
