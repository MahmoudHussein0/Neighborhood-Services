using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Reviews;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Reviews.Interfaces
{
    public interface IReviewAnalysisRepository
     : IGenericRepository<ReviewAnalysis, int>
    {
        Task<ReviewAnalysis?> GetByReviewIdAsync(
            int reviewId,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsByReviewIdAsync(
            int reviewId,
            CancellationToken cancellationToken = default);
    }
}
