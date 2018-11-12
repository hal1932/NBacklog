using System;
using System.Collections.Generic;

namespace NBacklog
{
    public class CacheItem
    {
        public int Id { get; protected set; }
    }

    internal static class ItemsCache
    {
        public static T Get<T>(T source)
            where T : CacheItem
        {
            var key = (typeof(T), source.Id);

            CacheItem item;
            if (_data.TryGetValue(key, out item))
            {
                return item as T;
            }

            _data[key] = source;
            return source;
        }

        public static T Get<T>(int? id, Func<T> selector)
            where T : CacheItem
        {
            if (!id.HasValue)
            {
                return default(T);
            }

            var key = (typeof(T), id.Value);

            CacheItem item;
            if (!_data.TryGetValue(key, out item))
            {
                item = selector();
                _data[key] = item;
            }

            return item as T;
        }

        public static T Update<T>(T item)
            where T : CacheItem
        {
            var key = (typeof(T), item.Id);
            _data[key] = item;
            return item;
        }

        public static T Delete<T>(T item)
            where T : CacheItem
        {
            var key = (typeof(T), item.Id);

            CacheItem deleted;
            if (_data.TryGetValue(key, out deleted))
            {
                _data.Remove(key);
                return deleted as T;
            }
            return null;
        }

        private static Dictionary<(Type, int), CacheItem> _data = new Dictionary<(Type, int), CacheItem>();
    }
}
