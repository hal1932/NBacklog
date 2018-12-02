using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NBacklog.Rest
{
    public class RestResponse
    {
        public HttpStatusCode StatusCode => _response.StatusCode;

        public RestResponse(HttpResponseMessage response, JsonSerializer serializer)
        {
            _response = response;
            _serializer = serializer;
        }

        public async Task<byte[]> GetContentBytesAsync()
        {
            return await _response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
        }

        public async Task<T> DeserializeContentAsync<T>()
        {
            using (var stream = await _response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            using (var streamReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                return _serializer.Deserialize<T>(jsonReader);
            }
        }

        private HttpResponseMessage _response;
        private JsonSerializer _serializer;
    }
}
