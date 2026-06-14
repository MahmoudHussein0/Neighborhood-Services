using Neighborhood.Services.Domain.Reviews;

namespace Neighborhood.Services.Application.ReviewsAnalysis.DTOs
{
    public class ReviewAnalysisDto
    {
        public int Id { get; set; }

        public int ReviewId { get; set; }

        public ReviewSentiment Sentiment { get; set; }

        public bool IsFlagged { get; set; }

        public decimal QualityScore { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
