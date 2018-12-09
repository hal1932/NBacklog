using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace NBacklog.Query
{
    public abstract class Query<T>
        where T : Query<T>
    {
        private protected T AddParameter(string key, object value)
        {
            _parameters.Add((key, value));
            return this as T;
        }

        private protected T AddParameter(string key, DateTime value)
        {
            _parameters.Add((key, value.ToString("yyyy-MM-dd")));
            return this as T;
        }

        private protected T AddParameterRange<S>(string key, IEnumerable<S> values)
        {
            foreach (var value in values)
            {
                _parameters.Add((key, value));
            }
            return this as T;
        }

        private protected T AddParameterRange<S, U>(string key, IEnumerable<S> values, Func<S, U> valueSelector)
        {
            foreach (var value in values)
            {
                _parameters.Add((key, valueSelector(value)));
            }
            return this as T;
        }

        private protected T ReplaceParameter<S>(string key, S value)
        {
            var parameter = _parameters.FirstOrDefault(x => x.Item1 == key);
            if (parameter.Item1 == key)
            {
                _parameters.Remove(parameter);
                parameter.Item2 = value;
                _parameters.Add(parameter);
            }
            else
            {
                _parameters.Add((key, value));
            }
            return this as T;
        }

        private protected string GetEnumDesc<TEnum>(TEnum value)
            where TEnum : Enum
        {
            var attribute = typeof(TEnum).GetMember(value.ToString())[0]
                .GetCustomAttributes(typeof(DescriptionAttribute), false)[0] as DescriptionAttribute;
            return attribute?.Description;
        }

        internal List<(string, object)> Build()
        {
            return _parameters;
        }

        private List<(string, object)> _parameters = new List<(string, object)>();
    }
}
