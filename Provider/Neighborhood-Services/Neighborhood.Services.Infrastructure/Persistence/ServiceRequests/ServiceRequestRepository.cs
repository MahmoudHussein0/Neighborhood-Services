using Neighborhood.Services.Application.ServiceRequests.Interfaces;
using Neighborhood.Services.Domain.ServiceRequests;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Neighborhood.Services.Infrastructure.Persistence.ServiceRequests
{
    public class ServiceRequestRepository : GenericRepository<ServiceRequest, int>, IServiceRequestRepository
    {
        public ServiceRequestRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<ServiceRequest?> GetServiceRequestWithDetailsAsync(int serviceRequestId)
        {
            return await _context.ServiceRequests
                .Include(sr => sr.Customer)
                .Include(sr => sr.Category)
                .Include(sr => sr.ProblemType)
                .Include(sr => sr.Offers)
                .FirstOrDefaultAsync(sr => sr.Id == serviceRequestId && !sr.IsDeleted);
        }

        public async Task<IEnumerable<ServiceRequest>> GetCustomerServiceRequestsAsync(int customerId)
        {
            return await _context.ServiceRequests
                .Where(sr => sr.CustomerId == customerId && !sr.IsDeleted)
                .ToListAsync();
        }
        public async Task<IEnumerable<ServiceRequest>> GetOpenServiceRequestsAsync(double latitude, double longitude, double radiusInMeters)
        {
            var location = new Point(longitude, latitude) { SRID = 4326 };
            return await _context.ServiceRequests
                .Where(sr => sr.Status == ServiceRequestStatus.Open
                    && !sr.IsDeleted
                    && sr.Location.IsWithinDistance(location, radiusInMeters))
                .ToListAsync();
        }
        public async Task<ServiceRequest?> GetServiceRequestWithOffersAsync(int serviceRequestId)
        {
            return await _context.ServiceRequests
                .Include(sr => sr.Offers)
                    .ThenInclude(o => o.Technician)
                .FirstOrDefaultAsync(sr => sr.Id == serviceRequestId && !sr.IsDeleted);
        }
    }
}
