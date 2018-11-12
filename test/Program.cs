using NBacklog;
using NBacklog.OAuth2;
using NBacklog.Query;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().Wait();
        }

        static async Task MainAsync()
        {
            var client = new BacklogClient("hal1932", "backlog.com");
            await client.AuthorizeAsync(new OAuth2App()
            {
                ClientId = "LKkkTWIJ5gkfXMNyKxJsMLQ0DcXMtLcv",
                ClientSecret = "ZeNtBZNRBvskUDxFbO1exlLNfLpDYc6AKytZKXij7qhzAwLKyEJtn7Nbf3ulSohA",
                RedirectUri = "http://localhost:54321/",
                CredentialsCachePath = "oauth2cache.json",
            });

            //var space = await client.GetSpaceInfoAsync();
            ////var spaceNotice = await client.UpdateSpaceNotificationAsync("test notice");
            //var disk = await client.GetSpaceDiskUsageAsync();
            //var users = await client.GetUsersAsync();
            //var user = await client.GetUserAsync(users.Content[0].Id);
            //var myUser = await client.GetMyUserAsync();
            var projs = await client.GetProjectsAsync();
            //var groups = await client.GetGroupsAsync();
            var tickets = await projs.Content[0].FindTicketsAsync(new TicketQuery());

            Console.WriteLine("");
        }
    }
}
