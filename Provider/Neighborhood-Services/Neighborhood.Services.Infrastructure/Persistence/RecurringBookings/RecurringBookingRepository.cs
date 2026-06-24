using Neighborhood.Services.Application.RecurringBookings.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.RecurringBookings;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.RecurringBookings
{
    public class RecurringBookingRepository : GenericRepository<RecurringBooking,int>,IRecurringBookingRepository
    {
        public RecurringBookingRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<RecurringBooking>> GetCustomerRecurringBookingsAsync(int customerId)
        {
            return await _context.RecurringBookings
                .Where(rb => rb.CustomerId == customerId && !rb.IsDeleted)
                .ToListAsync();
        }
        public async Task<IEnumerable<RecurringBooking>> GetActiveRecurringBookingsAsync()
        {
            return await _context.RecurringBookings
                .Where(rb => rb.Status == RecurringBookingStatus.Active && !rb.IsDeleted)
                .ToListAsync();
        }
        public async Task<RecurringBooking?> GetRecurringBookingWithDetailsAsync(int recurringBookingId)
        {
            return await _context.RecurringBookings
                .Include(rb => rb.Customer)
                .Include(rb => rb.Technician)
                .Include(rb => rb.ProblemType)
                .Include(rb => rb.Bookings)
                .FirstOrDefaultAsync(rb => rb.Id == recurringBookingId && !rb.IsDeleted);
        }

        public async Task<IEnumerable<RecurringBooking>> GetTechnicianRecurringBookingsAsync(int technicianId)
        {
            return await _context.RecurringBookings
                .Where(rb => rb.TechnicianId == technicianId && !rb.IsDeleted)
                .ToListAsync();
        }

        public async Task<PagedResult<RecurringBooking>> GetCustomerRecurringBookingsPagedAsync(int customerId, RecurringBookingStatus? status, string? search, int page, int pageSize)
        {
            var query = _context.RecurringBookings
                .Where(rb => rb.CustomerId == customerId && !rb.IsDeleted);

            query = ApplyFilters(query, status, search);

            var total = await query.CountAsync();
            var items = await query
                .Include(rb => rb.ProblemType) // so the DTO can show the problem-type name, not just its id
                .OrderByDescending(rb => rb.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<RecurringBooking>(items, total, page, pageSize);
        }

        public async Task<PagedResult<RecurringBooking>> GetTechnicianRecurringBookingsPagedAsync(int technicianId, RecurringBookingStatus? status, string? search, int page, int pageSize)
        {
            var query = _context.RecurringBookings
                .Where(rb => rb.TechnicianId == technicianId && !rb.IsDeleted);

            query = ApplyFilters(query, status, search);

            var total = await query.CountAsync();
            var items = await query
                .Include(rb => rb.ProblemType) // so the DTO can show the problem-type name, not just its id
                .OrderByDescending(rb => rb.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<RecurringBooking>(items, total, page, pageSize);
        }

        private static IQueryable<RecurringBooking> ApplyFilters(IQueryable<RecurringBooking> query, RecurringBookingStatus? status, string? search)
        {
            if (status.HasValue)
                query = query.Where(rb => rb.Status == status.Value);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                int? idTerm = int.TryParse(term, out var parsed) ? parsed : null;
                query = query.Where(rb =>
                    rb.Address.Contains(term) ||
                    (idTerm != null && rb.Id == idTerm));
            }

            return query;
        }

        public async Task<IEnumerable<RecurringBooking>> GetDueRecurringBookingsAsync(DateOnly date)
        {
            return await _context.RecurringBookings
                .Include(rb => rb.Customer)
                .Where(rb => rb.Status == RecurringBookingStatus.Active
                    && !rb.IsDeleted
                    && (rb.EndDate == null || rb.EndDate >= date))
                .ToListAsync();
        }
    }
}
