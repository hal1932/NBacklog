using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace NBacklog.Rest
{
    // RestSharp がスレッドアンセーフなので、自前で簡易版を用意する
    internal class RestClient
    {
        internal static readonly JsonSerializer DefaultSerializer = new JsonSerializer
        {
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling = NullValueHandling.Include,
            DefaultValueHandling = DefaultValueHandling.Include
        };

        public RestClient(string baseUri, int maxConnectionPerServer = 256, JsonSerializer serializer = null)
        {
            _client = new HttpClient(new HttpClientHandler()
            {
                MaxConnectionsPerServer = maxConnectionPerServer,
            });

            _serializer = serializer ?? DefaultSerializer;

            _baseUri = baseUri;
        }

        public async Task<RestResponse> SendAsync(RestRequest request)
        {
            var response = await _client.SendAsync(request.Build(_baseUri)).ConfigureAwait(false);
            return new RestResponse(response, _serializer);
        }

        private HttpClient _client;
        private JsonSerializer _serializer;
        private string _baseUri;
    }
}
