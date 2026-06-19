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

        // ── Knowledge (RAG) sync ────────────────────────────────────────────────
        // Keep the vector index in step with catalog edits. Enqueued AFTER the DB write
        // commits; the embedding/Qdrant call runs on a Hangfire worker so a slow or
        // out-of-quota AI call never blocks (or fails) the admin's CRUD request.

        /// <summary>Re-embed one category's vector after a create/update.</summary>
        void EnqueueCategoryIndex(int categoryId);

        /// <summary>Remove one category's vector after a delete.</summary>
        void EnqueueCategoryRemoval(int categoryId);

        /// <summary>Re-embed one problem type's vectors after a create/update.</summary>
        void EnqueueProblemTypeIndex(int problemTypeId);

        /// <summary>Remove one problem type's vectors after a delete.</summary>
        void EnqueueProblemTypeRemoval(int problemTypeId);
    }
}
