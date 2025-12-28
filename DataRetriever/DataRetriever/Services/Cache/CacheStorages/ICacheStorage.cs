namespace DataRetriever.Services.Cache.CacheStorages;

public interface ICacheStorage<TKey, TValue>
{
    Task<(bool Found, TValue? Value)> TryGetAsync(TKey key);
    Task SetAsync(TKey key, TValue value);
    Task<bool> RemoveAsync(TKey key);
    Task<bool> CanAcceptNewItemAsync();
}