using Neighborhood.Services.Application.Reviews.DTOs;
using Neighborhood.Services.Application.Reviews.Interfaces;
using Neighborhood.Services.Application.Reviews.Queries;
using Neighborhood.Services.Application.Shared.Mappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Reviews.Handlers
{
    public class GetReviewAnalysisByReviewIdQueryHandler
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
