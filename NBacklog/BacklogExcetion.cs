using NBacklog.DataTypes;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace NBacklog
{
    public class BacklogExcetion : Exception
    {
        public HttpRequestMessage Request { get; }
        public IEnumerable<Error> Errors { get; }

        public BacklogExcetion(HttpRequestMessage requext, IEnumerable<Error> errors)
        {
            Request = Request;
            Errors = errors;
        }
    }
}
