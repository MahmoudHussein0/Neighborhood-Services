using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.AI.Interfaces
{
    // One search result: the stored text, the similarity score (0..1), and any
    // extra payload fields we stored (e.g. "problemTypeId").
    public record SearchHit(string Text, float Score, IReadOnlyDictionary<string, string> Fields);

    public interface IVectorMemory
    {
        // fields = optional extra payload stored alongside the text (e.g. { "problemTypeId": "5" })
        Task UpsertAsync(string collection, string id, string text, IReadOnlyDictionary<string, string>? fields = null);

        // text-only search (for the chatbot RAG context)
        Task<IEnumerable<string>> SearchAsync(string collection, string query, int topK = 3);

        // detailed search — returns text + score + payload (for classification: get the id back)
        Task<IReadOnlyList<SearchHit>> SearchDetailedAsync(string collection, string query, int topK = 3);
    }
}
