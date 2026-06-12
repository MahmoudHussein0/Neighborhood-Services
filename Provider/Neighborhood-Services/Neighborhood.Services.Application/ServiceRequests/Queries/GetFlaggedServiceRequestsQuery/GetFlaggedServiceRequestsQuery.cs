using MediatR;
using Neighborhood.Services.Application.ServiceRequests.DTOs;
using Neighborhood.Services.Application.Shared;

namespace Neighborhood.Services.Application.ServiceRequests.Queries.GetFlaggedServiceRequestsQuery
{
    // Staff-only: the moderation queue — requests the agent marked Flagged, paged.
    public class GetFlaggedServiceRequestsQuery : IRequest<PagedResult<FlaggedServiceRequestDto>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
