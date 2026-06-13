using Neighborhood.Services.Application.Cache;
using StackExchange.Redis;
using System.Text.Json;

namespace Neighborhood.Services.Infrastructure.Cache
{
    public class ResponseCacheService : IResponseCacheService
    {
        private readonly IDatabase? _dataBase;
        private readonly IConnectionMultiplexer _connection;

        public ResponseCacheService(IConnectionMultiplexer? redis)
        {
            _dataBase = redis?.GetDatabase();
            _connection = redis ?? default;
        }

        public async Task CacheResponseAsync(string key, object value, TimeSpan timetoLive)
        {

            if (_dataBase is null)
                return ;

            try
            {
                var response = JsonSerializer.Serialize(value, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                await _dataBase.StringSetAsync(key, response, timetoLive);
            }
            catch
            {
                return;
            }
        }


        public async Task<string?> GetCachedResponseAsync(string key)
        {
            if (_dataBase is null)
                return null;
            try
            {

                var cachedResponse = await _dataBase.StringGetAsync(key);
                if (cachedResponse.IsNullOrEmpty) return null;
                return cachedResponse;
            }
            catch
            {
                return null;
            }
        }

        public async Task RemoveByPatternAsync(string pattern)
        {

            if (_connection is null)
                return;


            var server = _connection.GetServer(_connection.GetEndPoints().First());

            var keys = server.Keys(pattern: pattern);

            foreach (var key in keys)
            {
                await _dataBase.KeyDeleteAsync(key);
            }
        }
    }
}
