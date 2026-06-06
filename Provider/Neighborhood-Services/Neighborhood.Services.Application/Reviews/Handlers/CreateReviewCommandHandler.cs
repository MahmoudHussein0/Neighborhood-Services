using MediatR;
using Neighborhood.Services.Application.Reviews.Commands;
using Neighborhood.Services.Application.Reviews.DTOs;
using Neighborhood.Services.Application.Reviews.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Reviews;
using Neighborhood.Services.Application.Shared.Mappers;


namespace Neighborhood.Services.Application.Reviews.Handlers
{
    public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, ReviewDto>
    {
        private readonly IReviewRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateReviewCommandHandler(IReviewRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ReviewDto> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
        {
            var review = new Review
            {
                BookingId = request.BookingId,
                ReviewerId = request.ReviewerId,
                RevieweeId = request.RevieweeId,
                Rating = request.Rating,
                Comment = request.Comment,
                Status = ReviewStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _repository.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();

            return ReviewMapper.MapToDto(review);
        }
    }
}
