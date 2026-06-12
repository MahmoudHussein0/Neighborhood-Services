using Hangfire;
using Neighborhood.Services.Application.Shared;

namespace Neighborhood.Services.Infrastructure.Services
{
    /// <summary>
    /// Hangfire-backed implementation of <see cref="IBackgroundJobScheduler"/>.
    /// Lives in Infrastructure so the Application layer never references Hangfire.
    /// Uses the static <see cref="BackgroundJob"/> facade — same engine/storage as the
    /// RecurringJob registrations in Program.cs.
    /// </summary>
    public class BackgroundJobScheduler : IBackgroundJobScheduler
    {
        public void EnqueueServiceRequestModeration(int serviceRequestId)
        {
            BackgroundJob.Enqueue<ServiceRequestModerationJob>(
                job => job.Run(serviceRequestId));
        }
    }
}
