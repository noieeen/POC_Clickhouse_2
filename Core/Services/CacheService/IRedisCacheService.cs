namespace Core.Services.CacheService;

public interface IRedisCacheService
{
    void SetCacheValue<T>(string key, T value, TimeSpan expiration);
    Task SetCacheValueAsync<T>(string key, T value, TimeSpan expiration);
    T GetCacheValue<T>(string key);
    Task<T> GetCacheValueAsync<T>(string key);
    bool Remove(string key);
    Task<bool> RemoveAsync(string key);
}