using NBacklog.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NBacklog.DataTypes
{
    public class TicketSummary : BacklogItem
    {
        public int KeyId { get; }
        public string Summary { get; }
        public string Description { get; }

        internal TicketSummary(_TicketSummary data)
            : base(data.id)
        {
            KeyId = data.keyId;
            Summary = data.summary;
            Description = data.description;
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
        public Project Project { get; }
        public int ProjectId { get; }

        public string Key { get; }
        public int KeyId { get; }
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
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public double EstimatedHours { get; set; }
        public double ActualHours { get; set; }
        public int ParentTicketId { get; set; }
        public User Creator { get; }
        public DateTime Created { get; }
        public User LastUpdater { get; }
        public DateTime LastUpdated { get; }
        public CustomFieldValue[] CustomFields { get; set; }
        public Attachment[] Attachments { get; set; }
        public SharedFile[] SharedFiles { get; private set; }
        public Star[] Stars { get; }

        public Ticket(Project project, string summary, TicketType type, Priority priority)
            : base(-1)
        {
            Project = project;
            Summary = summary;
            Type = type;
            Priority = priority;
        }

        internal Ticket(_Ticket data, Project project, BacklogClient client)
            : base(data.id)
        {
            Project = project;
            ProjectId = data.projectId;
            Key = data.issueKey;
            KeyId = data.keyId;
            Type = client.ItemsCache.Update(data.issueType?.id, () => new TicketType(data.issueType, project));
            Summary = data.summary;
            Description = data.description;
            Resolution = client.ItemsCache.Update(data.resolutions?.id, () => new Resolution(data.resolutions));
            Priority = client.ItemsCache.Update(data.priority?.id, () => new Priority(data.priority));
            Status = client.ItemsCache.Update(data.status?.id, () => new Status(data.status));
            Assignee = client.ItemsCache.Update(data.assignee?.id, () => new User(data.assignee, client));
            Categories = data.category.Select(x => client.ItemsCache.Update(x.id, () => new Category(x))).ToArray();
            Versions = data.versions.Select(x => client.ItemsCache.Update(x.id, () => new Milestone(x, project))).ToArray();
            Milestones = data.milestone.Select(x => client.ItemsCache.Update(x.id, () => new Milestone(x, project))).ToArray();
            StartDate = data.startDate?.Date ?? default;
            DueDate = data.dueDate?.Date ?? default;
            EstimatedHours = data.estimatedHours ?? default;
            ActualHours = data.actualHours ?? default;
            ParentTicketId = data.parentIssueId ?? default;
            Creator = client.ItemsCache.Update(data.createdUser?.id, () => new User(data.createdUser, client));
            Created = data.created ?? default;
            LastUpdater = client.ItemsCache.Update(data.updatedUser?.id, () => new User(data.updatedUser, client));
            LastUpdated = data.updated ?? default;
            CustomFields = data.customFields.Select(x => new CustomFieldValue(x)).ToArray();
            Attachments = data.attachments.Select(x => new Attachment(x, this)).ToArray();
            SharedFiles = data.sharedFiles.Select(x => new SharedFile(x, project)).ToArray();
            Stars = data.stars.Select(x => new Star(x, client)).ToArray();

            _client = client;
        }

        #region comment
        public async Task<BacklogResponse<int>> GetCommentCount(CommentQuery query = null)
        {
            if (Id < 0)
            {
                throw new InvalidOperationException("ticket retrieved not from the server");
            }

            query = query ?? new CommentQuery();

            var response = await _client.GetAsync($"/api/v2/issues/{Id}/comments/comment", query.Build()).ConfigureAwait(false);
            return await _client.CreateResponseAsync<int, _Count>(
                response,
                HttpStatusCode.OK,
                data => data.count).ConfigureAwait(false);
        }

        public async Task<BacklogResponse<Comment[]>> GetCommentsAsync(CommentQuery query = null)
        {
            if (Id < 0)
            {
                throw new InvalidOperationException("ticket retrieved not from the server");
            }

            query = query ?? new CommentQuery();

            var response = await _client.GetAsync($"/api/v2/issues/{Id}/comments", query.Build()).ConfigureAwait(false);
            return await _client.CreateResponseAsync<Comment[], List<_Comment>>(
                response,
                HttpStatusCode.OK,
                data => data.Select(x => new Comment(x, this)).ToArray()).ConfigureAwait(false);
        }

        public async Task<BacklogResponse<Comment>> AddCommentAsync(Comment comment, IEnumerable<User> notifiedUsers = null, IEnumerable<Attachment> attachments = null)
        {
            if (Id < 0)
            {
                throw new InvalidOperationException("ticket retrieved not from the server");
            }

            var parameters = new List<(string, object)>
            {
                ("content", comment.Content),
            };

            if (notifiedUsers != null)
            {
                foreach (var user in notifiedUsers)
                {
                    parameters.Add(("notifiedUserId[]", user.Id));
                }
            }
            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    parameters.Add(("attachmentId[]", attachment.Id));
                }
            }

            var response = await _client.PostAsync($"/api/v2/issues/{Id}/comments", parameters).ConfigureAwait(false);
            return await _client.CreateResponseAsync<Comment, _Comment>(
                response,
                HttpStatusCode.Created,
                data => new Comment(data, this)).ConfigureAwait(false);
        }

        public async Task<BacklogResponse<Comment>> UpdateCommentAsync(Comment comment)
        {
            if (Id < 0)
            {
                throw new InvalidOperationException("ticket retrieved not from the server");
            }

            var parameters = new
            {
                content = comment.Content,
            };

            var response = await _client.PostAsync($"/api/v2/issues/{Id}/comments/{comment.Id}", parameters).ConfigureAwait(false);
            return await _client.CreateResponseAsync<Comment, _Comment>(
                response,
                HttpStatusCode.OK,
                data => new Comment(data, this)).ConfigureAwait(false);
        }

        public async Task<BacklogResponse<Comment[]>> DeleteCommentAsync(Comment comment)
        {
            if (Id < 0)
            {
                throw new InvalidOperationException("ticket retrieved not from the server");
            }

            var parameters = new
            {
                content = comment.Content,
            };

            var response = await _client.DeleteAsync($"/api/v2/issues/{Id}/comments/{comment.Id}", parameters).ConfigureAwait(false);
            return await _client.CreateResponseAsync<Comment[], List<_Comment>>(
                response,
                HttpStatusCode.OK,
                data => data.Select(x => new Comment(x, this)).ToArray()).ConfigureAwait(false);
        }
        #endregion

        #region shared files
        public async Task<BacklogResponse<SharedFile[]>> LinkSharedFilesAsync(params SharedFile[] sharedFiles)
        {
            if (Id < 0)
            {
                throw new InvalidOperationException("ticket retrieved not from the server");
            }

            var parameters = new QueryParameters();
            parameters.AddRange("fileId[]", sharedFiles.Select(x => x.Id));

            var response = await _client.PostAsync($"/api/v2/issues/{Id}/sharedFiles", parameters.Build()).ConfigureAwait(false);
            var result = await _client.CreateResponseAsync<SharedFile[], List<_SharedFile>>(
                response,
                HttpStatusCode.OK,
                data => data.Select(x => _client.ItemsCache.Update(x.id, () => new SharedFile(x, Project))).ToArray()
                ).ConfigureAwait(false);

            if (result.Content.Length > 0)
            {
                SharedFiles = SharedFiles.Concat(result.Content).ToArray();
            }

            return result;
        }

        public async Task<BacklogResponse<SharedFile>> UnlinkSharedFilesAsync(SharedFile sharedFile)
        {
            if (Id < 0)
            {
                throw new InvalidOperationException("ticket retrieved not from the server");
            }

            var response = await _client.DeleteAsync($"/api/v2/issues/{Id}/sharedFiles/{sharedFile.Id}").ConfigureAwait(false);
            var result = await _client.CreateResponseAsync<SharedFile, _SharedFile>(
                response,
                HttpStatusCode.OK,
                data => _client.ItemsCache.Delete<SharedFile>(data.id)).ConfigureAwait(false);

            if (result.Content != null)
            {
                SharedFiles = SharedFiles.Except(new[] { result.Content }).ToArray();
            }

            return result;
        }
        #endregion

        internal QueryParameters ToApiParameters()
        {
            var parameters = new QueryParameters();

            if (Summary != null) parameters.Add("summary", Summary);
            if (Type != null) parameters.Add("issueTypeId", Type.Id);
            if (Priority != null) parameters.Add("priorityId", Priority.Id);
            if (Description != null) parameters.Add("description", Description);
            if (Status != null) parameters.Add("statusId", Status.Id);
            if (StartDate != default) parameters.Add("startDate", StartDate.ToString("yyyy-MM-dd"));
            if (DueDate != default) parameters.Add("dueDate", DueDate.ToString("yyyy-MM-dd"));
            if (Categories != null) parameters.AddRange("categoryId[]", Categories.Select(x => x.Id));
            if (Versions != null) parameters.AddRange("versionId[]", Versions.Select(x => x.Id));
            if (Milestones != null) parameters.AddRange("milestoneId[]", Milestones.Select(x => x.Id));
            if (Attachments != null) parameters.AddRange("attachmentId[]", Attachments.Select(x => x.Id));
            if (ParentTicketId > 0) parameters.Add("parentIssueId", ParentTicketId);
            if (Resolution != null) parameters.Add("resolutionId", Resolution.Id);
            if (EstimatedHours > 0) parameters.Add("estimatedHours", EstimatedHours);
            if (ActualHours > 0) parameters.Add("actualHours", ActualHours);
            if (Assignee != null) parameters.Add("assigneeId", Assignee.Id);

            if (CustomFields != null)
            {
                foreach (var field in CustomFields)
                {
                    parameters.Add($"customField_{field.Id}", field.ToJsonValue());
                    if (field.OtherValue != null)
                    {
                        parameters.Add($"customField_{field.Id}_otherValue", field.OtherValue);
                    }
                }
            }

            return parameters;
        }

        private BacklogClient _client;
    }
}
