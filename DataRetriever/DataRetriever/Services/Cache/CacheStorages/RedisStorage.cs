namespace DataRetriever.Services.Cache.CacheStorages;

using DataRetriever.Services.Cache.Config;
using StackExchange.Redis;
using System.Text.Json;

public class RedisStorage<TKey, TValue> : ICacheStorage<TKey, TValue>
{
    private readonly IDatabase _db;
    private readonly TimeSpan _ttl;
    private readonly string _keyPrefix;

    public RedisStorage(IConnectionMultiplexer redis, RedisStorageConfig config)
    {
        _db = redis.GetDatabase();
        _ttl = TimeSpan.FromMinutes(config.TtlMinutes);
        _keyPrefix = config.KeyPrefix;
    }

    public async Task<(bool Found, TValue? Value)> TryGetAsync(TKey key)
    {
        var val = await _db.StringGetAsync(ComposeKey(key));
        if (!val.HasValue)
            return (false, default);

        return (true, JsonSerializer.Deserialize<TValue>(val));
    }

    public async Task SetAsync(TKey key, TValue value)
    {
        var json = JsonSerializer.Serialize(value);
        await _db.StringSetAsync(ComposeKey(key), json, _ttl);
    }

    public Task<bool> RemoveAsync(TKey key)
        => _db.KeyDeleteAsync(ComposeKey(key));

    public Task<bool> CanAcceptNewItemAsync() =>
        Task.FromResult(true);

    private string ComposeKey(TKey key) => $"{_keyPrefix}:{key}";
}
