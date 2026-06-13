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

        // Paged + optional status filter + search (used by "my service requests")
        Task<PagedResult<ServiceRequest>> GetCustomerServiceRequestsPagedAsync(int customerId, ServiceRequestStatus? status, string? search, int page, int pageSize);
        Task<IEnumerable<ServiceRequest>> GetOpenServiceRequestsAsync(double latitude, double longitude, double radiusInMeters);
        Task<ServiceRequest?> GetServiceRequestWithOffersAsync(int serviceRequestId);

        // Resolves technician id → full name (joins ApplicationUser), so offer cards can show who made them.
        Task<Dictionary<int, string>> GetTechnicianNamesAsync(IEnumerable<int> technicianIds);

        // Paged list of requests in a given status — used by the staff moderation queue (Flagged).
        Task<PagedResult<ServiceRequest>> GetByStatusPagedAsync(ServiceRequestStatus status, int page, int pageSize);
    }
}
