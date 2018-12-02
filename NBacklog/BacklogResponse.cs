using NBacklog.DataTypes;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace NBacklog
{
    public class BacklogResponse<TContent>
    {
        public bool IsSuccess => Errors == null;
        public HttpStatusCode StatusCode { get; }
        public TContent Content { get; internal set; }
        public Error[] Errors { get; }

        public BacklogResponse(HttpStatusCode code, TContent content)
        {
            StatusCode = code;
            Content = content;
        }

        public BacklogResponse(HttpStatusCode code, IEnumerable<Error> errors)
        {
            StatusCode = code;
            Errors = errors.ToArray();
        }
    }
}
