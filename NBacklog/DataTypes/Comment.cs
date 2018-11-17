using System;
using System.Linq;

namespace NBacklog.DataTypes
{
    public class Comment : BacklogItem
    {
        public string Content { get; set; }
        public ChangeLog[] ChangeLogs { get; set; }
        public User Creator { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
        public Star[] Stars;
        public Notification[] Notifications { get; set; }

        internal Comment(_Comment data, BacklogClient client)
            : base(data.id)
        {
            Content = data.content;
            ChangeLogs = data.changeLog.Select(x => new ChangeLog(x)).ToArray();
            Creator = client.ItemsCache.Get(data.createdUser.id, () => new User(data.createdUser, client));
            Created = data.created;
            LastUpdated = data.updated;
            Stars = data.stars.Select(x => new Star(x, client)).ToArray();
            Notifications = data.notifications.Select(x => new Notification(x)).ToArray();
        }
    }
}
