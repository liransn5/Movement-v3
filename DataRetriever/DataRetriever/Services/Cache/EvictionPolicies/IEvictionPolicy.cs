namespace DataRetriever.Services.Cache.EvictionPolicies;

public interface IEvictionPolicy<TKey>
{
    void KeyAccessed(TKey key);
    void KeyAdded(TKey key);
    void KeyRemoved(TKey key);
    TKey SelectKeyForEviction();
}