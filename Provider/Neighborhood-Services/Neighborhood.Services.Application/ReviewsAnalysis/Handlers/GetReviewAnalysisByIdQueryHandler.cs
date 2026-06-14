using MediatR;
using Neighborhood.Services.Application.ReviewsAnalysis.DTOs;
using Neighborhood.Services.Application.ReviewsAnalysis.Interfaces;
using Neighborhood.Services.Application.ReviewsAnalysis.Queries;
using Neighborhood.Services.Application.Shared.Mappers;

namespace Neighborhood.Services.Application.ReviewsAnalysis.Handlers
{
    public class GetReviewAnalysisByIdQueryHandler
        : IRequestHandler<GetReviewAnalysisByIdQuery, ReviewAnalysisDto>
    {
        private readonly IReviewAnalysisRepository _repository;

        public GetReviewAnalysisByIdQueryHandler(IReviewAnalysisRepository repository)
        {
            _repository = repository;
        }

        public async Task<ReviewAnalysisDto> Handle(
            GetReviewAnalysisByIdQuery request,
            CancellationToken cancellationToken)
        {
            var analysis = await _repository.GetByIdAsync(request.Id);

            if (analysis == null)
                throw new Exception("Review Analysis not found");

            return ReviewAnalysisMapper.MapToDto(analysis);
        }
    }
}
