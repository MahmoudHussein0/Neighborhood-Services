using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.AI.Interfaces
{
    // One search result: the stored text, the similarity score (0..1), and any
    // extra payload fields we stored (e.g. "problemTypeId").
    public record SearchHit(string Text, float Score, IReadOnlyDictionary<string, string> Fields);

    // One doc to embed+store: id, text, and optional payload fields.
    public record VectorDoc(string Id, string Text, IReadOnlyDictionary<string, string>? Fields = null);

    public interface IVectorMemory
    {
        // fields = optional extra payload stored alongside the text (e.g. { "problemTypeId": "5" })
        Task UpsertAsync(string collection, string id, string text, IReadOnlyDictionary<string, string>? fields = null);

        // Batched upsert: embeds all docs in as few embedding calls as possible (one request per
        // chunk) and stores them together. Far cheaper than calling UpsertAsync in a loop.
        Task UpsertManyAsync(string collection, IReadOnlyList<VectorDoc> docs);

        // text-only search (for the chatbot RAG context)
        Task<IEnumerable<string>> SearchAsync(string collection, string query, int topK = 3);

        // detailed search — returns text + score + payload (for classification: get the id back)
        Task<IReadOnlyList<SearchHit>> SearchDetailedAsync(string collection, string query, int topK = 3);

        // Reads a single stored payload field for a doc by id. Returns null if the doc
        // doesn't exist (or the field isn't set). Used to skip re-embedding unchanged docs.
        Task<string?> GetFieldAsync(string collection, string id, string field);

        // Deletes every point in the collection whose doc id is NOT in keepIds.
        // Used to prune orphaned vectors (docs deleted from the source-of-truth DB).
        Task RemoveExceptAsync(string collection, IReadOnlyCollection<string> keepIds);

        // Deletes a single doc by id (no-op if it doesn't exist). For event-driven removals.
        Task RemoveAsync(string collection, string id);
    }
}
