using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.ServiceRequests;
using System;
using System.Collections.Generic;
using System.Text;
using Point = NetTopologySuite.Geometries.Point;

namespace Neighborhood.Services.Application.ServiceRequests.Interfaces
{
    public interface IServiceRequestRepository : IGenericRepository<ServiceRequest, int>
    {
        Task<ServiceRequest?> GetServiceRequestWithDetailsAsync(int serviceRequestId);
        Task<IEnumerable<ServiceRequest>> GetCustomerServiceRequestsAsync(int customerId);
        Task<IEnumerable<ServiceRequest>> GetOpenServiceRequestsAsync(Point location, double radiusInMeters);

        Task<ServiceRequest?> GetServiceRequestWithOffersAsync(int serviceRequestId);
    }
}
