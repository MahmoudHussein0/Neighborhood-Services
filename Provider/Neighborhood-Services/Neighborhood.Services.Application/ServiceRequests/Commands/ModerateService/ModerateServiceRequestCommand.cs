using MediatR;

namespace Neighborhood.Services.Application.ServiceRequests.Commands.ModerateService
{
    /// <summary>
    /// Runs the content-moderation agent over a service request's description + image,
    /// then flips its status from PendingReview to Open (clean) or Flagged (inappropriate).
    /// Dispatched from a Hangfire background job — the user never waits for this.
    /// Returns true if the request ended up Open (allowed), false if Flagged.
    /// </summary>
    public class ModerateServiceRequestCommand : IRequest<bool>
    {
        public int ServiceRequestId { get; set; }
    }
}
