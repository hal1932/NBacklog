using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace NBacklog
{
    public class TicketType : CacheItem
    {
        public Project Project;
        public string Name;
        public Color Color;
        public int DisplayOrder;

        internal TicketType(_TicketType data, Project project)
        {
            Id = data.id;
            Project = project;
            Name = data.name;
            Color = Utils.ColorFromWebColorStr(data.color);
            DisplayOrder = data.displayOrder;
        }
    }

    public class Category : CacheItem
    {
        public string Name;
        public int DisplayOrder;

        internal Category(_Category data)
        {
            Id = data.id;
            Name = data.name;
            DisplayOrder = data.displayOrder;
        }
    }

    public class Milestone : CacheItem
    {
        public Project Project;
        public string Name;
        public string Description;
        public DateTime StartDate;
        public DateTime ReleaseDueDate;
        public bool IsArchived;
        public int DisplayOrder;

        internal Milestone(_Milestone data, Project project)
        {
            Id = data.id;
            Project = project;
            Name = data.name;
            Description = data.description;
            StartDate = data.startDate;
            ReleaseDueDate = data.releaseDueDate;
            IsArchived = data.archived;
            DisplayOrder = data.displayOrder;
        }
    }

    public class CustomField : CacheItem
    {
    }

    public class Attachment
    {
        public int Id;
        public string Name;
        public int Size;

        internal Attachment(_Attachment data)
        {
            Id = data.id;
            Name = data.name;
            Size = data.size;
        }
    }

    public class SharedFile
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Dir { get; set; }
        public string Name { get; set; }
        public int Size { get; set; }
        public User Creator { get; set; }
        public DateTime Created { get; set; }
        public User LastUpdater { get; set; }
        public DateTime LastUpdated { get; set; }

        internal SharedFile(_Sharedfile data, BacklogClient client)
        {
            Id = data.id;
            Type = data.type;
            Dir = data.dir;
            Name = data.name;
            Size = data.size;
            Creator = ItemsCache.Get(data.createdUser.id, () => new User(data.createdUser, client));
            Created = data.created;
            LastUpdater = ItemsCache.Get(data.updatedUser.id, () => new User(data.updatedUser, client));
            LastUpdated = data.updated;
        }
    }

    public class Star
    {
        public int Id { get; set; }
        public object Comment { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public User Creator { get; set; }
        public DateTime Created { get; set; }

        internal Star(_Star data, BacklogClient client)
        {
            Id = data.id;
            Comment = data.comment;
            Url = data.url;
            Title = data.title;
            Creator = ItemsCache.Get(data.presenter.id, () => new User(data.presenter, client));
            Created = data.created;
        }
    }

    public class Group : CacheItem
    {
        public string Name;
        public User[] Members;
        public int DisplayOrder;
        public User Creator;
        public DateTime Created;
        public User LastUpdater;
        public DateTime LastUpdated;

        internal Group(_Group data, BacklogClient _)
        {
            Id = data.id;
            Name = data.name;
            Members = data.members.Select(x => ItemsCache.Get(x.id, () => new User(x, _))).ToArray();
            DisplayOrder = data.displayOrder;
            Creator = ItemsCache.Get(data.createdUser.id, () => new User(data.createdUser, _));
            Created = data.created;
            LastUpdater = ItemsCache.Get(data.updatedUser.id, () => new User(data.updatedUser, _));
            LastUpdated = data.updated;
        }
    }

    public class StatusType : CacheItem
    {
        public string Name;

        internal StatusType(_StatusType data)
        {
            Id = data.id;
            Name = data.name;
        }
    }

    public class ResolutionType : CacheItem
    {
        public string Name;

        internal ResolutionType(_ResolutionType data)
        {
            Id = data.id;
            Name = data.name;
        }
    }

    public class PriorityType : CacheItem
    {
        public string Name;

        internal PriorityType(_PriorityType data)
        {
            Id = data.id;
            Name = data.name;
        }
    }

    public class Space
    {
        public string Key;
        public string Name;
        public int OwnerId;
        public string Language;
        public string TimeZone;
        public string ReportSendTime;
        public string TextFormattingRule;
        public DateTime Created;
        public DateTime Updated;
    }

    public class SpaceNotification
    {
        public string Content;
        public DateTime Updated;
    }

    public class SpaceDiskUsage
    {
        public int Capacity;
        public int Issue;
        public int Wiki;
        public int File;
        public int Subversion;
        public int Git;
        public SpaceDiskUsageDetail[] Details;
    }

    public class SpaceDiskUsageDetail
    {
        public int ProjectId;
        public int Issue;
        public int Wiki;
        public int File;
        public int Subversion;
        public int Git;
    }
}
