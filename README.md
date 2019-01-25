# NBacklog

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

C# wrapper for the Nulab's Backlog API v3.

http://developer.nulab-inc.com/docs/backlog

# Usage
```csharp
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
            var settings = JsonConvert.DeserializeObject<_Settings>(File.ReadAllText("client.json"));
            var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = 3 };

            var client = new BacklogClient("nbacklog", "backlog.com");
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
            var priorities = client.GetPriorityTypesAsync().Result.Content;
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

                    var ticket = new Ticket($"summary_{i}", type, priority);
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
    }
}
```

## API Responses
```csharp
var result = await client.GetTicketAsync("TEST-00");
if (result.IsSuccess)
{
    var ticket = result.Content;
    Console.WriteLine(ticket.Key); // -> "TEST-00"
}
else
{
    Console.Error.WriteLine(result.StatusCode); // HTTP Status COde

    var errors = result.Errors;
    foreach (var error in errors)
    {
        Console.Error.WriteLine(error.Code); // -> InternalError, LicenceError, ...
        Console.Error.WriteLine(error.Message); // -> error message same as raw api response
        Console.Error.WriteLine(error.MoreInfo); // -> more info same as raw api response
    }
}
```

## Low-Level Methods

```csharp
var client = new BacklogClient("nbacklog", "backlog.com");
await client.AuthorizeAsync(...);

await client.GetAsync("/api/v2/issues"); // GET
await client.PostAsync("/api/v2/issues", new { projectId = 00000 }); // POST
await client.PatchAsync($"/api/v2/issues/00000", new { summary = "summary" }); // PATCH
await client.DeleteAsync($"/api/v2/issues/00000"); // DELETE
```

## Authentication

### OAuth2
```csharp
var client = new BacklogClient("SPACE_KEY", "DOMAIN (backlog.jp OR backlog.com)");
await client.AuthorizeAsync(new OAuth2App()
{
    ClientId = "OAUTH2_CLIENT_ID",
    ClientSecret = "OAUTH2_CLIENT_SECRET",
    RedirectUri = "OAUTH2_REDIRECT_URI",
    CredentialsCachePath = "AUTH_CACHE_FILE_PATH (for saving AccessToken and RefreshToken and Expiration-DateTime)",
});
```

## License

[MIT](LICENSE)

