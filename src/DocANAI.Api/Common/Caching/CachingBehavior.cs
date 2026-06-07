using DocANAI.Api.Infrastructure.Caching;
using MediatR;

namespace DocANAI.Api.Common.Caching;

public sealed class CachingBehavior<TRequest, TResponse>(ICacheService cacheService) 
    : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : ICacheableQuery
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var cachedResponse = await cacheService.GetAsync<TResponse>(request.CacheKey, ct);
        if (cachedResponse != null && !EqualityComparer<TResponse>.Default.Equals(cachedResponse, default!)) return cachedResponse;

        var response = await next(ct);
        
        await cacheService.SetAsync(request.CacheKey, response, request.Expiration, ct);
        return response;
    }
}