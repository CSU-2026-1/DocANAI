using System.Text.Json;
using DocANAI.Contracts.DTOs.AIModels;
using StackExchange.Redis;

namespace DocANAI.Api.Infrastructure.Caching;

public sealed class CacheService(IConnectionMultiplexer redis) : ICacheService
{
    private readonly IDatabase _db = redis.GetDatabase();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        Converters = { new ResultConverter<IReadOnlyList<AIModelDto>, string>()},
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
    
    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var value = await _db.StringGetAsync(key);
        if (!value.HasValue) return default;
        string jsonString = value.ToString(); 
        if (string.IsNullOrEmpty(jsonString)) return default;

        return JsonSerializer.Deserialize<T>(jsonString, JsonOptions);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(value, JsonOptions);
        
        if (expiry.HasValue)
        {
            await _db.StringSetAsync(key, json, expiry.Value);
        }
        else
        {
            await _db.StringSetAsync(key, json);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default) => 
        await _db.KeyDeleteAsync(key);
}