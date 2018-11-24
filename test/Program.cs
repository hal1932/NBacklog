using NBacklog;
using NBacklog.DataTypes;
using NBacklog.Extensions;
using NBacklog.OAuth2;
using NBacklog.Query;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            MainAsync1().Wait();
        }

        static async Task MainAsync1()
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

            var project = client.GetProjectAsync("TEST").Result.Content;
            var types = project.GetTicketTypesAsync().Result.Content;
            var priorities = client.GetPriorityTypeAsync().Result.Content;
            var categories = project.GetCategoriesAsync().Result.Content;
            var milestones = project.GetMilestonesAsync().Result.Content;
            var users = project.GetUsersAsync().Result.Content;

            {
                var tickets = await project.BatchGetTicketAsync();
                foreach (var ticket in tickets)
                {
                    //await project.DeleteTicketAsync(ticket);
                }
            }

            {
                var rand = new Random();
                var tickets = new List<Ticket>();

                for (var i = 0; i < 100; ++i)
                {
                    var type = types[rand.Next(i) % types.Length];
                    var priority = priorities[rand.Next(i) % priorities.Length];
                    var category = categories[rand.Next(i) % categories.Length];
                    var versions = milestones[rand.Next(i) % milestones.Length];
                    var milestone = milestones[rand.Next(i) % milestones.Length];
                    var assignee = users[rand.Next(i) % users.Length];

                    var ticket = new Ticket(project, $"summary_{i}", type, priority);
                    if (rand.Next(3) == 0) ticket.Description = $"desc_{i}";
                    if (rand.Next(3) == 0) ticket.Assignee = assignee;
                    if (rand.Next(3) == 0) ticket.Categories = new[] { category };
                    if (rand.Next(3) == 0) ticket.Milestones = new[] { milestone };
                    if (rand.Next(3) == 0) ticket.StartDate = DateTime.Now - TimeSpan.FromDays(rand.Next(5));
                    if (rand.Next(3) == 0) ticket.DueDate = DateTime.Now + TimeSpan.FromDays(rand.Next(10));
                    if (rand.Next(3) == 0) ticket.EstimatedHours = rand.NextDouble() * 30;
                    if (rand.Next(5) == 0 && tickets.Any()) ticket.ParentTicketId = tickets[rand.Next(i) % tickets.Count].Id;

                    var response = await project.AddTicketAsync(ticket);
                    if (response.Errors != null)
                    {
                        Console.WriteLine(string.Join(", ", response.Errors.Select(x => x.Message)));
                    }
                    else
                    {
                        ticket = response.Content;
                        Console.WriteLine($"create: {ticket.Key} {ticket.Summary}");
                        tickets.Add(ticket);
                    }
                }
            }

            Console.WriteLine("");
        }

        static async Task MainAsync()
        {
            var settings = JsonConvert.DeserializeObject<_Settings>(File.ReadAllText("client.json"));

            //var client = new TestClient("hal1932", "backlog.com");
            var client = new BacklogClient("hal1932", "backlog.com");
            await client.AuthorizeAsync(new OAuth2App()
            {
                ClientId = settings.client_id,
                ClientSecret = settings.client_secret,
                RedirectUri = "http://localhost:54321/",
                CredentialsCachePath = "oauth2cache.json",
            });

            var space = await client.GetSpaceAsync();
            var activities = await space.Content.GetActivitiesAsync();
            var spaceNotices = await space.Content.GetNotificationAsync();
            var spaceDisk = await space.Content.GetDiskUsageAsync();
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
