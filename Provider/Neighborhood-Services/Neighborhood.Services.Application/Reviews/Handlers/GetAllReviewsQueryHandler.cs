using MediatR;
using Neighborhood.Services.Application.Reviews.DTOs;
using Neighborhood.Services.Application.Reviews.Interfaces;
using Neighborhood.Services.Application.Reviews.Queries;
using Neighborhood.Services.Application.Shared.Mappers;


namespace Neighborhood.Services.Application.Reviews.Handlers
{
    public class GetAllReviewsQueryHandler : IRequestHandler<GetAllReviewsQuery, IReadOnlyList<ReviewDto>>
    {
        private readonly IReviewRepository _repository;

        public GetAllReviewsQueryHandler(IReviewRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<ReviewDto>> Handle(GetAllReviewsQuery request, CancellationToken cancellationToken)
        {
            var reviews = await _repository.GetAllAsync(cancellationToken);
            return reviews.Select(ReviewMapper.MapToDto).ToList();
        }
    }
}
