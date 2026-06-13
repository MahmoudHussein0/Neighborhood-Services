using Neighborhood.Services.Application.ReviewsAnalysis.DTOs;
using Neighborhood.Services.Domain.Reviews;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Shared.Mappers
{
    public class ReviewAnalysisMapper
    {
        public static ReviewAnalysisDto MapToDto(ReviewAnalysis analysis)
        {
            return new ReviewAnalysisDto
            {
                Id = analysis.Id,
                ReviewId = analysis.ReviewId,
                Sentiment = analysis.Sentiment,
                IsFlagged = analysis.IsFlagged,
                QualityScore = analysis.QualityScore,
                CreatedAt = analysis.CreatedAt
            };
        }
    }
}
