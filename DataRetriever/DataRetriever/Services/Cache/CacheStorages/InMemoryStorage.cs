namespace DataRetriever.Services.Cache.CacheStorages;

using System.Collections.Concurrent;
using DataRetriever.Services.Cache.Config;
using DataRetriever.Services.Cache.Config.Validators;

public class InMemoryStorage<TKey, TValue> : ICacheStorage<TKey, TValue>
{
    private readonly ConcurrentDictionary<TKey, TValue> _data = new();
    private readonly int _capacity;

    public InMemoryStorage(InMemoryStorageConfig config, IConfigValidator<InMemoryStorageConfig> validator)
    {
        validator.Validate(config);
        _capacity = config.Capacity;
    }

    public Task<(bool, TValue?)> TryGetAsync(TKey key)
    { 
        return Task.FromResult(_data.TryGetValue(key, out var val)
            ? (true, val)
            : (false, default));
    }

    public Task SetAsync(TKey key, TValue value)
    {
        _data[key] = value;
        return Task.CompletedTask;
    }

    public Task<bool> RemoveAsync(TKey key)
        => Task.FromResult(_data.TryRemove(key, out _));

    public async Task<bool> CanAcceptNewItemAsync()
    {
        return _data.Count < _capacity;
    }
}
