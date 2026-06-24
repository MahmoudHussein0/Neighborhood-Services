using MediatR;
using Neighborhood.Services.Application.ServiceRequests.DTOs;
using Neighborhood.Services.Application.Shared;

namespace Neighborhood.Services.Application.ServiceRequests.Queries.GetOpenServiceRequestsQuery
{
    public class GetOpenServiceRequestsQuery : IRequest<PagedResult<ServiceRequestSummaryDto>>
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double RadiusInMeters { get; set; }   // <= 0 means "All" (no distance filter)
        // true  = only requests in the technician's own categories (default).
        // false = whole market (all categories); they still can't offer outside their categories.
        public bool OnlyMyCategories { get; set; } = true;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
