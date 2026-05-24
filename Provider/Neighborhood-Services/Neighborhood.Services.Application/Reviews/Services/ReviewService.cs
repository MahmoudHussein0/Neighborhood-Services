using Neighborhood.Services.Application.Reviews.DTOs;
using Neighborhood.Services.Application.Reviews.Interfaces;
using Neighborhood.Services.Domain.Reviews;

namespace Neighborhood.Services.Application.Reviews.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;

    public ReviewService(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task CreateReviewAsync(CreateReviewDto dto)
    {
        // Validation lives here, not on the entity
        if (dto.Rating < 1 || dto.Rating > 5)
            throw new Exception("Rating must be between 1 and 5");

        // Defaults are set here, not in the constructor
        var review = new Review(
            dto.BookingId,
            dto.ReviewerId,
            dto.RevieweeId,
            dto.Rating,
            dto.Comment,
            status: ReviewStatus.Pending,   // default
            createdAt: DateTime.UtcNow,     // default
            isDeleted: false                // default
        );

        await _reviewRepository.AddAsync(review);
        await _reviewRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var review = await _reviewRepository.GetByIdAsync(id);

        if (review is null)
            throw new Exception("Review not found");

        // Soft delete: build a new state or use a separate method on repo
        // Since the entity is pure, handle this via repository or EF directly
        await _reviewRepository.SoftDeleteAsync(id);
        await _reviewRepository.SaveChangesAsync();
    }

    public async Task<List<ReviewDto>> GetAllAsync()
    {
        var reviews = await _reviewRepository.GetAllAsync();

        return reviews.Select(r => new ReviewDto
        {
            Id = r.Id,
            BookingId = r.BookingId,
            ReviewerId = r.ReviewerId,
            RevieweeId = r.RevieweeId,
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt
        }).ToList();
    }

    public async Task<ReviewDto?> GetByIdAsync(int id)
    {
        var review = await _reviewRepository.GetByIdAsync(id);

        if (review is null)
            return null;

        return new ReviewDto
        {
            Id = review.Id,
            BookingId = review.BookingId,
            ReviewerId = review.ReviewerId,
            RevieweeId = review.RevieweeId,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt
        };
    }
}