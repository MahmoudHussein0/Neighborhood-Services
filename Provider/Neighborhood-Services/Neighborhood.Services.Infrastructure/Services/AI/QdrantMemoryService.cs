using Microsoft.Extensions.AI;
using Neighborhood.Services.Application.AI.Interfaces;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using System.Security.Cryptography;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Services.AI
{
    public class QdrantMemoryService : IVectorMemory
    {
        private readonly QdrantClient _client;
        private readonly IEmbeddingGenerator<string, Embedding<float>> _embedding;
        private const int VectorSize = 1536; // text-embedding-3-small

        public QdrantMemoryService(QdrantClient client, IEmbeddingGenerator<string, Embedding<float>> embedding)
        {
            _client = client;
            _embedding = embedding;
        }

        public async Task UpsertAsync(string collection, string id, string text, IReadOnlyDictionary<string, string>? fields = null)
        {
            await EnsureCollectionAsync(collection);

            // 1. Convert text to a vector
            var vector = await _embedding.GenerateVectorAsync(text);

            // 2. Store the vector + the original text in Qdrant
            var point = new PointStruct
            {
                Id = new PointId { Uuid = DeterministicGuid(id) },
                Vectors = vector.ToArray(),
                Payload = { ["text"] = text }
            };

            // 3. Store any extra fields (e.g. problemTypeId) in the payload as strings
            if (fields != null)
            {
                foreach (var kv in fields)
                    point.Payload[kv.Key] = kv.Value;
            }

            await _client.UpsertAsync(collection, new List<PointStruct> { point });
        }

        // Text-only search — thin wrapper over the detailed one (single source of truth).
        public async Task<IEnumerable<string>> SearchAsync(string collection, string query, int topK = 3)
        {
            var hits = await SearchDetailedAsync(collection, query, topK);
            return hits.Select(h => h.Text).ToList();
        }

        public async Task<IReadOnlyList<SearchHit>> SearchDetailedAsync(string collection, string query, int topK = 3)
        {
            if (!await _client.CollectionExistsAsync(collection))
                return Array.Empty<SearchHit>();

            // 1. Convert the question to a vector
            var queryVector = await _embedding.GenerateVectorAsync(query);

            // 2. Find the nearest stored vectors
            var results = await _client.SearchAsync(collection, queryVector.ToArray(), limit: (ulong)topK);

            // 3. Map each result to text + score + payload fields
            return results.Select(r =>
            {
                var fields = r.Payload.ToDictionary(kv => kv.Key, kv => kv.Value.StringValue ?? "");
                var text = r.Payload.TryGetValue("text", out var t) ? t.StringValue : "";
                return new SearchHit(text, r.Score, fields);
            }).ToList();
        }

        private async Task EnsureCollectionAsync(string collection)
        {
            if (await _client.CollectionExistsAsync(collection))
                return;

            await _client.CreateCollectionAsync(collection, new VectorParams
            {
                Size = VectorSize,
                Distance = Distance.Cosine
            });
        }

        private static string DeterministicGuid(string input)
        {
            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return new Guid(hash).ToString();
        }
    }
}