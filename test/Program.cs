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

        static async Task Main(string[] args)
        {
            await MainAsync();
        }

        static async Task MainAsync1()
        {
            var settings = JsonConvert.DeserializeObject<_Settings>(File.ReadAllText("client.json"));
            var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = 3 };

            var client = new BacklogClient("hal1932", "backlog.com");
            await client.AuthorizeAsync(new OAuth2App()
            {
                ClientId = settings.client_id,
                ClientSecret = settings.client_secret,
                RedirectUri = "http://localhost:54321/",
                CredentialsCachePath = "oauth2cache.json",
            });

            var space = client.GetSpaceAsync().Result.Content;

            var project = client.GetProjectAsync("TEST").Result.Content;
            var types = project.GetTicketTypesAsync().Result.Content;
            var priorities = client.GetPriorityTypeAsync().Result.Content;
            var categories = project.GetCategoriesAsync().Result.Content;
            var milestones = project.GetMilestonesAsync().Result.Content;
            var users = project.GetUsersAsync().Result.Content;
            var sharedFiles = project.GetSharedFilesRecursiveAsync().Result;

            {
                var tickets = await project.BatchGetTicketsAsync();
                Console.WriteLine(tickets.Length);
                Parallel.ForEach(tickets.OrderBy(x => x.ParentTicketId == default), parallelOptions, ticket =>
                {
                    var deleteResult = project.DeleteTicketAsync(ticket).Result;
                    Console.WriteLine($"delete {ticket.Key} {deleteResult.StatusCode} {string.Join(", ", deleteResult.Errors?.Select(x => x.Message).ToArray() ?? Array.Empty<string>())}");
                });
            }

            {
                var rand = new Random(0x12345678);
                var tickets = new List<Ticket>();
                    
                Parallel.For(0, 500, parallelOptions, i =>
                {
                    var type = types[rand.Next(i) % types.Length];
                    var priority = priorities[rand.Next(i) % priorities.Length];
                    var category = categories[rand.Next(i) % categories.Length];
                    var versions = milestones[rand.Next(i) % milestones.Length];
                    var milestone = milestones[rand.Next(i) % milestones.Length];
                    var assignee = users[rand.Next(i) % users.Length];
                    var sharedFile = sharedFiles[rand.Next(i) % sharedFiles.Length];
                    var startDate = DateTime.Now - TimeSpan.FromDays(rand.Next(5));
                    var dueDate = DateTime.Now + TimeSpan.FromDays(rand.Next(10));
                    var estimatedHours = rand.NextDouble() * 30;
                    var attachSharedFile = rand.Next(3) == 0;

                    var r = rand.Next(i);
                    var parentTicketId = tickets.Any() ? tickets[r % tickets.Count].Id : -1;

                    var ticket = new Ticket(project, $"summary_{i}", type, priority);
                    if (rand.Next(3) == 0) ticket.Description = $"desc_{i}";
                    if (rand.Next(3) == 0) ticket.Assignee = assignee;
                    if (rand.Next(3) == 0) ticket.Categories = new[] { category };
                    if (rand.Next(3) == 0) ticket.Milestones = new[] { milestone };
                    if (rand.Next(3) == 0) ticket.StartDate = startDate;
                    if (rand.Next(3) == 0) ticket.DueDate = dueDate;
                    if (rand.Next(3) == 0) ticket.EstimatedHours = estimatedHours;
                    if (rand.Next(5) == 0 && tickets.Any()) ticket.ParentTicketId = parentTicketId;
                    if (rand.Next(3) == 0)
                    {
                        var attachment = space.AddAttachment(new FileInfo(@"C:\Users\yuta\Downloads\fantasy_ocean_kraken.png")).Result.Content;
                        ticket.Attachments = new[] { attachment };
                    }

                    var addResult = project.AddTicketAsync(ticket).Result;

                    // トランザクション系のエラーだったら１回だけリトライ
                    if (!addResult.IsSuccess && addResult.Errors.Any(x => x.Message.StartsWith("Deadlock")))
                    {
                        addResult = project.AddTicketAsync(ticket).Result;
                    }

                    if (!addResult.IsSuccess)
                    {
                        Console.WriteLine(string.Join(", ", addResult.Errors.Select(x => x.Message)));
                    }
                    else
                    {
                        ticket = addResult.Content;
                        Console.WriteLine($"create: {ticket.Key} {ticket.Summary}");
                        tickets.Add(ticket);

                        if (attachSharedFile)
                        {
                            var linkResult = ticket.LinkSharedFilesAsync(sharedFile).Result;

                            // トランザクション系のエラーだったら１回だけリトライ
                            if (!linkResult.IsSuccess && linkResult.Errors.Any(x => x.Message.StartsWith("Deadlock")))
                            {
                                linkResult = ticket.LinkSharedFilesAsync(sharedFiles).Result;
                            }
                        }
                    }
                });
            }

            Console.WriteLine("complete!!");
            Console.ReadKey();
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

            var space = client.GetSpaceAsync().Result.Content;
            var activities = space.GetActivitiesAsync().Result.Content;
            //var spaceNotices = await space.Content.GetNotificationAsync();
            //var spaceDisk = await space.Content.GetDiskUsageAsync();
            //var users = await client.GetUsersAsync();
            //var user = await client.GetUserAsync(users.Content[0].Id);
            //var myUser = await client.GetAuthorizedUserAsync();
            //var statuses = await client.GetStatusTypesAsync();
            //var resolutions = await client.GetResolutionTypesAsync();
            //var priorities = await client.GetPriorityTypeAsync();
            //var groups = await client.GetGroupsAsync();
            var projs = await client.GetProjectsAsync();

            var proj = projs.Content[0];
            var repos = proj.GetGitRepositoriesAsync().Result.Content;
            var pullReqs = repos[0].GetPullRequestsAsync().Result.Content;

            //var repo = proj.GetGitRepositoryAsync((activities[0].Content as GitRepositoryCreatedActivityContent).Repository).Result.Content;

            //var wiki = new Wikipage("name", "hoge", new[] { "tag1", "tag4" });
            //var tags = proj.GetWikipageTagsAsync().Result.Content;
            //wiki = proj.AddWikipageAsync(wiki).Result.Content;
            //tags = proj.GetWikipageTagsAsync().Result.Content;
            //wiki.GetContentAsync().Wait();
            //wiki.Content += "aaaaa";
            //wiki = proj.UpdateWikipageAsync(wiki).Result.Content;
            //tags = proj.GetWikipageTagsAsync().Result.Content;
            //wiki = proj.DeleteWikipageAsync(wiki).Result.Content;
            //tags = proj.GetWikipageTagsAsync().Result.Content;

            var tickets = proj.GetTicketsAsync(new TicketQuery()).Result.Content;
            //var comment = tickets[0].AddCommentAsync(new Comment("hoge", tickets[0])).Result.Content;
            //var projUses = await proj.GetUsersAsync();
            //var ticketTypes = await proj.GetTicketTypesAsync();
            //var categories = await proj.GetCategoriesAsync();
            //var miles = await proj.GetMilestonesAsync();
            //var customFields = await proj.GetCustomFieldsAsync();
            //var webhooks = proj.GetWebhooksAsync().Result.Content;
            //webhooks[0].HookUrl = "http://localhost";
            //await proj.UpdateWebhookAsync(webhooks[0]);
            //await proj.DeleteWebhookAsync(webhooks[1]);
            //webhooks[0].Name = "test2";
            //await proj.AddWebhookAsync(webhooks[0]);

            //var ticket = tickets.Content[0];
            //var comments = await ticket.GetCommentsAsync();

            Console.WriteLine("");
        }
    }
}
