using NBacklog.DataTypes;
using NBacklog.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NBacklog.Extensions
{
    public static class ProjectExtensions
    {
        public static async Task<Ticket[]> BatchGetTicketsAsync(this Project project, TicketQuery query = null, ErrorHandler onError = null)
        {
            query = query ?? new TicketQuery();

            var countResponse = await project.GetTicketCountAsync(query).ConfigureAwait(false);
            if (!countResponse.CanContinueBatchJobs(onError))
            {
                return Array.Empty<Ticket>();
            }

            var ticketCount = countResponse.Content;

            var tickets = new List<Ticket>();
            while (tickets.Count < ticketCount)
            {
                var count = Math.Min(ticketCount - tickets.Count, TicketQuery.MaxCount);
                query.Offset(tickets.Count).Count(count);

                var ticketsResponse = await project.GetTicketsAsync(query).ConfigureAwait(false);
                if (!ticketsResponse.CanContinueBatchJobs(onError))
                {
                    break;
                }

                tickets.AddRange(ticketsResponse.Content);
            }

            return tickets.ToArray();
        }

        public static async Task<SharedFile[]> GetSharedFilesRecursiveAsync(this Project project, string directory = "", SharedFileQuery query = null, ErrorHandler onError = null)
        {
            query = query ?? new SharedFileQuery();

            var typeNames = query.TypeNames;

            query.Count(SharedFileQuery.MaxCount);
            query.FileType(SharedFileType.Directory | SharedFileType.File);

            var files = new List<SharedFile>();

            var directories = new Queue<string>();
            directories.Enqueue(directory);

            var success = true;
            while (success && directories.Any())
            {
                var tmpFiles = new List<SharedFile>();
                var currentDir = directories.Dequeue();

                while (true)
                {
                    query.Offset(tmpFiles.Count);
                    var fileResponse = await project.GetSharedFilesAsync(currentDir, query).ConfigureAwait(false);
                    if (!fileResponse.CanContinueBatchJobs(onError))
                    {
                        success = false;
                        break;
                    }

                    tmpFiles.AddRange(fileResponse.Content);

                    foreach (var file in fileResponse.Content)
                    {
                        if (file.Type == SharedFileType.Directory)
                        {
                            directories.Enqueue(currentDir + file.Name);
                        }
                    }

                    if (fileResponse.Content.Length < SharedFileQuery.MaxCount)
                    {
                        break;
                    }
                }

                files.AddRange(tmpFiles.Where(x => typeNames.Contains(x.TypeName)));
            }

            return files.ToArray();
        }
    }
}
