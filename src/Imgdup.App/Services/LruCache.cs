namespace Imgdup.App.Services;

/// <summary>Thread-safe LRU cache with bounded capacity. Evicts the least-recently-used entry on overflow.</summary>
internal sealed class LruCache<TKey, TValue> where TKey : notnull
{
    private readonly int _capacity;
    private readonly Dictionary<TKey, LinkedListNode<(TKey Key, TValue Value)>> _map;
    private readonly LinkedList<(TKey Key, TValue Value)> _order = new();
    private readonly object _lock = new();

    public LruCache(int capacity)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(capacity, 1);
        _capacity = capacity;
        _map = new Dictionary<TKey, LinkedListNode<(TKey, TValue)>>(capacity + 1);
    }

    public bool TryGet(TKey key, out TValue value)
    {
        lock (_lock)
        {
            if (_map.TryGetValue(key, out var node))
            {
                _order.Remove(node);
                _order.AddFirst(node);
                value = node.Value.Value;
                return true;
            }
        }
        value = default!;
        return false;
    }

    /// <summary>Adds a key/value pair. Returns the evicted value when the cache was full, otherwise null/default.</summary>
    public TValue? Add(TKey key, TValue value)
    {
        lock (_lock)
        {
            if (_map.TryGetValue(key, out var existing))
            {
                _order.Remove(existing);
                _order.AddFirst(existing);
                return default;
            }

            var node = _order.AddFirst((key, value));
            _map[key] = node;

            if (_map.Count <= _capacity)
                return default;

            var lru = _order.Last!;
            _order.RemoveLast();
            _map.Remove(lru.Value.Key);
            return lru.Value.Value;
        }
    }
}
