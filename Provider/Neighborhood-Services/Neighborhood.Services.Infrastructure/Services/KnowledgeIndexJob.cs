using Hangfire;
using Neighborhood.Services.Application.AI.Interfaces;

namespace Neighborhood.Services.Infrastructure.Services
{
    /// <summary>
    /// Hangfire entry point for keeping the vector (RAG) index in sync with catalog edits.
    /// Hangfire resolves this from DI on a worker thread and calls one of the methods below.
    /// Stays thin — the real work lives in <see cref="IKnowledgeIndexer"/> (KnowledgeSeeder).
    ///
    /// Retries are capped low on purpose: if the embedding call fails because the OpenAI key
    /// is out of quota (or Qdrant is down), retrying 10× the default way just burns the worker
    /// for a problem that won't fix itself soon. We retry at most 3 times, spread over
    /// 1 / 5 / 15 minutes, then give up (the row stays in the DB; a later /reindex re-syncs it).
    /// </summary>
    [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 60, 300, 900 })]
    public class KnowledgeIndexJob
    {
        private readonly IKnowledgeIndexer _indexer;

        public KnowledgeIndexJob(IKnowledgeIndexer indexer)
        {
            _indexer = indexer;
        }

        // Cascade for categories: a category's name is embedded in each of its problem types,
        // so create/update/delete must also refresh/remove the children.
        public Task IndexCategory(int categoryId) => _indexer.IndexCategoryWithChildrenAsync(categoryId);

        public Task RemoveCategory(int categoryId) => _indexer.RemoveCategoryWithChildrenAsync(categoryId);

        public Task IndexProblemType(int problemTypeId) => _indexer.IndexProblemTypeAsync(problemTypeId);

        public Task RemoveProblemType(int problemTypeId) => _indexer.RemoveProblemTypeAsync(problemTypeId);
    }
}
