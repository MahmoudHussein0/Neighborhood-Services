using MediatR;
using Neighborhood.Services.Application.ReviewsAnalysis.DTOs;

namespace Neighborhood.Services.Application.ReviewsAnalysis.Queries
{
    public class GetReviewAnalysisByReviewIdQuery
        : IRequest<ReviewAnalysisDto>
    {
        public int ReviewId { get; set; }
    }
}
