using Neighborhood.Services.Application.Reviews.Interfaces;
using Neighborhood.Services.Domain.Reviews;

namespace Neighborhood.Services.Application.Reviews.Services;

public class ReviewAnalysisService : IReviewAnalysisService
{
    public async Task AnalyzeAsync(int reviewId, string comment)
    {
        // Fake AI Logic

        var sentiment = ReviewSentiment.Neutral;

        if (comment.Contains("bad"))
        {
            sentiment = ReviewSentiment.Negative;
        }

        if (comment.Contains("excellent"))
        {
            sentiment = ReviewSentiment.Positive;
        }

        bool isFlagged = sentiment == ReviewSentiment.Negative;

        decimal qualityScore = 0.85m;

        // Later:
        // Save ReviewAnalysis in DB
    }
}