using System.Diagnostics;
using StackExchange.Redis;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Core.Services.CacheService;

// public interface IRedisCacheService
// {
//     void SetCacheValue<T>(string key, T value, TimeSpan expiration);
//     Task SetCacheValueAsync<T>(string key, T value, TimeSpan expiration);
//     T GetCacheValue<T>(string key);
//     Task<T> GetCacheValueAsync<T>(string key);
// }

public class RedisCacheService : IRedisCacheService
{
    private readonly IConnectionMultiplexer _redis;
    private static readonly ActivitySource _activitySource = new("RedisCacheService");

    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public void SetCacheValue<T>(string key, T value, TimeSpan expiration)
    {
        using var activity = _activitySource.StartActivity("Redis SET", ActivityKind.Client);
        activity?.SetTag("db.system.name", "redis");
        activity?.SetTag("db.system", "redis");
        activity?.SetTag("db.operation", "SET");
        activity?.SetTag("db.redis.key", key);

        var db = _redis.GetDatabase();
        var json = JsonSerializer.Serialize(value);
        db.StringSet(key, json, expiration);
    }

    public async Task SetCacheValueAsync<T>(string key, T value, TimeSpan expiration)
    {
        using var activity = _activitySource.StartActivity("Redis SET", ActivityKind.Client);
        activity?.SetTag("db.system.name", "redis");
        activity?.SetTag("db.system", "redis");
        activity?.SetTag("db.operation", "SET");
        activity?.SetTag("db.redis.key", key);

        var db = _redis.GetDatabase();
        var json = JsonSerializer.Serialize(value);
        await db.StringSetAsync(key, json, expiration);
    }

    public T GetCacheValue<T>(string key)
    {
        using var activity = _activitySource.StartActivity("Redis GET", ActivityKind.Client);
        activity?.SetTag("db.system.name", "redis");
        activity?.SetTag("db.system", "redis");
        activity?.SetTag("db.operation", "GET");
        activity?.SetTag("db.redis.key", key);

        var db = _redis.GetDatabase();
        var json = db.StringGet(key);
        return json.HasValue ? JsonSerializer.Deserialize<T>(json) : default;
    }

    public async Task<T> GetCacheValueAsync<T>(string key)
    {
        using var activity = _activitySource.StartActivity("Redis GET", ActivityKind.Client);
        activity?.SetTag("db.system.name", "redis");
        activity?.SetTag("db.system", "redis");
        activity?.SetTag("db.operation", "GET");
        activity?.SetTag("db.redis.key", key);

        var db = _redis.GetDatabase();
        var json = await db.StringGetAsync(key);
        return json.HasValue ? JsonSerializer.Deserialize<T>(json) : default;
    }

    public bool Remove(string key)
    {
        using var activity = _activitySource.StartActivity("Redis DELETE", ActivityKind.Client);
        activity?.SetTag("db.system.name", "redis");
        activity?.SetTag("db.system", "redis");
        activity?.SetTag("db.operation", "DELETE");
        activity?.SetTag("db.redis.key", key);

        var db = _redis.GetDatabase();
        return db.KeyDelete(key);
    }

    public async Task<bool> RemoveAsync(string key)
    {
        using var activity = _activitySource.StartActivity("Redis DELETE", ActivityKind.Client);
        activity?.SetTag("db.system.name", "redis");
        activity?.SetTag("db.system", "redis");
        activity?.SetTag("db.operation", "DELETE");
        activity?.SetTag("db.redis.key", key);

        var db = _redis.GetDatabase();
        return await db.KeyDeleteAsync(key);
    }
}