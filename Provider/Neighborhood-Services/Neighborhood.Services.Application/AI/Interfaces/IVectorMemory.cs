using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.AI.Interfaces
{
    public interface IVectorMemory
    {
        Task UpsertAsync(string collection, string id, string text, object? metadata = null);
        Task<IEnumerable<string>> SearchAsync(string collection, string query, int topK = 3);
    }
}
