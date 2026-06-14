using MediatR;
using Neighborhood.Services.Application.ReviewsAnalysis.DTOs;
using Neighborhood.Services.Domain.Reviews;

namespace Neighborhood.Services.Application.ReviewsAnalysis.Commands
{
    public record CreateReviewAnalysisCommand(
        int ReviewId,
        ReviewSentiment Sentiment,
        bool IsFlagged,
        decimal QualityScore
    ) : IRequest<ReviewAnalysisDto>;
}
