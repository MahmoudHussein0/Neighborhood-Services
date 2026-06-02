using MediatR;
using Neighborhood.Services.Application.ServiceRequests.DTOs;

namespace Neighborhood.Services.Application.ServiceRequests.Queries.GetMyServiceRequestsQuery
{
    // Returns the authenticated customer's own service requests.
    public class GetMyServiceRequestsQuery : IRequest<IEnumerable<ServiceRequestSummaryDto>>
    {
    }
}
