using DataRetriever.Services.Cache.CacheStorages;
using DataRetriever.Services.Cache.EvictionPolicies;

namespace DataRetriever.Services.Cache;

public interface ICache<TKey, TValue>
{
    Task<(bool Found, TValue Value)> GetAsync(TKey key);
    Task SetAsync(TKey key, TValue value);
}

public interface ISdcsCache<TKey, TValue> : ICache<TKey, TValue> { }
public interface IRedisCache<TKey, TValue> : ICache<TKey, TValue> { }

public class RedisCache<TKey, TValue> : Cache<TKey, TValue>, IRedisCache<TKey, TValue>
{
    public RedisCache(ICacheStorage<TKey, TValue> storage, IEvictionPolicy<TKey> eviction)
        : base(storage, eviction) { }
}

public class SdcsCache<TKey, TValue> : Cache<TKey, TValue>, ISdcsCache<TKey, TValue>
{
    public SdcsCache(ICacheStorage<TKey, TValue> storage, IEvictionPolicy<TKey> eviction)
        : base(storage, eviction) { }
}
