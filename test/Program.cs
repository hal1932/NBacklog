using NBacklog;
using NBacklog.OAuth2;
using NBacklog.Query;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace test
{
    class Program
    {
        class _Settings
        {
            public string client_id { get; set; }
            public string client_secret { get; set; }
        }

        static void Main(string[] args)
        {
            MainAsync().Wait();
        }

        static async Task MainAsync()
        {
            var settings = JsonConvert.DeserializeObject<_Settings>(File.ReadAllText("client.json"));

            var client = new BacklogClient("hal1932", "backlog.com");
            await client.AuthorizeAsync(new OAuth2App()
            {
                ClientId = settings.client_id,
                ClientSecret = settings.client_secret,
                RedirectUri = "http://localhost:54321/",
                CredentialsCachePath = "oauth2cache.json",
            });

            //var space = await client.GetSpaceInfoAsync();
            ////var spaceNotice = await client.UpdateSpaceNotificationAsync("test notice");
            //var disk = await client.GetSpaceDiskUsageAsync();
            //var users = await client.GetUsersAsync();
            //var user = await client.GetUserAsync(users.Content[0].Id);
            //var myUser = await client.GetMyUserAsync();
            var notices = await client.GetSpaceNotificationAsync();
            var projs = await client.GetProjectsAsync();
            //var groups = await client.GetGroupsAsync();
            var tickets = await projs.Content[0].GetTicketsAsync(new TicketQuery());
            var comments = await tickets.Content[0].GetCommentsAsync();

            Console.WriteLine("");
        }
    }
}
