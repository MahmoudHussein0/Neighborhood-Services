using MediatR;
using Neighborhood.Services.Application.ReviewsAnalysis.DTOs;

namespace Neighborhood.Services.Application.ReviewsAnalysis.Queries
{
    public class GetAllReviewAnalysisQuery
        : IRequest<IReadOnlyList<ReviewAnalysisDto>>
    {
    }
}
