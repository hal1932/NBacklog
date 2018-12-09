using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NBacklog.DataTypes
{
    public class Comment : BacklogItem
    {
        public string Content { get; set; }
        public ChangeLog[] ChangeLogs { get;}
        public User Creator { get; }
        public DateTime Created { get; }
        public DateTime LastUpdated { get; }
        public Star[] Stars { get; }
        public Notification[] Notifications { get; private set; }

        public Comment(string content)
            : base(-1)
        {
            Content = content;
        }

        internal Comment(_Comment data, Ticket ticket)
            : base(data.id)
        {
            var client = ticket.Project.Client;

            Content = data.content;
            ChangeLogs = data.changeLog.Select(x => new ChangeLog(x)).ToArray();
            Creator = client.ItemsCache.Update(data.createdUser?.id, () => new User(data.createdUser, client));
            Created = data.created ?? default;
            LastUpdated = data.updated ?? default;
            Stars = data.stars.Select(x => new Star(x, client)).ToArray();
            Notifications = data.notifications.Select(x => new Notification(x, client)).ToArray();

            _ticket = ticket;
        }

        internal Comment(int id, string content)
            : base(id)
        {
            Content = content;
        }

        public async Task<BacklogResponse<Notification[]>> AddNotificationsAsync(IEnumerable<User> users)
        {
            var parameters = new
            {
                notifiedUserId = users?.Select(x => x.Id).ToArray(),
            };

            var client = _ticket.Project.Client;
            var response = await client.PostAsync($"/api/v2/issues/{_ticket.Id}/comments/{Id}/notifications", parameters).ConfigureAwait(false);
            var result = await client.CreateResponseAsync<Comment, _Comment>(
                response,
                HttpStatusCode.OK,
                data => new Comment(data, _ticket)).ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                return new BacklogResponse<Notification[]>(result.StatusCode, result.Errors);
            }

            Notifications = result.Content.Notifications;
            return new BacklogResponse<Notification[]>(result.StatusCode, Notifications);
        }

        private Ticket _ticket;
    }
}
