using Neighborhood.Services.Domain.Reviews;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.QA.DTOs
{
    public class ReviewAnalysisDto
    {
        public ReviewSentiment Sentiment { get; set; } 
        public int QualityScore { get; set; }
        public bool FlagAbuse { get; set; }
    }
}
