using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Neighborhood.Services.Application.Cache;
using System.Text;

namespace Neighborhood.Services.API.Helper
{
    public class CacheAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLive;

        public CacheAttribute(int timeToLive)
        {
            _timeToLive = timeToLive;
        }


        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var responseService =  context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
            var key = GetCacheKeyFromRequest(context.HttpContext.Request);

            var cachedResponse = await responseService.GetCachedResponseAsync(key);

            if(cachedResponse is not null)
            {
                var result = new ContentResult()
                {
                    Content = cachedResponse,
                    StatusCode = 200 ,
                    ContentType = "application/json"
                };
                context.Result = result;
                return;
            }

           var actionExecutedContext = await next.Invoke();
              
            if( actionExecutedContext.Result  is OkObjectResult okObjectResult &&   okObjectResult.Value is not null  )
            {
               await responseService.CacheResponseAsync(key, okObjectResult.Value , TimeSpan.FromSeconds(_timeToLive) );
            }
        }

        private string GetCacheKeyFromRequest(HttpRequest request)
        {
            var cacheKey = new StringBuilder();
            cacheKey.Append(request.Path);
            foreach (var (key , value) in request.Query.OrderBy(q => q.Key)) 
            {
                cacheKey.Append($"|{key}-{value}");
            }
            return cacheKey.ToString();
        }
    }
}
