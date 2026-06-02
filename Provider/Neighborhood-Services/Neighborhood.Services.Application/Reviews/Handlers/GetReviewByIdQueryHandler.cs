using MediatR;
using Neighborhood.Services.Application.Reviews.DTOs;
using Neighborhood.Services.Application.Reviews.Interfaces;
using Neighborhood.Services.Application.Reviews.Queries;
using Neighborhood.Services.Application.Shared.Mappers;

namespace Neighborhood.Services.Application.Reviews.Handlers
{
    public class GetReviewByIdQueryHandler : IRequestHandler<GetReviewByIdQuery, ReviewDto>
    {
        private readonly IReviewRepository _repository;

        public GetReviewByIdQueryHandler(IReviewRepository repository)
        {
            _repository = repository;
        }

        public async Task<ReviewDto> Handle(GetReviewByIdQuery request, CancellationToken cancellationToken)
        {
            var review = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (review is null)
                throw new Exception($"Review with id {request.Id} not found.");

            return ReviewMapper.MapToDto(review);
        }
    }
}
