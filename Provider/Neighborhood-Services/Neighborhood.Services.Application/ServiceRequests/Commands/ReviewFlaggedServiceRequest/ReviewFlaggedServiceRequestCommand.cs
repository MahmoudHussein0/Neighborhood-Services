using MediatR;

namespace Neighborhood.Services.Application.ServiceRequests.Commands.ReviewFlaggedServiceRequest
{
    // Staff decision on a Flagged request: Approve -> Open (goes live), Reject -> Closed.
    public class ReviewFlaggedServiceRequestCommand : IRequest<bool>
    {
        public int ServiceRequestId { get; set; }
        public bool Approved { get; set; }
    }
}
