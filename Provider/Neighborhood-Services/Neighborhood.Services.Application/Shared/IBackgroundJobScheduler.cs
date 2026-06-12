namespace Neighborhood.Services.Application.Shared
{
    /// <summary>
    /// Thin seam over the background-job engine (Hangfire) so the Application layer
    /// can enqueue work WITHOUT taking a direct dependency on Hangfire.
    /// Implemented in Infrastructure, where Hangfire lives.
    /// </summary>
    public interface IBackgroundJobScheduler
    {
        /// <summary>
        /// Queue a moderation pass for a freshly-created service request.
        /// Returns immediately — the actual AI call runs on a Hangfire worker.
        /// </summary>
        void EnqueueServiceRequestModeration(int serviceRequestId);
    }
}
