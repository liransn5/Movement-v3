using DataRetriever.Services.Cache;

namespace DataRetriever.Services.Providers;

public class SdcsProvider<TKey, TValue> : DataProviderBase<TKey, TValue>
{
    private readonly ILogger<SdcsProvider<TKey,TValue>> _logger;
    private readonly ICache<TKey, TValue> _sdcsCache;

    public SdcsProvider(ISdcsCache<TKey, TValue> sdcsCache, ILogger<SdcsProvider<TKey, TValue>> logger)
    {
        _sdcsCache = sdcsCache;
        _logger = logger;
    }

    /// <summary>
    /// Attempts to retrieve a value from the SDCS cache.
    /// If the value is not found, the request is forwarded to the next data provider in the chain.
    /// When the next provider returns a value, it is cached before returning to the caller.
    /// </summary>
    public override async Task<(bool Found, TValue Value)> GetAsync(TKey key)
    {
        var result = await _sdcsCache.GetAsync(key);

        if (result.Found)
        {
            _logger.LogInformation($"CACHE HIT SDCS id={key}");
            return (true, result.Value);
        }

        _logger.LogInformation($"CACHE MISS SDCS id={key}");

        if (_next != null)
        {
            result = await _next.GetAsync(key);
            if (result.Found)
                await _sdcsCache.SetAsync(key, result.Value);

            return result;
        }

        return (false, default);
    }

    public override async Task SaveAsync(TKey key, TValue value)
    {
        if (_next != null)
            await _next.SaveAsync(key, value);
    }
}