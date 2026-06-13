using MediatR;
using Neighborhood.Services.Application.Reviews.DTOs;
using Neighborhood.Services.Application.Reviews.Interfaces;
using Neighborhood.Services.Application.Reviews.Queries;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Shared.Mappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Reviews.Handlers
{
    public class GetReviewAnalysisByIdQueryHandler : IRequestHandler<GetReviewAnalysisByIdQuery, ReviewAnalysisDto>
    {

        private readonly IReviewAnalysisRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        public GetReviewAnalysisByIdQueryHandler(IReviewAnalysisRepository repository, IUnitOfWork unitOfWork)
        {

            _repository = repository;
            _unitOfWork = unitOfWork;
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
