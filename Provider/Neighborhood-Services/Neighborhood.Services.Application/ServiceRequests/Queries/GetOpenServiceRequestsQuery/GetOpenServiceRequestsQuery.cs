using MediatR;
using Neighborhood.Services.Application.ServiceRequests.DTOs;

namespace Neighborhood.Services.Application.ServiceRequests.Queries.GetOpenServiceRequestsQuery
{
    public class GetOpenServiceRequestsQuery : IRequest<IEnumerable<ServiceRequestSummaryDto>>
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double RadiusInMeters { get; set; }
    }
}
