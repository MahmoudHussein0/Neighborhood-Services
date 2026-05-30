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

        public async Task<Dispute?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Disputes
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Dispute>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Disputes
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Dispute>> GetByStatusAsync(DisputeStatus status, CancellationToken cancellationToken = default)
        {
            return await _context.Disputes
                .Where(d => d.Status == status)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Dispute>> GetByBookingIdAsync(int bookingId, CancellationToken cancellationToken = default)
        {
            return await _context.Disputes
                .Where(d => d.BookingId == bookingId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Dispute>> GetByRaisedByAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Disputes
                .Where(d => d.RaisedBy == userId)
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

        // ── Commands ───────────────────────────────────────────────────────────

        public async Task AddAsync(Dispute dispute, CancellationToken cancellationToken = default)
        {
            await _context.Disputes.AddAsync(dispute, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Dispute dispute, CancellationToken cancellationToken = default)
        {
            _context.Disputes.Update(dispute);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Dispute dispute, CancellationToken cancellationToken = default)
        {
            _context.Disputes.Remove(dispute);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
