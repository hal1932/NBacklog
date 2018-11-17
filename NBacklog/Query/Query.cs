using System;
using System.Collections.Generic;

namespace NBacklog.Query
{
    public abstract class Query<T>
        where T : Query<T>
    {
        protected T AddParameter(string key, object value)
        {
            _parameters.Add((key, value));
            return this as T;
        }

        protected T AddParameter(string key, DateTime value)
        {
            _parameters.Add((key, value.ToString("yyyy-MM-dd")));
            return this as T;
        }

        protected T AddParameterRange<S>(string key, IEnumerable<S> values)
        {
            foreach (var value in values)
            {
                _parameters.Add((key, value));
            }
            return this as T;
        }

        protected T AddParameterRange<S, U>(string key, IEnumerable<S> values, Func<S, U> valueSelector)
        {
            foreach (var value in values)
            {
                _parameters.Add((key, valueSelector(value)));
            }
            return this as T;
        }

        internal List<(string, object)> Build()
        {
            return _parameters;
        }

        private List<(string, object)> _parameters = new List<(string, object)>();
    }
}
