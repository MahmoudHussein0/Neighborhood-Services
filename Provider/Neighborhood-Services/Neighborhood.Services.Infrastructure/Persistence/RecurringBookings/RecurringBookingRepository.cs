using Neighborhood.Services.Application.RecurringBookings.Interfaces;
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
                .Where(rb => rb.IsActive && !rb.IsDeleted)
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

        public async Task<IEnumerable<RecurringBooking>> GetDueRecurringBookingsAsync(DateOnly date)
        {
            return await _context.RecurringBookings
                .Where(rb => rb.IsActive
                    && !rb.IsDeleted
                    && (rb.EndDate == null || rb.EndDate >= date))
                .ToListAsync();
        }
    }
}
