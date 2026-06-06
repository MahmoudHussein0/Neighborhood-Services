using Neighborhood.Services.Application.Cache;
using StackExchange.Redis;
using System.Text.Json;

namespace Neighborhood.Services.Infrastructure.Cache
{
    public class ResponseCacheService : IResponseCacheService
    {
        private readonly IDatabase _dataBase;
        public ResponseCacheService(IConnectionMultiplexer redis)
        {
            _dataBase = redis.GetDatabase();
        }

        public async Task CacheResponseAsync(string key, object value, TimeSpan timetoLive)
        {
            var response  = JsonSerializer.Serialize(value , new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            await  _dataBase.StringSetAsync(key, response, timetoLive);
        }


        public async Task<string?> GetCachedResponseAsync(string key)
        {
            var cachedResponse  = await  _dataBase.StringGetAsync(key);
            if (cachedResponse.IsNullOrEmpty) return null;
            return cachedResponse;
        }
    }
}
