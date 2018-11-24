using NBacklog;
using NBacklog.OAuth2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace test
{
    class TestResponse : IRestResponse
    {
        public IRestRequest Request { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string ContentType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public long ContentLength { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string ContentEncoding { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Content { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public bool IsSuccessful => throw new NotImplementedException();

        public string StatusDescription { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public byte[] RawBytes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Uri ResponseUri { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Server { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IList<RestResponseCookie> Cookies => throw new NotImplementedException();

        public IList<Parameter> Headers => throw new NotImplementedException();

        public ResponseStatus ResponseStatus { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string ErrorMessage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Exception ErrorException { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Version ProtocolVersion { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public TestResponse(HttpStatusCode code, string content)
        {
            StatusCode = code;
            Content = content;
        }
    }

    class TestClient : BacklogClient
    {
        public TestClient(string spaceKey, string domain = "backlog.jp")
            : base(spaceKey, domain)
        {
            var getData = (JsonConvert.DeserializeObject(File.ReadAllText("testdata.json")) as JObject)["get"] as JObject;
            foreach (var item in getData)
            {
                _dataPaths.Add((new Regex($"^{item.Key}$"), Path.Combine("data", item.Value.Value<string>())));
            }
        }

        public override async Task AuthorizeAsync(OAuth2App app)
        {
            await Task.Delay(0);
        }

        public override async Task<IRestResponse> SendAsync(string resource, Method method, object parameters = null)
        {
            await Task.Delay(0);

            string content = null;
            if (method == Method.GET)
            {
                foreach ((Regex reg, string filePath) in _dataPaths)
                {
                    if (reg.IsMatch(resource))
                    {
                        content = File.ReadAllText(filePath);
                        break;
                    }
                }
            }

            if (content == null)
            {
                throw new Exception(resource);
            }

            return new TestResponse(HttpStatusCode.OK, content);
        }

        private List<(Regex, string)> _dataPaths = new List<(Regex, string)>();
    }
}
