using MediatR;
using Neighborhood.Services.Application.Reviews.DTOs;

namespace Neighborhood.Services.Application.Reviews.Queries
{
    public class GetReviewByIdQuery : IRequest<ReviewDto>
    {
        public int Id { get; set; }
        public GetReviewByIdQuery(int id) => Id = id;
    }
}
