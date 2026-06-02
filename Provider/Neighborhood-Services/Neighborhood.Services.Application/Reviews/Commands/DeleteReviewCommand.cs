using MediatR;

namespace Neighborhood.Services.Application.Reviews.Commands
{
    public class DeleteReviewCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
