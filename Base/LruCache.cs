using System.Collections.Generic;

namespace MobiFlight.Base
{
    public class LRUCache<TKey, TValue>
    {
        private readonly int capacity;
        private readonly Dictionary<TKey, LinkedListNode<CacheItem>> cacheMap;
        private readonly LinkedList<CacheItem> lruList;

        public LRUCache(int capacity)
        {
            this.capacity = capacity;
            this.cacheMap = new Dictionary<TKey, LinkedListNode<CacheItem>>(capacity);
            this.lruList = new LinkedList<CacheItem>();
        }

        public TValue this[TKey key]
        {
            get => Get(key);
            set => Add(key, value);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (cacheMap.TryGetValue(key, out var node))
            {
                value = node.Value.Value;
                lruList.Remove(node);
                lruList.AddLast(node);
                return true;
            }

            value = default;
            return false;
        }

        public void Add(TKey key, TValue value)
        {
            if (cacheMap.TryGetValue(key, out var node))
            {
                lruList.Remove(node);
                cacheMap.Remove(key);
            }
            else if (cacheMap.Count >= capacity)
            {
                var oldest = lruList.First;
                lruList.RemoveFirst();
                cacheMap.Remove(oldest.Value.Key);
            }

            var cacheItem = new CacheItem(key, value);
            var newNode = new LinkedListNode<CacheItem>(cacheItem);
            lruList.AddLast(newNode);
            cacheMap[key] = newNode;
        }

        private TValue Get(TKey key)
        {
            if (cacheMap.TryGetValue(key, out var node))
            {
                lruList.Remove(node);
                lruList.AddLast(node);
                return node.Value.Value;
            }

            throw new KeyNotFoundException();
        }

        private class CacheItem
        {
            public TKey Key { get; }
            public TValue Value { get; }

            public CacheItem(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }
        }
    }
}
