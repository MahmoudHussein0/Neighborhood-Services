using Neighborhood.Services.Application.Reviews.DTOs;
using Neighborhood.Services.Domain.Reviews;

namespace Neighborhood.Services.Application.Shared.Mappers
{
    public static class ReviewMapper
    {
        public static ReviewDto MapToDto(Review review) => new()
        {
            Id = review.Id,
            BookingId = review.BookingId,
            ReviewerId = review.ReviewerId,
            RevieweeId = review.RevieweeId,
            ReviewerName = review.Reviewer?.FullName,
            RevieweeName = review.Reviewee?.FullName,
            Rating = review.Rating,
            Comment = review.Comment,
            Status = review.Status.ToString(),
            CreatedAt = review.CreatedAt
        };
    }
}
