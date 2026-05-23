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
        // Validation
        if (dto.Rating < 1 || dto.Rating > 5)
        {
            throw new Exception("Rating must be between 1 and 5");
        }

        // Create Entity
        var review = new Review();

        // Save
        await _reviewRepository.AddAsync(review);

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

    public async Task DeleteAsync(int id)
    {
        var review = await _reviewRepository.GetByIdAsync(id);

        if (review is null)
            throw new Exception("Review not found");

        review.Delete();

        await _reviewRepository.SaveChangesAsync();
    }
}