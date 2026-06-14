using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Reviews;

namespace Neighborhood.Services.Application.ReviewsAnalysis.Interfaces
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
