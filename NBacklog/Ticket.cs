using System;
using System.Linq;

namespace NBacklog
{
    public class Ticket : CacheItem
    {
        public Project Project { get; set; }
        public string Key { get; set; }
        public int KeyId { get; set; }
        public TicketType Type { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public ResolutionType Resolution { get; set; }
        public PriorityType Priority { get; set; }
        public StatusType Status { get; set; }
        public User Assignee { get; set; }
        public Category[] Categories { get; set; }
        public Milestone[] Versions { get; set; }
        public Milestone[] Milestones { get; set; }
        public DateTime StateDate { get; set; }
        public DateTime DueDate { get; set; }
        public int EstimatedHours { get; set; }
        public int ActualHours { get; set; }
        public int ParentTicketId { get; set; }
        public User Creator { get; set; }
        public DateTime Created { get; set; }
        public User LastUpdater { get; set; }
        public DateTime LastUpdated { get; set; }
        public Attachment[] Attachments { get; set; }
        public SharedFile[] SharedFiles { get; set; }
        public Star[] Stars { get; set; }

        internal Ticket(_Ticket data, Project project, BacklogClient client)
        {
            Id = data.id;
            Project = project;
            Key = data.issueKey;
            KeyId = data.keyId;
            Type = ItemsCache.Get(data.issueType.id, () => new TicketType(data.issueType, project));
            Summary = data.summary;
            Description = data.description;
            Resolution = ItemsCache.Get(data.resolutions?.id, () => new ResolutionType(data.resolutions));
            Priority = ItemsCache.Get(data.priority?.id, () => new PriorityType(data.priority));
            Status = ItemsCache.Get(data.status?.id, () => new StatusType(data.status));
            Assignee = ItemsCache.Get(data.assignee?.id, () => new User(data.assignee, client));
            Categories = data.category.Select(x => ItemsCache.Get(x.id, () => new Category(x))).ToArray();
            Versions = data.versions.Select(x => ItemsCache.Get(x.id, () => new Milestone(x, project))).ToArray();
            Milestones = data.milestone.Select(x => ItemsCache.Get(x.id, () => new Milestone(x, project))).ToArray();
            StateDate = data.startDate;
            DueDate = data.dueDate;
            EstimatedHours = data.estimatedHours;
            ActualHours = data.actualHours;
            ParentTicketId = data.parentIssueId;
            Creator = ItemsCache.Get(data.createdUser.id, () => new User(data.createdUser, client));
            Created = data.created;
            LastUpdater = ItemsCache.Get(data.updatedUser.id, () => new User(data.updatedUser, client));
            LastUpdated = data.updated;
            Attachments = data.attachments.Select(x => new Attachment(x)).ToArray();
            SharedFiles = data.sharedFiles.Select(x => new SharedFile(x, client)).ToArray();
            Stars = data.stars.Select(x => new Star(x, client)).ToArray();
        }
    }
}
