using System;
using System.Collections.Generic;

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
            //return _data.TryGetValue((typeof(T), id), out value);
            if (_data.TryGetValue((typeof(T), id), out var item))
            {
                value = item as T;
                return true;
            }

            value = default;
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
                _data[(typeof(T), id.Value)] = item;
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

            var key = (typeof(T), id.Value);

            CachableBacklogItem deleted;
            lock (_dataLockObj)
            {
                if (_data.TryGetValue(key, out deleted))
                {
                    _data.Remove(key);
                }
            }
            return deleted as T;
        }

        private Dictionary<(Type, int), CachableBacklogItem> _data = new Dictionary<(Type, int), CachableBacklogItem>();
        private object _dataLockObj = new object();
    }
}
