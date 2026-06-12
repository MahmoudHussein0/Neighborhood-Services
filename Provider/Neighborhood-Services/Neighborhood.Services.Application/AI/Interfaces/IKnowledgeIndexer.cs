namespace Neighborhood.Services.Application.AI.Interfaces
{
    /// <summary>
    /// Keeps the vector knowledge index in sync with the database.
    ///
    /// Two layers:
    ///   - <see cref="ReindexAllAsync"/> : bulk rebuild of the WHOLE index from the DB
    ///     (+ static docs + FAQs). Triggered deliberately — CLI, an admin "reindex" button,
    ///     or a CI/CD deploy step. Use for first-time setup, recovery, or migrations.
    ///   - The single-item hooks : keep the index live as individual catalog rows change.
    ///     Call these from the admin create/update/delete handlers (wiring comes later).
    /// </summary>
    public interface IKnowledgeIndexer
    {
        Task ReindexAllAsync();

        // Upsert/refresh the vector(s) for one item (call on admin create + update).
        Task IndexCategoryAsync(int categoryId);
        Task IndexProblemTypeAsync(int problemTypeId);

        // Remove the vector(s) for one item (call on admin delete).
        Task RemoveCategoryAsync(int categoryId);
        Task RemoveProblemTypeAsync(int problemTypeId);
    }
}
