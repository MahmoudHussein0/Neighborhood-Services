using MediatR;
using Neighborhood.Services.Application.ServiceRequests.DTOs;

namespace Neighborhood.Services.Application.ServiceRequests.Queries.GetServiceRequestByIdQuery
{
    public class GetServiceRequestByIdQuery : IRequest<ServiceRequestDetailsDto>
    {
        public int ServiceRequestId { get; set; }
    }
}
