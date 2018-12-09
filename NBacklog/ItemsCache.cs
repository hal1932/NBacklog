using System;
using System.Collections.Generic;
using System.Linq;

namespace NBacklog
{
    public abstract class CachableBacklogItem : BacklogItem
    {
        private protected CachableBacklogItem(int id)
            : base(id)
        { }
    }

    internal class ItemsCache
    {
        public bool TryGetValue<T>(int id, out T value)
            where T : CachableBacklogItem
        {
            if (_data.TryGetValue(typeof(T), out var items))
            {
                if (items.TryGetValue(id, out var item))
                {
                    value = item as T;
                    return true;
                }
            }

            value = default;
            return false;
        }

        public bool TryGetValues<T>(out T[] values)
            where T : CachableBacklogItem
        {
            if (_data.TryGetValue(typeof(T), out var items))
            {
                values = items.Values.Cast<T>().ToArray();
                return true;
            }

            values = Array.Empty<T>();
            return false;
        }

        public T Update<T>(int? id, Func<T> selector)
            where T : CachableBacklogItem
        {
            if (!id.HasValue)
            {
                return default;
            }

            var item = selector();
            lock (_dataLockObj)
            {
                if (!_data.TryGetValue(typeof(T), out var items))
                {
                    items = new Dictionary<int, CachableBacklogItem>();
                    _data[typeof(T)] = items;
                }
                items[id.Value] = item;
            }

            return item as T;
        }

        public T Delete<T>(int? id)
            where T : CachableBacklogItem
        {
            if (!id.HasValue)
            {
                return default;
            }

            CachableBacklogItem deleted;
            lock (_dataLockObj)
            {
                if (!_data.TryGetValue(typeof(T), out var items))
                {
                    return default;
                }

                if (!items.TryGetValue(id.Value, out var item))
                {
                    return default;
                }

                deleted = item;
                items.Remove(id.Value);
            }

            return deleted as T;
        }

        private Dictionary<Type, Dictionary<int, CachableBacklogItem>> _data = new Dictionary<Type, Dictionary<int, CachableBacklogItem>>();
        private object _dataLockObj = new object();
    }
}
