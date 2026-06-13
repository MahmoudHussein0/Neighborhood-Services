using MediatR;
using Neighborhood.Services.Application.Reviews.DTOs;
using Neighborhood.Services.Domain.Reviews;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Reviews.Commands
{
    public record CreateReviewAnalysisCommand(
     int ReviewId,
     ReviewSentiment Sentiment,
     bool IsFlagged,
     decimal QualityScore
 ) : IRequest<ReviewAnalysisDto>;
}
