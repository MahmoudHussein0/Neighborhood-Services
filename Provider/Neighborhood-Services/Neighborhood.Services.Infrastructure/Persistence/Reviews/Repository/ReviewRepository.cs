using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.Reviews.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Reviews;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;

namespace Neighborhood.Services.Infrastructure.Persistence.Reviews.Repository
{
    public class ReviewRepository : GenericRepository<Review, int>, IReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public ReviewRepository(ApplicationDbContext context) : base(context){ _context = context; }

        // ── Queries ────────────────────────────────────────────────────────────

        // Override so the staff reviews list carries reviewer/reviewee names (FullName) + analysis.
        public override async Task<IReadOnlyList<Review>> GetAllAsync()
        {
            return await _context.Reviews
                .Include(r => r.Analysis)
                .Include(r => r.Reviewer)
                .Include(r => r.Reviewee)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Review?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Reviews
                .Include(r => r.Analysis)
                .Include(r => r.Reviewer)
                .Include(r => r.Reviewee)
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        }

        public async Task<Review?> GetByBookingIdAsync(int bookingId, CancellationToken cancellationToken = default)
        {
            return await _context.Reviews
                .Include(r => r.Analysis)
                .FirstOrDefaultAsync(r => r.BookingId == bookingId, cancellationToken);
        }
       
        public async Task<IReadOnlyList<Review>> GetByReviewerIdAsync(string reviewerId, CancellationToken cancellationToken = default)
        {
            return await _context.Reviews
                .Include(r => r.Analysis)
                .Where(r => r.ReviewerId == reviewerId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Review>> GetByRevieweeIdAsync(string revieweeId, CancellationToken cancellationToken = default)
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
                .Include(r => r.Reviewer)
                .Include(r => r.Reviewee)
                .Where(r => r.Analysis != null && r.Analysis.IsFlagged)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        //public async Task<bool> ExistsByBookingIdAsync(int bookingId, CancellationToken cancellationToken = default)
        //{
        //    return await _context.Reviews
        //        .AnyAsync(r => r.BookingId == bookingId, cancellationToken);
        //}

        // ── Commands ───────────────────────────────────────────────────────────



     
        public async Task<bool> ExistsByDirectionAsync(int bookingId,ReviewType direction,CancellationToken cancellationToken = default)
        {
            return await _context.Reviews.AnyAsync(r =>
                r.BookingId == bookingId &&
                r.ReviewType == direction,
                cancellationToken);
        }

        public async Task<HashSet<int>> GetBookingIdsReviewedByAsync(IEnumerable<int> bookingIds, string reviewerId, CancellationToken cancellationToken = default)
        {
            var ids = bookingIds as ICollection<int> ?? bookingIds.ToList();
            if (ids.Count == 0) return new HashSet<int>();

            var reviewed = await _context.Reviews
                .Where(r => r.ReviewerId == reviewerId && ids.Contains(r.BookingId))
                .Select(r => r.BookingId)
                .ToListAsync(cancellationToken);

            return reviewed.ToHashSet();
        }
    }
}
