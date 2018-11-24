using NBacklog.DataTypes;
using NBacklog.OAuth2;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NBacklog
{
    public partial class BacklogClient : OAuth2Client
    {
        public string SpaceKey { get; }
        public string Domain { get; }

        internal ItemsCache ItemsCache { get; } = new ItemsCache();

        public BacklogClient(string spaceKey, string domain = "backlog.jp")
        {
            SpaceKey = spaceKey;
            Domain = domain;
            _client = new RestClient($"https://{SpaceKey}.{Domain}/");
            _client.SetJsonNetSerializer(_serializer);
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

        public async Task<IRestResponse> GetAsync(string resource, object parameters = null)
        {
            return await SendAsync(resource, Method.GET, parameters).ConfigureAwait(false);
        }

        public async Task<IRestResponse> PutAsync(string resource, object parameters = null)
        {
            return await SendAsync(resource, Method.PUT, parameters).ConfigureAwait(false);
        }

        public async Task<IRestResponse> PatchAsync(string resource, object parameters = null)
        {
            return await SendAsync(resource, Method.PATCH, parameters).ConfigureAwait(false);
        }

        public async Task<IRestResponse> PostAsync(string resource, object parameters = null)
        {
            return await SendAsync(resource, Method.POST, parameters).ConfigureAwait(false);
        }

        public async Task<IRestResponse> DeleteAsync(string resource, object parameters = null)
        {
            return await SendAsync(resource, Method.DELETE, parameters).ConfigureAwait(false);
        }

        public virtual async Task<IRestResponse> SendAsync(string resource, Method method, object parameters = null)
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

                return _client.Execute(request);
            });
        }

        internal BacklogResponse<TApiData> CreateResponse<TApiData, TContent>(IRestResponse response, HttpStatusCode successfulStatusCode, Func<TContent, TApiData> contentSelector)
        {
            if (response.StatusCode != successfulStatusCode)
            {
                return new BacklogResponse<TApiData>(
                    response.StatusCode,
                    _client.Deserialize<_Errors>(response).Data.errors.Select(x => new Error(x)));
            }

            return new BacklogResponse<TApiData>(
                response.StatusCode,
                contentSelector(_client.Deserialize<TContent>(response).Data));
        }

        private RestClient _client;
        private JsonNetSerializer _serializer = new JsonNetSerializer();
    }
}
