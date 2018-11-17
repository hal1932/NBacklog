using System;

namespace NBacklog.DataTypes
{
    public class Notification : BacklogItem
    {
        public string Content { get; set; }
        public DateTime Updated { get; set; }

        internal Notification(_Notification data)
            : base(-1)
        {
            Content = data.content;
            Updated = data.updated;
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
}
