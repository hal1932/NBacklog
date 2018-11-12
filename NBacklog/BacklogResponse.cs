using RestSharp;
using System.Net;

namespace NBacklog
{
    public struct BacklogResponse<T>
    {
        public HttpStatusCode StatusCode;
        public string ErrorContent;
        public T Content;

        internal static BacklogResponse<T> Create(IRestResponse response, T content)
        {
            var instance = new BacklogResponse<T>()
            {
                StatusCode = response.StatusCode,
                Content = content,
            };

            if (instance.StatusCode != HttpStatusCode.OK)
            {
                instance.ErrorContent = response.Content;
            }

            return instance;
        }
    }
}
