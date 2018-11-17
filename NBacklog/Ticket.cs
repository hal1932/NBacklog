using NBacklog.DataTypes;
using NBacklog.Query;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace NBacklog
{
    public class TicketType : CachableBacklogItem
    {
        public Project Project { get; set; }
        public string Name { get; set; }
        public Color Color { get; set; }
        public int DisplayOrder { get; set; }

        internal TicketType(_TicketType data, Project project)
            : base(data.id)
        {
            Project = project;
            Name = data.name;
            Color = Utils.ColorFromWebColorStr(data.color);
            DisplayOrder = data.displayOrder;
        }
    }

    public class Status : CachableBacklogItem
    {
        public string Name { get; set; }

        internal Status(_Status data)
            : base(data.id)
        {
            Name = data.name;
        }
    }

    public class Resolution : CachableBacklogItem
    {
        public string Name { get; set; }

        internal Resolution(_Resolution data)
            : base(data.id)
        {
            Name = data.name;
        }
    }

    public class Priority : CachableBacklogItem
    {
        public string Name { get; set; }

        internal Priority(_Priority data)
            : base(data.id)
        {
            Name = data.name;
        }
    }

    public class Ticket : CachableBacklogItem
    {
        public Project Project { get; set; }
        public string Key { get; set; }
        public int KeyId { get; set; }
        public TicketType Type { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public Resolution Resolution { get; set; }
        public Priority Priority { get; set; }
        public Status Status { get; set; }
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
            : base(data.id)
        {
            Project = project;
            Key = data.issueKey;
            KeyId = data.keyId;
            Type = client.ItemsCache.Get(data.issueType.id, () => new TicketType(data.issueType, project));
            Summary = data.summary;
            Description = data.description;
            Resolution = client.ItemsCache.Get(data.resolutions?.id, () => new Resolution(data.resolutions));
            Priority = client.ItemsCache.Get(data.priority?.id, () => new Priority(data.priority));
            Status = client.ItemsCache.Get(data.status?.id, () => new Status(data.status));
            Assignee = client.ItemsCache.Get(data.assignee?.id, () => new User(data.assignee, client));
            Categories = data.category.Select(x => client.ItemsCache.Get(x.id, () => new Category(x))).ToArray();
            Versions = data.versions.Select(x => client.ItemsCache.Get(x.id, () => new Milestone(x, project))).ToArray();
            Milestones = data.milestone.Select(x => client.ItemsCache.Get(x.id, () => new Milestone(x, project))).ToArray();
            StateDate = data.startDate;
            DueDate = data.dueDate;
            EstimatedHours = data.estimatedHours;
            ActualHours = data.actualHours;
            ParentTicketId = data.parentIssueId;
            Creator = client.ItemsCache.Get(data.createdUser.id, () => new User(data.createdUser, client));
            Created = data.created;
            LastUpdater = client.ItemsCache.Get(data.updatedUser.id, () => new User(data.updatedUser, client));
            LastUpdated = data.updated;
            Attachments = data.attachments.Select(x => new Attachment(x)).ToArray();
            SharedFiles = data.sharedFiles.Select(x => new SharedFile(x, client)).ToArray();
            Stars = data.stars.Select(x => new Star(x, client)).ToArray();

            _client = client;
        }

        public async Task<BacklogResponse<Comment[]>> GetCommentsAsync(CommentQuery query = null)
        {
            query = query ?? new CommentQuery();

            var response = await _client.GetAsync<List<_Comment>>($"/api/v2/issues/{Id}/comments", query.Build());
            var data = response.Data;
            return BacklogResponse<Comment[]>.Create(
                response,
                data.Select(x => new Comment(x, _client)).ToArray());
        }

        private BacklogClient _client;
    }
}
