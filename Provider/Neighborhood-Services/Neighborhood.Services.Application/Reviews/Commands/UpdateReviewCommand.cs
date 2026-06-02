using MediatR;
using Neighborhood.Services.Application.Reviews.DTOs;

namespace Neighborhood.Services.Application.Reviews.Commands
{
    public class UpdateReviewCommand : IRequest<ReviewDto>
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
