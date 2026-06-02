using MediatR;
using Neighborhood.Services.Application.Reviews.Commands;
using Neighborhood.Services.Application.Reviews.DTOs;
using Neighborhood.Services.Application.Reviews.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Shared.Mappers;

namespace Neighborhood.Services.Application.Reviews.Handlers
{
    public class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommand, ReviewDto>
    {
        private readonly IReviewRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateReviewCommandHandler(IReviewRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ReviewDto> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
        {
            var review = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (review is null)
                throw new Exception($"Review with id {request.Id} not found.");

            review.Rating = request.Rating;
            review.Comment = request.Comment;

            await _repository.UpdateAsync(review);
            await _unitOfWork.SaveChangesAsync();

            return ReviewMapper.MapToDto(review);
        }
    }
}
