using Neighborhood.Services.Domain.Reviews;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Reviews.DTOs
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
