using MediatR;
using Neighborhood.Services.Application.Reviews.Interfaces;
using Neighborhood.Services.Application.ReviewsAnalysis.Commands;
using Neighborhood.Services.Application.ReviewsAnalysis.DTOs;
using Neighborhood.Services.Application.ReviewsAnalysis.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Shared.Mappers;
using Neighborhood.Services.Domain.Reviews;

namespace Neighborhood.Services.Application.ReviewsAnalysis.Handlers
{
    public class CreateReviewAnalysisCommandHandler
        : IRequestHandler<CreateReviewAnalysisCommand, ReviewAnalysisDto>
    {
        private readonly IReviewAnalysisRepository _repository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateReviewAnalysisCommandHandler(
            IReviewAnalysisRepository repository,
            IReviewRepository reviewRepository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _reviewRepository = reviewRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ReviewAnalysisDto> Handle(
            CreateReviewAnalysisCommand request,
            CancellationToken cancellationToken)
        {
            var exists = await _repository.ExistsByReviewIdAsync(
                request.ReviewId,
                cancellationToken);

            if (exists)
                throw new Exception("Analysis already exists for this review.");

            var analysis = new ReviewAnalysis
            {
                ReviewId = request.ReviewId,
                Sentiment = request.Sentiment,
                IsFlagged = request.IsFlagged,
                QualityScore = request.QualityScore,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(analysis);

            // Drive the review's moderation status from the AI verdict:
            // flagged → Flagged (hidden from public, surfaces in the staff queue),
            // otherwise → Approved (visible). Tracked entity, committed in the same SaveChanges.
            var review = await _reviewRepository.GetByIdAsync(request.ReviewId, cancellationToken);
            if (review is not null)
            {
                review.Status = request.IsFlagged ? ReviewStatus.Flagged : ReviewStatus.Approved;
            }

            await _unitOfWork.SaveChangesAsync();

            return ReviewAnalysisMapper.MapToDto(analysis);
        }
    }
}
