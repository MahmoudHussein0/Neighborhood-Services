using MediatR;
using Neighborhood.Services.Application.Reviews.DTOs;

namespace Neighborhood.Services.Application.Reviews.Commands
{
    // ReviewerId and RevieweeId are intentionally NOT on the command — the handler derives
    // ReviewerId from the auth token (ICurrentUserService) and RevieweeId from the booking's
    // other party. Trusting raw ids from the client would let anyone review *as* someone else.
    public class CreateReviewCommand : IRequest<ReviewDto>
    {
        public int BookingId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }

}
