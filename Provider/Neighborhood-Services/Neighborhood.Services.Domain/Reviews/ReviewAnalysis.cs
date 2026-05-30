using Neighborhood.Services.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Domain.Reviews
{
    public class ReviewAnalysis:BaseEntity<int>
    {
      
        public int ReviewId { get; private set; }

        public ReviewSentiment Sentiment { get; private set; }

        public bool IsFlagged { get; private set; }

        public decimal QualityScore { get; private set; }

        public DateTime CreatedAt { get; private set; }


        // Navigation Property
        public Review Review { get; private set; }
    }
}
