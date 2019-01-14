using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBacklog.OAuth2;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NBacklog.Tests
{
    static class Utils
    {
        class _Settings
        {
            public string client_id { get; set; }
            public string client_secret { get; set; }
        }

        public static BacklogClient CreateTestClient()
        {
            var settings = JsonConvert.DeserializeObject<_Settings>(File.ReadAllText("client.json"));
            var config = new BacklogClientConfig()
            {
                ThrowOnClientError = true,
            };

            var client = new BacklogClient("hal1932", "backlog.com", config);
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

        public static T SetPrivateProperty<T>(T obj, string propertyName, object value)
        {
            var privateObj = new PrivateObject(obj);
            privateObj.SetProperty(propertyName, value);
            return obj;
        }
    }
}
