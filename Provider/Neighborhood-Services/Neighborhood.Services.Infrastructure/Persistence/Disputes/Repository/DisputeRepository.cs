using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.Disputes.Interfaces;
using Neighborhood.Services.Domain.Disputes;
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
