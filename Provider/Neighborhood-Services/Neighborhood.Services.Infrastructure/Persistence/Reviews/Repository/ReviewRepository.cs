using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.Reviews.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Reviews;

namespace Neighborhood.Services.Infrastructure.Persistence.Reviews.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly IApplicationDbContext _context;

        public ReviewRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        // ── Queries ────────────────────────────────────────────────────────────

        public async Task<Review?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Reviews
                .Include(r => r.Analysis)
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        }

        public async Task<Review?> GetByBookingIdAsync(int bookingId, CancellationToken cancellationToken = default)
        {
            return await _context.Reviews
                .Include(r => r.Analysis)
                .FirstOrDefaultAsync(r => r.BookingId == bookingId, cancellationToken);
        }

        public async Task<IReadOnlyList<Review>> GetByReviewerIdAsync(int reviewerId, CancellationToken cancellationToken = default)
        {
            return await _context.Reviews
                .Include(r => r.Analysis)
                .Where(r => r.ReviewerId == reviewerId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Review>> GetByRevieweeIdAsync(int revieweeId, CancellationToken cancellationToken = default)
        {
            return await _context.Reviews
                .Include(r => r.Analysis)
                .Where(r => r.RevieweeId == revieweeId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Review>> GetByStatusAsync(ReviewStatus status, CancellationToken cancellationToken = default)
        {
            return await _context.Reviews
                .Include(r => r.Analysis)
                .Where(r => r.Status == status)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        // Returns reviews whose analysis is flagged
        public async Task<IReadOnlyList<Review>> GetFlaggedAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Reviews
                .Include(r => r.Analysis)
                .Where(r => r.Analysis != null && r.Analysis.IsFlagged)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsByBookingIdAsync(int bookingId, CancellationToken cancellationToken = default)
        {
            return await _context.Reviews
                .AnyAsync(r => r.BookingId == bookingId, cancellationToken);
        }

        // ── Commands ───────────────────────────────────────────────────────────

        public async Task AddAsync(Review review, CancellationToken cancellationToken = default)
        {
            await _context.Reviews.AddAsync(review, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Review review, CancellationToken cancellationToken = default)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync(cancellationToken);
        }

        // Hard delete — soft delete is handled via IsDeleted flag on the domain side
        public async Task DeleteAsync(Review review, CancellationToken cancellationToken = default)
        {
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
