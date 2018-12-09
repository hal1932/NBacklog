using System;
using System.Linq;

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
        public Notification[] Notifications { get; }

        internal Comment(_Comment data, BacklogClient client)
            : base(data.id)
        {
            Content = data.content;
            ChangeLogs = data.changeLog.Select(x => new ChangeLog(x)).ToArray();
            Creator = client.ItemsCache.Get(data.createdUser?.id, () => new User(data.createdUser, client));
            Created = data.created ?? default(DateTime);
            LastUpdated = data.updated ?? default(DateTime);
            Stars = data.stars.Select(x => new Star(x, client)).ToArray();
            Notifications = data.notifications.Select(x => new Notification(x, client)).ToArray();
            _client = client;
        }

        internal Comment(int id, string content)
            : base(id)
        {
            Content = content;
        }

        private BacklogClient _client;
    }
}
