using MediatR;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Reviews.Commands;
using Neighborhood.Services.Application.Reviews.DTOs;
using Neighborhood.Services.Application.Reviews.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Shared.Mappers;
using Neighborhood.Services.Domain.Reviews;

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
            var review = await _repository.GetByIdAsync(request.Id);
            if (review is null)
                throw new NotFoundException(nameof(Review), request.Id);

            // Staff moderation: change the review's status (Approve / Reject / Flag).
            if (request.Status.HasValue)
                review.Status = request.Status.Value;

            // Edit the rating/comment — only allowed while the review isn't already approved.
            if (request.Rating.HasValue || request.Comment is not null)
            {
                if (review.Status == ReviewStatus.Approved && !request.Status.HasValue)
                    throw new BadRequestException("Approved reviews cannot be edited.");

                if (request.Rating.HasValue)
                {
                    if (request.Rating < 1 || request.Rating > 5)
                        throw new BadRequestException("Invalid rating. Must be between 1 and 5.");
                    review.Rating = request.Rating.Value;
                }

                if (request.Comment is not null)
                    review.Comment = request.Comment;
            }

            await _repository.UpdateAsync(review);
            await _unitOfWork.SaveChangesAsync();

            return ReviewMapper.MapToDto(review);
        }
    }
}
