using MediatR;
using Neighborhood.Services.Application.ReviewsAnalysis.DTOs;
using Neighborhood.Services.Application.ReviewsAnalysis.Interfaces;
using Neighborhood.Services.Application.ReviewsAnalysis.Queries;
using Neighborhood.Services.Application.Shared.Mappers;

namespace Neighborhood.Services.Application.ReviewsAnalysis.Handlers
{
    public class GetReviewAnalysisByReviewIdQueryHandler
        : IRequestHandler<GetReviewAnalysisByReviewIdQuery, ReviewAnalysisDto>
    {
        private readonly IReviewAnalysisRepository _repository;

        public GetReviewAnalysisByReviewIdQueryHandler(IReviewAnalysisRepository repository)
        {
            _repository = repository;
        }

        public async Task<ReviewAnalysisDto> Handle(
            GetReviewAnalysisByReviewIdQuery request,
            CancellationToken cancellationToken)
        {
            var analysis = await _repository
                .GetByReviewIdAsync(request.ReviewId, cancellationToken);

            if (analysis == null)
                throw new Exception("Review Analysis not found");

            return ReviewAnalysisMapper.MapToDto(analysis);
        }
    }
}
