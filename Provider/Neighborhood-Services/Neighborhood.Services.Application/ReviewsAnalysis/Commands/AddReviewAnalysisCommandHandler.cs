using MediatR;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Reviews;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.ReviewsAnalysis.Commands
{
    internal class AddReviewAnalysisCommandHandler : IRequestHandler<AddReviewAnalysisCommand, int>
    {
        private readonly IReviewAnalysisRepository _analysisRepo;
        private readonly IUnitOfWork _unitOfWork;

        public AddReviewAnalysisCommandHandler(IReviewAnalysisRepository analysisRepo, IUnitOfWork unitOfWork)
        {
            _analysisRepo = analysisRepo;
            _unitOfWork = unitOfWork;
        }


        public async Task<int> Handle(AddReviewAnalysisCommand request, CancellationToken cancellationToken)
        {
            var reviewAnalysis = new ReviewAnalysis()
            {
                ReviewId = request.ReviewId,
                Sentiment = request.Sentiment,
                IsFlagged = request.IsFlagged,
                QualityScore = request.QualityScore,
            };

            await _analysisRepo.AddAsync(reviewAnalysis);
            try
            {
               await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                throw;
            }
            return reviewAnalysis.Id;
        }
    }
}
