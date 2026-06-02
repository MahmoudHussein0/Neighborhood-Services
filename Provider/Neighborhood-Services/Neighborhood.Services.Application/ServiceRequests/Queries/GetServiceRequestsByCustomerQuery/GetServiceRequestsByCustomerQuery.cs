using MediatR;
using Neighborhood.Services.Application.ServiceRequests.DTOs;

namespace Neighborhood.Services.Application.ServiceRequests.Queries.GetServiceRequestsByCustomerQuery
{
    public class GetServiceRequestsByCustomerQuery : IRequest<IEnumerable<ServiceRequestSummaryDto>>
    {
        public int CustomerId { get; set; }
    }
}
