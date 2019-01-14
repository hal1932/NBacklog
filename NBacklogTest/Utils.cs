using NBacklog;
using NBacklog.OAuth2;
using Newtonsoft.Json;
using System.IO;

namespace NBacklogTest
{
    static class Utils
    {
        class _Settings
        {
            public string client_id { get; set; }
            public string client_secret { get; set; }
        }

        static BacklogClient CreateClient()
        {
            var settings = JsonConvert.DeserializeObject<_Settings>(File.ReadAllText("client.json"));

            var client = new BacklogClient("hal1932", "backlog.com");
            client.AuthorizeAsync(new OAuth2App()
            {
                ClientId = settings.client_id,
                ClientSecret = settings.client_secret,
                RedirectUri = "http://localhost:54321/",
                CredentialsCachePath = "oauth2cache.json",
            })
            .Wait();

            return client;
        }
    }
}
