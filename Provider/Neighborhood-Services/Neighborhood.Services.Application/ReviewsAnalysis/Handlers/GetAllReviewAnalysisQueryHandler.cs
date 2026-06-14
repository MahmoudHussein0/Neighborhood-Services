using MediatR;
using Neighborhood.Services.Application.ReviewsAnalysis.DTOs;
using Neighborhood.Services.Application.ReviewsAnalysis.Interfaces;
using Neighborhood.Services.Application.ReviewsAnalysis.Queries;
using Neighborhood.Services.Application.Shared.Mappers;

namespace Neighborhood.Services.Application.ReviewsAnalysis.Handlers
{
    public class GetAllReviewAnalysisQueryHandler
        : IRequestHandler<
            GetAllReviewAnalysisQuery,
            IReadOnlyList<ReviewAnalysisDto>>
    {
        private readonly IReviewAnalysisRepository _repository;

        public GetAllReviewAnalysisQueryHandler(
            IReviewAnalysisRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<ReviewAnalysisDto>> Handle(
            GetAllReviewAnalysisQuery request,
            CancellationToken cancellationToken)
        {
            var analyses = await _repository.GetAllAsync();

            return analyses
                .Select(ReviewAnalysisMapper.MapToDto)
                .ToList();
        }
    }
}
