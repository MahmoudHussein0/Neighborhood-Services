using MediatR;
using Neighborhood.Services.Application.ReviewsAnalysis.DTOs;

namespace Neighborhood.Services.Application.ReviewsAnalysis.Queries
{
    public class GetReviewAnalysisByIdQuery : IRequest<ReviewAnalysisDto>
    {
        public int Id { get; set; }
    }
}
