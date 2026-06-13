using MediatR;
using Neighborhood.Services.Application.Reviews.Commands;
using Neighborhood.Services.Application.Reviews.DTOs;
using Neighborhood.Services.Application.Reviews.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Shared.Mappers;
using Neighborhood.Services.Domain.Reviews;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Reviews.Handlers
{
    public class CreateReviewAnalysisCommandHandler
    : IRequestHandler<CreateReviewAnalysisCommand, ReviewAnalysisDto>
    {
        private readonly IReviewAnalysisRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateReviewAnalysisCommandHandler(
            IReviewAnalysisRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ReviewAnalysisDto> Handle(
            CreateReviewAnalysisCommand request,
            CancellationToken cancellationToken)
        {
            var exists = await _repository.ExistsByReviewIdAsync(
                request.ReviewId,
                cancellationToken);

            if (exists)
                throw new Exception("Analysis already exists for this review.");

            var analysis = new ReviewAnalysis
            {
                ReviewId = request.ReviewId,
                Sentiment = request.Sentiment,
                IsFlagged = request.IsFlagged,
                QualityScore = request.QualityScore,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(analysis);

            await _unitOfWork.SaveChangesAsync();

            return ReviewAnalysisMapper.MapToDto(analysis);
        }
    }
}
