using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.Disputes.DTOs;
using Neighborhood.Services.Application.Disputes.Interfaces;
using Neighborhood.Services.Domain.Disputes;
using Neighborhood.Services.Domain.Escrows;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;

namespace Neighborhood.Services.Infrastructure.Persistence.Disputes.Repository
{
    public class DisputeRepository : GenericRepository<Dispute, int>, IDisputeRepository
    {

        private readonly ApplicationDbContext _context;
        public DisputeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // ── Queries ────────────────────────────────────────────────────────────
        // the rest of CRUD operations are inherited from GenericRepository

        public async Task<DisputeDto?> GetDetailByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            // Project to a translatable shape (no enum.ToString() in SQL), then map enums client-side.
            // Customer/Technician have no ApplicationUser navigation, so names are looked up by id.
            var row = await _context.Disputes
                .AsNoTracking()
                .Where(d => d.Id == id)
                .Select(d => new
                {
                    d.Id,
                    d.BookingId,
                    d.RaisedByUserId,
                    d.ResolvedByStaffId,
                    d.DisputeType,
                    d.Reason,
                    d.Resolution,
                    d.Status,
                    d.CreatedAt,
                    d.ResolvedAt,

                    CustomerUserId = d.Booking.Customer.ApplicationUserId,
                    TechnicianUserId = d.Booking.Technician.ApplicationUserId,
                    CustomerName = _context.Users
                        .Where(u => u.Id == d.Booking.Customer.ApplicationUserId)
                        .Select(u => u.FullName)
                        .FirstOrDefault(),
                    TechnicianName = _context.Users
                        .Where(u => u.Id == d.Booking.Technician.ApplicationUserId)
                        .Select(u => u.FullName)
                        .FirstOrDefault(),

                    EscrowId = (int?)d.Booking.Escrow!.Id,
                    EscrowStatus = (EscrowStatus?)d.Booking.Escrow!.Status,
                    EscrowAmount = (decimal?)d.Booking.Escrow!.Amount
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (row is null)
                return null;

            return new DisputeDto
            {
                Id = row.Id,
                BookingId = row.BookingId,
                RaisedByUserId = row.RaisedByUserId,
                ResolvedByStaffId = row.ResolvedByStaffId,
                DisputeType = row.DisputeType.ToString(),
                Reason = row.Reason,
                Resolution = row.Resolution,
                Status = row.Status.ToString(),
                CreatedAt = row.CreatedAt,
                ResolvedAt = row.ResolvedAt,

                CustomerUserId = row.CustomerUserId,
                TechnicianUserId = row.TechnicianUserId,
                CustomerName = row.CustomerName,
                TechnicianName = row.TechnicianName,

                EscrowId = row.EscrowId,
                EscrowStatus = row.EscrowStatus?.ToString(),
                EscrowAmount = row.EscrowAmount
            };
        }

        public async Task<IReadOnlyList<Dispute>> GetByStatusAsync(DisputeStatus status, CancellationToken cancellationToken = default)
        {
            return await _context.Disputes
                .Where(d => d.Status == status)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

       
        public async Task<Dispute?> GetByBookingIdAsync(int bookingId, CancellationToken cancellationToken)
        {
            return await _context.Disputes
          .FirstOrDefaultAsync(d => d.BookingId == bookingId);
        }
        public async Task<IReadOnlyList<Dispute>> GetByRaisedByAsync(
    string userId,
    CancellationToken cancellationToken = default)
        {
            return await _context.Disputes
                .Where(d => d.RaisedByUserId == userId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Dispute>> GetByResolvedByStaffIdAsync(int staffId, CancellationToken cancellationToken = default)
        {
            return await _context.Disputes
                .Where(d => d.ResolvedByStaffId == staffId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Dispute>> GetByTypeAsync(DisputeType type, CancellationToken cancellationToken = default)
        {
            return await _context.Disputes
                .Where(d => d.DisputeType == type)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsByBookingIdAsync(int bookingId, CancellationToken cancellationToken = default)
        {
            return await _context.Disputes
                .AnyAsync(d => d.BookingId == bookingId, cancellationToken);
        }


    }
}
