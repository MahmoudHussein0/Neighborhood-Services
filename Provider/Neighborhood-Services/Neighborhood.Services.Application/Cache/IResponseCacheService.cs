using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Cache
{
    public  interface IResponseCacheService
    {
        Task CacheResponseAsync(string key, object value, TimeSpan timetoLive);

        Task<string?> GetCachedResponseAsync(string key);


    }
}
