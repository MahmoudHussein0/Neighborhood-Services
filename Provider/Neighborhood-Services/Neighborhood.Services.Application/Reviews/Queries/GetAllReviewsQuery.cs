using MediatR;
using Neighborhood.Services.Application.Reviews.DTOs;

namespace Neighborhood.Services.Application.Reviews.Queries
{
    public class GetAllReviewsQuery : IRequest<IReadOnlyList<ReviewDto>>
    {
    }

}
