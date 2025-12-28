namespace DataRetriever.Services.Cache.EvictionPolicies;

public partial class LruEvictionPolicy<TKey> : IEvictionPolicy<TKey>
{
    private class LruNode<TKey>
    {
        public TKey Key;
        public LruNode<TKey>? Prev;
        public LruNode<TKey>? Next;

        public LruNode(TKey key)
        {
            Key = key;
        }
    }

    private class LruLinkedList<TKey>
    {
        private LruNode<TKey>? _head;
        private LruNode<TKey>? _tail;

        public void AddToTail(LruNode<TKey> node)
        {
            if (_tail == null)
            {
                _head = node;
                _tail = node;
            }
            else
            {
                _tail.Next = node;
                node.Prev = _tail;
                _tail = node;
            }
        }

        public void MoveToTail(LruNode<TKey> node)
        {
            if (_tail == node)
                return;

            Remove(node);
            AddToTail(node);
        }

        public void Remove(LruNode<TKey> node)
        {
            if (node.Prev != null)
                node.Prev.Next = node.Next;
            else
                _head = node.Next;

            if (node.Next != null)
                node.Next.Prev = node.Prev;
            else
                _tail = node.Prev;

            node.Prev = null;
            node.Next = null;
        }

        public TKey GetHeadKey()
        {
            if (_head == null)
                throw new InvalidOperationException("List is empty");

            return _head.Key;
        }
    }

}
