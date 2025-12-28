namespace DataRetriever.Services.Providers;
public interface IDataProvider<TKey, TValue>
{
    void SetNext(IDataProvider<TKey, TValue> next);
    Task<(bool Found, TValue Value)> GetAsync(TKey key);
    Task SaveAsync(TKey key, TValue value);
}

public abstract class DataProviderBase<TKey, TValue> : IDataProvider<TKey, TValue>
{
    protected IDataProvider<TKey, TValue>? _next;

    public void SetNext(IDataProvider<TKey, TValue> next) => _next = next;

    public abstract Task<(bool Found, TValue Value)> GetAsync(TKey key);
    public abstract Task SaveAsync(TKey key, TValue value);
}
