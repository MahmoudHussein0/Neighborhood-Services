using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.ReviewsAnalysis.Interfaces;
using Neighborhood.Services.Domain.Reviews;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;

namespace Neighborhood.Services.Infrastructure.Persistence.ReviewsAnalysis
{
    public class ReviewAnalysisRepository
        : GenericRepository<ReviewAnalysis, int>, IReviewAnalysisRepository
    {
        private readonly ApplicationDbContext _context;

        public ReviewAnalysisRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ReviewAnalysis?> GetByReviewIdAsync(
            int reviewId,
            CancellationToken cancellationToken = default)
        {
            return await _context.ReviewAnalyses
                .FirstOrDefaultAsync(x => x.ReviewId == reviewId, cancellationToken);
        }

        public async Task<bool> ExistsByReviewIdAsync(
            int reviewId,
            CancellationToken cancellationToken = default)
        {
            return await _context.ReviewAnalyses
                .AnyAsync(x => x.ReviewId == reviewId, cancellationToken);
        }
    }
}
