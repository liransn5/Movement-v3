namespace DataRetriever.Services.Cache.EvictionPolicies;

public class NoEvictionPolicy<TKey> : IEvictionPolicy<TKey>
{
    public void KeyAccessed(TKey key)
    {
    }

    public void KeyAdded(TKey key)
    {
    }

    public void KeyRemoved(TKey key)
    {
    }

    public TKey SelectKeyForEviction()
    {
        throw new InvalidOperationException("No eviction policy. Should not be invoked");
    }
}

