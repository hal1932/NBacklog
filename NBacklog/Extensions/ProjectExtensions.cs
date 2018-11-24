using NBacklog.DataTypes;
using NBacklog.Query;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NBacklog.Extensions
{
    public static class ProjectExtensions
    {
        public static async Task<Ticket[]> BatchGetTicketAsync(this Project project, TicketQuery query = null, ErrorHandler onError = null)
        {
            query = query ?? new TicketQuery();

            var countResponse = await project.GetTicketCountAsync(query);
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

                var ticketsResponse = await project.GetTicketsAsync(query);
                if (!ticketsResponse.CanContinueBatchJobs(onError))
                {
                    break;
                }

                tickets.AddRange(ticketsResponse.Content);
            }

            return tickets.ToArray();
        }
    }
}
