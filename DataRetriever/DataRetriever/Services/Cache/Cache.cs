namespace DataRetriever.Services.Cache;

using DataRetriever.Services.Cache.CacheStorages;
using DataRetriever.Services.Cache.EvictionPolicies;


public class Cache<TKey, TValue> : ICache<TKey, TValue>
{
    private readonly ICacheStorage<TKey, TValue> _storage;
    private readonly IEvictionPolicy<TKey> _policy;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public Cache(ICacheStorage<TKey, TValue> storage, IEvictionPolicy<TKey> policy)
    {
        _storage = storage;
        _policy = policy;
    }

    public async Task<(bool Found, TValue Value)> GetAsync(TKey key)
    {
        var result = await _storage.TryGetAsync(key);

        if (!result.Found)
            return (false, default!);

        _policy.KeyAccessed(key);
        return (true, result.Value);
    }

    /// <summary>
    /// Stores a value in the cache using a capacity-aware eviction strategy.
    /// </summary>
    /// <param name="key">The unique key associated with the cached value.</param>
    /// <param name="value">The value to store in the cache.</param>
    public async Task SetAsync(TKey key, TValue value)
    {
        await _semaphore.WaitAsync();
        try
        {
            if (await _storage.CanAcceptNewItemAsync() == false)
            {
                var evictedKey = _policy.SelectKeyForEviction();
                await _storage.RemoveAsync(evictedKey);
                _policy.KeyRemoved(evictedKey);
            }

            await _storage.SetAsync(key, value);
            _policy.KeyAdded(key);
        }
        finally
        { 
            _semaphore.Release();
        }
    }
}
