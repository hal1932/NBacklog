using System;

namespace NBacklog.DataTypes
{
    public enum NotificationReason
    {
        Undefined = -1,
        Assigned = 1,
        Commented = 2,
        IssueCreated = 3,
        IssueUpdated = 4,
        FileAttached = 5,
        ProjectUserAdded = 6,
        Other = 9,
        PullRequestAssigned = 10,
        PullRequestCommented = 11,
        PullRequestAdded = 12,
        PullRequestUpdated = 13,
    }

    public class Notification : BacklogItem
    {
        public bool IsAlreadyRead { get; }
        public NotificationReason Reason { get; }
        public User User { get; }
        public bool IsResourceAlreadyRead { get; }

        internal Notification(_Notification data, BacklogClient client)
            : base(data.id)
        {
            IsAlreadyRead = data.alreadyRead;
            Reason = (NotificationReason)data.reason;
            User = client.ItemsCache.Get(data.user?.id, () => new User(data.user, client));
            IsResourceAlreadyRead = data.resourceAlreadyRead;
        }
    }

    public class NotificationInfo : BacklogItem
    {
        public string Type { get; set; }

        internal NotificationInfo(_NotificationInfo data)
            : base(-1)
        {
            Type = data.type;
        }
    }

    public class SpaceNotification : BacklogItem
    {
        public string Content { get; set; }
        public DateTime Updated { get; set; }

        internal SpaceNotification(_SpaceNotification data)
            : base(-1)
        {
            Content = data.content;
            Updated = data.updated ?? default(DateTime);
        }
    }
}
