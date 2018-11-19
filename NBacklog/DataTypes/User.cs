﻿namespace NBacklog.DataTypes
{
    public enum UserRole
    {
        Admin = 1,
        User = 2,
        Reporter = 3,
        Viewer = 4,
        GuestReporter = 5,
        GuestViewer = 6,
    }

    public class User : CachableBacklogItem
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public UserRole Role { get; set; }
        public string Language { get; set; }
        public string MailAddress { get; set; }

        internal User(_User data)
            : base(data.id)
        {
            UserId = data.userId;
            Name = data.name;
            Role = (UserRole)data.roleType;
            Language = data.lang;
            MailAddress = data.mailAddress;
        }
    }
}
