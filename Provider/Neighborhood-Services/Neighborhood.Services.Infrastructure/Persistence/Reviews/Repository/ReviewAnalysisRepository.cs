using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.Reviews.Interfaces;
using Neighborhood.Services.Domain.Reviews;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.Reviews.Repository
{
    public class ReviewAnalysisRepository
     : GenericRepository<ReviewAnalysis, int>,
       IReviewAnalysisRepository
    {
        private readonly ApplicationDbContext _context;

        public ReviewAnalysisRepository(ApplicationDbContext context) : base(context) { _context = context; }


        public async Task<ReviewAnalysis?> GetByReviewIdAsync(
            int reviewId,
            CancellationToken cancellationToken = default)
        {
            return await _context.ReviewAnalyses
                .FirstOrDefaultAsync(
                    x => x.ReviewId == reviewId,
                    cancellationToken);
        }

        public async Task<bool> ExistsByReviewIdAsync(
            int reviewId,
            CancellationToken cancellationToken = default)
        {
            return await _context.ReviewAnalyses
                .AnyAsync(
                    x => x.ReviewId == reviewId,
                    cancellationToken);
        }
    }
}
