using System;
using System.Collections.Generic;

namespace NBacklog
{
    public abstract class CachableBacklogItem : BacklogItem
    {
        protected CachableBacklogItem(int id)
            : base(id)
        { }
    }

    internal class ItemsCache
    {
        public T Get<T>(T source)
            where T : CachableBacklogItem
        {
            var key = (typeof(T), source.Id);

            CachableBacklogItem item;
            if (_data.TryGetValue(key, out item))
            {
                return item as T;
            }

            _data[key] = source;
            return source;
        }

        public T Get<T>(int? id, Func<T> selector)
            where T : CachableBacklogItem
        {
            if (!id.HasValue)
            {
                return default(T);
            }

            var key = (typeof(T), id.Value);

            CachableBacklogItem item;
            if (!_data.TryGetValue(key, out item))
            {
                item = selector();
                _data[key] = item;
            }

            return item as T;
        }

        public T Update<T>(T item)
            where T : CachableBacklogItem
        {
            var key = (typeof(T), item.Id);
            _data[key] = item;
            return item;
        }

        public T Delete<T>(T item)
            where T : CachableBacklogItem
        {
            var key = (typeof(T), item.Id);

            CachableBacklogItem deleted;
            if (_data.TryGetValue(key, out deleted))
            {
                _data.Remove(key);
                return deleted as T;
            }
            return null;
        }

        private Dictionary<(Type, int), CachableBacklogItem> _data = new Dictionary<(Type, int), CachableBacklogItem>();
    }
}
