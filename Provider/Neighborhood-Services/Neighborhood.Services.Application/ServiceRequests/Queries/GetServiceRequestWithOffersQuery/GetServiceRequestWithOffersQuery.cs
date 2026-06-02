using MediatR;
using Neighborhood.Services.Application.ServiceRequests.DTOs;

namespace Neighborhood.Services.Application.ServiceRequests.Queries.GetServiceRequestWithOffersQuery
{
    public class GetServiceRequestWithOffersQuery : IRequest<ServiceRequestWithOffersDto>
    {
        public int ServiceRequestId { get; set; }
    }
}
