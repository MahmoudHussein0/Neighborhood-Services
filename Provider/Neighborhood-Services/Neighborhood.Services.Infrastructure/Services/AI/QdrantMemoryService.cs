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

        public async Task UpsertManyAsync(string collection, IReadOnlyList<VectorDoc> docs)
        {
            if (docs.Count == 0)
                return;

            await EnsureCollectionAsync(collection);

            // Embed in chunks so a big re-seed stays within the embedding API's batch limits,
            // but still collapses N per-doc round-trips into ~N/chunk requests.
            const int chunkSize = 100;
            for (var start = 0; start < docs.Count; start += chunkSize)
            {
                var chunk = docs.Skip(start).Take(chunkSize).ToList();

                // One embedding request for the whole chunk.
                var embeddings = await _embedding.GenerateAsync(chunk.Select(d => d.Text).ToList());

                var points = new List<PointStruct>(chunk.Count);
                for (var i = 0; i < chunk.Count; i++)
                {
                    var point = new PointStruct
                    {
                        Id = new PointId { Uuid = DeterministicGuid(chunk[i].Id) },
                        Vectors = embeddings[i].Vector.ToArray(),
                        Payload = { ["text"] = chunk[i].Text }
                    };

                    if (chunk[i].Fields is { } fields)
                    {
                        foreach (var kv in fields)
                            point.Payload[kv.Key] = kv.Value;
                    }

                    points.Add(point);
                }

                // One upsert for the whole chunk.
                await _client.UpsertAsync(collection, points);
            }
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

        public async Task<string?> GetFieldAsync(string collection, string id, string field)
        {
            if (!await _client.CollectionExistsAsync(collection))
                return null;

            var points = await _client.RetrieveAsync(
                collection,
                new PointId { Uuid = DeterministicGuid(id) },
                withPayload: true,
                withVectors: false);

            var point = points.FirstOrDefault();
            if (point is null)
                return null;

            return point.Payload.TryGetValue(field, out var value) ? value.StringValue : null;
        }

        public async Task RemoveExceptAsync(string collection, IReadOnlyCollection<string> keepIds)
        {
            if (!await _client.CollectionExistsAsync(collection))
                return;

            // The point id we store is DeterministicGuid(docId); rebuild the keep-set in that space.
            var keepGuids = new HashSet<string>(
                keepIds.Select(DeterministicGuid), StringComparer.OrdinalIgnoreCase);

            var orphans = new List<PointId>();
            PointId? offset = null;

            // Page through every point (ids only — no payload/vectors needed).
            do
            {
                var page = await _client.ScrollAsync(
                    collection,
                    limit: 256,
                    offset: offset,
                    payloadSelector: false,
                    vectorsSelector: false);

                foreach (var point in page.Result)
                {
                    var uuid = point.Id?.Uuid;
                    if (!string.IsNullOrEmpty(uuid) && !keepGuids.Contains(uuid))
                        orphans.Add(point.Id);
                }

                offset = page.NextPageOffset;
            }
            while (offset is not null);

            if (orphans.Count > 0)
                await _client.DeleteAsync(collection, orphans);
        }

        public async Task RemoveAsync(string collection, string id)
        {
            if (!await _client.CollectionExistsAsync(collection))
                return;

            await _client.DeleteAsync(collection, new PointId { Uuid = DeterministicGuid(id) });
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