using NBacklog;
using NBacklog.OAuth2;
using NBacklog.Query;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

            //var client new TestClient("hal1932", "backlog.com");
            var client = new BacklogClient("hal1932", "backlog.com");
            await client.AuthorizeAsync(new OAuth2App()
            {
                ClientId = settings.client_id,
                ClientSecret = settings.client_secret,
                RedirectUri = "http://localhost:54321/",
                CredentialsCachePath = "oauth2cache.json",
            });

            var space = await client.GetSpaceAsync();
            var spaceNotices = await client.GetSpaceNotificationAsync();
            var spaceDisk = await client.GetSpaceDiskUsageAsync();
            var users = await client.GetUsersAsync();
            var user = await client.GetUserAsync(users.Content[0].Id);
            var myUser = await client.GetMyUserAsync();
            var statuses = await client.GetStatusTypesAsync();
            var resolutions = await client.GetResolutionTypesAsync();
            var priorities = await client.GetPriorityTypeAsync();
            var groups = await client.GetGroupsAsync();
            var projs = await client.GetProjectsAsync();

            var proj = projs.Content[0];
            var tickets = await proj.GetTicketsAsync(new TicketQuery());
            var projUses = await proj.GetUsersAsync();
            var ticketTypes = await proj.GetTicketTypesAsync();
            var categories = await proj.GetCategoriesAsync();
            var miles = await proj.GetMilestonesAsync();
            var customFields = await proj.GetCustomFieldsAsync();

            var ticket = tickets.Content[0];
            var comments = await ticket.GetCommentsAsync();

            Console.WriteLine("");
        }
    }
}
