using NBacklog.OAuth2;
using NBacklog.Query;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NBacklog
{
    public partial class BacklogClient : OAuth2Client
    {
        public string SpaceKey { get; }
        public string Domain { get; }

        public BacklogClient(string spaceKey, string domain = "backlog.jp")
        {
            SpaceKey = spaceKey;
            Domain = domain;
            _client = new RestClient($"https://{SpaceKey}.{Domain}/");
        }

        public override async Task AuthorizeAsync(OAuth2App app)
        {
            var endPoint = new OAuth2EndPoint()
            {
                BaseUri = $"https://{SpaceKey}.{Domain}/",
                AuthResource = "/OAuth2AccessRequest.action",
                QueryTokenResource = "/api/v2/oauth2/token",
            };
            await AuthorizeAsync(app, endPoint);
        }

        public async Task<IRestResponse<T>> GetAsync<T>(string resource, object parameters = null)
            where T: new()
        {
            return await SendAsync<T>(resource, Method.GET, parameters).ConfigureAwait(false);
        }

        public async Task<IRestResponse<T>> PutAsync<T>(string resource, object parameters = null)
            where T : new()
        {
            return await SendAsync<T>(resource, Method.PUT, parameters).ConfigureAwait(false);
        }

        public async Task<IRestResponse<T>> PatchAsync<T>(string resource, object parameters = null)
            where T : new()
        {
            return await SendAsync<T>(resource, Method.PATCH, parameters).ConfigureAwait(false);
        }

        public async Task<IRestResponse<T>> PostAsync<T>(string resource, object parameters = null)
            where T : new()
        {
            return await SendAsync<T>(resource, Method.POST, parameters).ConfigureAwait(false);
        }

        public async Task<IRestResponse<T>> DeleteAsync<T>(string resource, object parameters = null)
            where T : new()
        {
            return await SendAsync<T>(resource, Method.DELETE, parameters).ConfigureAwait(false);
        }

        public async Task<IRestResponse<T>> SendAsync<T>(string resource, Method method, object parameters = null)
            where T : new()
        {
            var token = await GetAccessTokenAsync();

            return await Task.Factory.StartNew(() =>
            {
                var request = new RestRequest(resource, method, DataFormat.Json);
                request.SetJsonNetSerializer(_serializer);
                request.AddHeader("Authorization", $"Bearer {token}");

                if (parameters != null)
                {
                    if (parameters is List<(string, object)>)
                    {
                        foreach ((string key, object value) in parameters as List<(string, object)>)
                        {
                            request.AddParameter(key, value);
                        }
                    }
                    else
                    {
                        foreach (var prop in parameters.GetType().GetProperties())
                        {
                            request.AddParameter(prop.Name, prop.GetValue(parameters));
                        }
                    }
                }

                return _client.Execute<T>(request);
            });
        }

        private RestClient _client;
        private JsonNetSerializer _serializer = new JsonNetSerializer();
    }
}
