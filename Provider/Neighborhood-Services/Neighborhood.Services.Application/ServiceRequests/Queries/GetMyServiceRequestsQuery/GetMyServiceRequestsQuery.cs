using MediatR;
using Neighborhood.Services.Application.ServiceRequests.DTOs;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.ServiceRequests;

namespace Neighborhood.Services.Application.ServiceRequests.Queries.GetMyServiceRequestsQuery
{
    // Returns the authenticated customer's own service requests (paged + optional filter/search).
    public class GetMyServiceRequestsQuery : IRequest<PagedResult<ServiceRequestSummaryDto>>
    {
        public ServiceRequestStatus? Status { get; set; }   // optional filter
        public string? Search { get; set; }                 // matches description, address, or exact id
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
