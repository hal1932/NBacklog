using NBacklog.DataTypes;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace NBacklog
{
    public struct BacklogResponse<T>
    {
        public HttpStatusCode StatusCode { get; private set; }
        public Error[] Errors { get; private set; }
        public T Content { get; private set; }

        internal static BacklogResponse<T> Create(IRestResponse response, HttpStatusCode requiredCode, T content)
        {
            var instance = new BacklogResponse<T>()
            {
                StatusCode = response.StatusCode,
                Errors = Array.Empty<Error>(),
                Content = content,
            };

            if (instance.StatusCode != requiredCode)
            {
                instance.Errors = JsonConvert.DeserializeObject<List<_Error>>(response.Content)
                    .Select(x => new Error(x))
                    .ToArray();
            }

            return instance;
        }
    }
}
