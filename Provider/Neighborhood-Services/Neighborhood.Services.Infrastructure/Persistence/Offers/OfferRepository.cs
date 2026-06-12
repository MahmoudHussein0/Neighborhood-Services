using Neighborhood.Services.Application.Offers.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Offers;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.Offers
{
    public class OfferRepository : GenericRepository<Offer, int>, IOfferRepository
    {
        public OfferRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Offer>> GetOffersByServiceRequestAsync(int serviceRequestId)
        {
            return await _context.Offers
                .Where(o => o.ServiceRequestId == serviceRequestId && !o.IsDeleted)
                .Include(o => o.Technician)
                .ToListAsync();
        }

        public async Task<IEnumerable<Offer>> GetTechnicianOffersAsync(int technicianId)
        {
            return await _context.Offers
                .Where(o => o.TechnicianId == technicianId && !o.IsDeleted)
                .ToListAsync();
        }

        public async Task<PagedResult<Offer>> GetTechnicianOffersPagedAsync(int technicianId, OfferStatus? status, int page, int pageSize)
        {
            var query = _context.Offers
                .Where(o => o.TechnicianId == technicianId && !o.IsDeleted);

            if (status.HasValue)
                query = query.Where(o => o.Status == status.Value);

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Offer>(items, total, page, pageSize);
        }


        public async Task<Offer?> GetAcceptedOfferAsync(int serviceRequestId)
        {
            return await _context.Offers
                .FirstOrDefaultAsync(o => o.ServiceRequestId == serviceRequestId
                    && o.Status == OfferStatus.Accepted);
        }
        public async Task<Offer?> GetOfferWithDetailsAsync(int offerId)
        {
            return await _context.Offers
                .Include(o => o.ServiceRequest)
                .Include(o => o.Technician)
                .FirstOrDefaultAsync(o => o.Id == offerId && !o.IsDeleted);
        }
    }
}
