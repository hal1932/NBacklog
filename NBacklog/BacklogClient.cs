using NBacklog.DataTypes;
using NBacklog.OAuth2;
using NBacklog.Rest;
using System;
using System.Collections.Generic;
using System.IO;
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
        }

        public override async Task AuthorizeAsync(OAuth2App app)
        {
            var endPoint = new OAuth2EndPoint()
            {
                BaseUri = $"https://{SpaceKey}.{Domain}/",
                AuthResource = "/OAuth2AccessRequest.action",
                QueryTokenResource = "/api/v2/oauth2/token",
            };
            await AuthorizeAsync(app, endPoint).ConfigureAwait(false);
        }

        public async Task<RestResponse> GetAsync(string resource, object parameters = null)
        {
            return await SendAsync(resource, Method.GET, parameters).ConfigureAwait(false);
        }

        public async Task<RestResponse> PutAsync(string resource, object parameters = null)
        {
            return await SendAsync(resource, Method.PUT, parameters).ConfigureAwait(false);
        }

        public async Task<RestResponse> PatchAsync(string resource, object parameters = null)
        {
            return await SendAsync(resource, Method.PATCH, parameters).ConfigureAwait(false);
        }

        public async Task<RestResponse> PostAsync(string resource, object parameters = null)
        {
            return await SendAsync(resource, Method.POST, parameters).ConfigureAwait(false);
        }

        public async Task<RestResponse> DeleteAsync(string resource, object parameters = null)
        {
            return await SendAsync(resource, Method.DELETE, parameters).ConfigureAwait(false);
        }

        public virtual async Task<RestResponse> SendAsync(string resource, Method method, object parameters = null)
        {
            var token = await GetAccessTokenAsync().ConfigureAwait(false);

            var request = new RestRequest(resource, method);
            request.SetOAuth2AccessToken(token);

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

            return await _client.SendAsync(request).ConfigureAwait(false);
        }

        public virtual async Task<RestResponse> SendFileAsync(string resource, Method method, string name, FileInfo file)
        {
            var token = await GetAccessTokenAsync().ConfigureAwait(false);

            var request = new RestRequest(resource, method, DataFormat.MultiPart);
            request.SetOAuth2AccessToken(token);

            request.AddFile(name, file.FullName, "application/octet-stream");

            return await _client.SendAsync(request).ConfigureAwait(false);
        }

        internal async Task<BacklogResponse<TApiData>> CreateResponseAsync<TApiData, TContent>(RestResponse response, HttpStatusCode successfulStatusCode, Func<TContent, TApiData> contentSelector)
        {
            if (response.StatusCode != successfulStatusCode)
            {
                Console.WriteLine(response.StatusCode);
                var content = await response.DeserializeContentAsync<_Errors>().ConfigureAwait(false);
                return new BacklogResponse<TApiData>(
                    response.StatusCode,
                    content?.errors.Select(x => new Error(x)) ?? Array.Empty<Error>());
            }
            else
            {
                var content = await response.DeserializeContentAsync<TContent>().ConfigureAwait(false);
                return new BacklogResponse<TApiData>(response.StatusCode, contentSelector(content));
            }
        }

        internal async Task<BacklogResponse<T>> CreateResponseAsync<T>(RestResponse response, HttpStatusCode successfulStatusCode, Func<byte[], T> contentSelector)
        {
            if (response.StatusCode != successfulStatusCode)
            {
                var content = await response.DeserializeContentAsync<_Errors>().ConfigureAwait(false);
                return new BacklogResponse<T>(
                    response.StatusCode,
                    content?.errors.Select(x => new Error(x)) ?? Array.Empty<Error>());
            }
            else
            {
                var content = await response.GetContentBytesAsync().ConfigureAwait(false);
                return new BacklogResponse<T>(response.StatusCode, contentSelector(content));
            }
        }

        private RestClient _client;
    }
}
