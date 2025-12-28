namespace DataRetriever.Services.Cache.EvictionPolicies;

public partial class LruEvictionPolicy<TKey> : IEvictionPolicy<TKey>
{
    private readonly Dictionary<TKey, LruNode<TKey>> _map = new();
    private readonly LruLinkedList<TKey> _list = new();
    private readonly object _lock = new();

    public void KeyAdded(TKey key)
    {
        lock (_lock)
        {
            if (_map.TryGetValue(key, out var existingNode))
            {
                _list.MoveToTail(existingNode);
            }
            else
            {
                var node = new LruNode<TKey>(key);
                _list.AddToTail(node);
                _map[key] = node;
            }
        }
    }

    public void KeyAccessed(TKey key)
    {
        lock (_lock)
        {
            if (_map.TryGetValue(key, out var node))
            {
                _list.MoveToTail(node);
            }
        }
    }

    public void KeyRemoved(TKey key)
    {
        lock (_lock)
        {
            if (_map.TryGetValue(key, out var node))
            {
                _list.Remove(node);
                _map.Remove(key);
            }
        }
    }

    public TKey SelectKeyForEviction()
    {
        lock (_lock)
        {
            return _list.GetHeadKey();
        }
    }
}
