using Neighborhood.Services.Application.Bookings.DTOs;
using Neighborhood.Services.Application.Bookings.Interface;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.BookingImages;
using Neighborhood.Services.Domain.Bookings;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.Bookings
{
    public class BookingRepository: GenericRepository<Booking,int>,IBookingRepository
    {
        public BookingRepository(ApplicationDbContext context):base(context)
        {
            
        }

        public async Task<PagedResult<StaffBookingDto>> GetBookingsForStaffPagedAsync(BookingStatus? status, string? search, int page, int pageSize)
        {
            var query =
                from b in _context.Bookings.AsNoTracking()
                where !b.IsDeleted
                join cust in _context.Customers on b.CustomerId equals cust.Id
                join cu in _context.Users on cust.ApplicationUserId equals cu.Id
                join tech in _context.Technicians on b.TechnicianId equals tech.Id
                join tu in _context.Users on tech.ApplicationUserId equals tu.Id
                select new { b, CustomerName = cu.FullName, TechnicianName = tu.FullName };

            if (status.HasValue)
                query = query.Where(x => x.b.Status == status.Value);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                int? idTerm = int.TryParse(term, out var parsed) ? parsed : null;
                query = query.Where(x =>
                    x.CustomerName.Contains(term) ||
                    x.TechnicianName.Contains(term) ||
                    x.b.Description.Contains(term) ||
                    (idTerm != null && x.b.Id == idTerm));
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.b.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new StaffBookingDto
                {
                    Id = x.b.Id,
                    BookingType = x.b.BookingType,
                    Status = x.b.Status,
                    CustomerName = x.CustomerName,
                    TechnicianName = x.TechnicianName,
                    EstimatedPrice = x.b.EstimatedPrice,
                    FinalPrice = x.b.FinalPrice,
                    ScheduledAt = x.b.ScheduledAt,
                    CreatedAt = x.b.CreatedAt,
                    Address = x.b.Address
                })
                .ToListAsync();

            return new PagedResult<StaffBookingDto>(items, total, page, pageSize);
        }

        public async Task<Booking?> GetBookingWithDetailsAsync(int bookingId)
        {
            return await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Technician)
                .Include(b => b.ProblemType)
                .Include(b => b.BookingImages)
                .Include(b => b.Offer)
                .Include(b => b.ServiceRequest)
                .FirstOrDefaultAsync(b => b.Id == bookingId);
        }

        public async Task<IEnumerable<Booking>> GetCustomerBookingsAsync(int customerId, bool includeDeleted = false)
        {
            var query = _context.Bookings
                .Where(b => b.CustomerId == customerId);

            if (!includeDeleted)
                query = query.Where(b => !b.IsDeleted);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetByRecurringBookingIdAsync(int recurringBookingId)
        {
            return await _context.Bookings
                .AsNoTracking()
                .Where(b => b.RecurringBookingId == recurringBookingId && !b.IsDeleted)
                .OrderBy(b => b.ScheduledAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetTechnicianBookingsAsync(int technicianId)
        {
            return await _context.Bookings
                .Where(b => b.TechnicianId == technicianId && !b.IsDeleted)
                .ToListAsync();
        }

        public async Task<PagedResult<Booking>> GetCustomerBookingsPagedAsync(int customerId, BookingStatus? status, string? search, int page, int pageSize)
        {
            var query = _context.Bookings
                .Where(b => b.CustomerId == customerId && !b.IsDeleted);

            if (status.HasValue)
                query = query.Where(b => b.Status == status.Value);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                int? idTerm = int.TryParse(term, out var parsed) ? parsed : null;
                query = query.Where(b =>
                    b.Description.Contains(term) ||
                    b.Address.Contains(term) ||
                    (idTerm != null && b.Id == idTerm));
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(b => b.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Booking>(items, total, page, pageSize);
        }

        public async Task<PagedResult<Booking>> GetTechnicianBookingsPagedAsync(int technicianId, BookingStatus? status, string? search, int page, int pageSize)
        {
            var query = _context.Bookings
                .Where(b => b.TechnicianId == technicianId && !b.IsDeleted);

            if (status.HasValue)
                query = query.Where(b => b.Status == status.Value);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                int? idTerm = int.TryParse(term, out var parsed) ? parsed : null;
                query = query.Where(b =>
                    b.Description.Contains(term) ||
                    b.Address.Contains(term) ||
                    (idTerm != null && b.Id == idTerm));
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(b => b.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Booking>(items, total, page, pageSize);
        }

        public async Task<IEnumerable<Booking>> GetAllBookingsAsync()
        {
            return await _context.Bookings
                .ToListAsync();
        }
        public async Task<Booking?> GetBookingWithImagesAsync(int bookingId)
        {
            return await _context.Bookings
                .Include(b => b.BookingImages)
                .FirstOrDefaultAsync(b => b.Id == bookingId);
        }

        public async Task<IEnumerable<Booking>> GetBookingsByStatusAsync(BookingStatus status)
        {
            return await _context.Bookings
                .Where(b => b.Status == status)
                .ToListAsync();
        }
        public async Task<Booking?> GetActiveBookingForTechnicianAsync(int technicianId, DateTime scheduledAt)
        {
            return await _context.Bookings
                .FirstOrDefaultAsync(b => b.TechnicianId == technicianId
                    && b.ScheduledAt == scheduledAt
                    && b.Status == BookingStatus.Confirmed
                    && !b.IsDeleted);
        }

        public async Task<bool> HasOverlappingConfirmedBookingAsync(int technicianId, DateTime start, DateTime end, int? excludeBookingId = null)
        {
            // Pull the technician's confirmed bookings that have a duration set,
            // then evaluate interval overlap in memory (small per-technician set, avoids
            // EF translation issues with nullable date arithmetic).
            var confirmed = await _context.Bookings
                .Where(b => b.TechnicianId == technicianId
                    && b.Status == BookingStatus.Confirmed
                    && !b.IsDeleted
                    && b.DurationMinutes != null
                    && (excludeBookingId == null || b.Id != excludeBookingId))
                .Select(b => new { b.ScheduledAt, b.DurationMinutes })
                .ToListAsync();

            // Two intervals [s1,e1) and [s2,e2) overlap when s1 < e2 && s2 < e1
            return confirmed.Any(b =>
                b.ScheduledAt < end &&
                start < b.ScheduledAt.AddMinutes(b.DurationMinutes!.Value));
        }

        public async Task<Booking?> GetBookingWithEscrowAsync(int bookingId)
        {
            return await _context.Bookings
                .Include(b => b.Escrow)
                .FirstOrDefaultAsync(b => b.Id == bookingId);
        }
    }
}
