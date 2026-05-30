using Neighborhood.Services.Domain.Reviews;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Reviews.Interfaces
{
    public interface IReviewRepository
    {
        // ── Queries ────────────────────────────────────────────────────────────
        Task<Review?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Review?> GetByBookingIdAsync(int bookingId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Review>> GetByReviewerIdAsync(int reviewerId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Review>> GetByRevieweeIdAsync(int revieweeId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Review>> GetByStatusAsync(ReviewStatus status, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Review>> GetFlaggedAsync(CancellationToken cancellationToken = default);
        Task<bool> ExistsByBookingIdAsync(int bookingId, CancellationToken cancellationToken = default);

        // ── Commands ───────────────────────────────────────────────────────────
        Task AddAsync(Review review, CancellationToken cancellationToken = default);
        Task UpdateAsync(Review review, CancellationToken cancellationToken = default);
        Task DeleteAsync(Review review, CancellationToken cancellationToken = default);
    }
}
