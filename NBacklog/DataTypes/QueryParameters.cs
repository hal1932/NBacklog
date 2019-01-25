using NBacklog.Query;
using System;
using System.Collections.Generic;

namespace NBacklog.DataTypes
{
    public class QueryParameters : Query<QueryParameters>
    {
        public QueryParameters Add<T>(string key, T value, bool throwOnDefaultValue = false)
        {
            if (throwOnDefaultValue && value == default)
            {
                throw new ArgumentException($"{key} must not be {value}");
            }
            return AddParameter(key, value);
        }

        public QueryParameters Replace<T>(string key, T value)
        {
            return ReplaceParameter(key, value);
        }

        public QueryParameters AddRange<T>(string key, IEnumerable<T> values)
        {
            return AddParameterRange(key, values);
        }

        public QueryParameters Remove(string key)
        {
            return RemoveParameter(key);
        }
    }
}
