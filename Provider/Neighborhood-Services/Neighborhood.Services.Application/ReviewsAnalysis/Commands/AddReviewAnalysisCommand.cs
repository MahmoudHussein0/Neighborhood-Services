using MediatR;
using Neighborhood.Services.Domain.Reviews;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.ReviewsAnalysis.Commands
{
    public class AddReviewAnalysisCommand : IRequest<int>
    {
        public int ReviewId { get; set; }
        public ReviewSentiment Sentiment { get; set; }
        public bool IsFlagged { get; set; }
        public decimal QualityScore { get; set; }
    }
}
