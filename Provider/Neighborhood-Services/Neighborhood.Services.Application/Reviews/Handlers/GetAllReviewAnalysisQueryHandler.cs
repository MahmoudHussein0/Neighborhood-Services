using MediatR;
using Neighborhood.Services.Application.Reviews.DTOs;
using Neighborhood.Services.Application.Reviews.Interfaces;
using Neighborhood.Services.Application.Reviews.Queries;
using Neighborhood.Services.Application.Shared.Mappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Reviews.Handlers
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
