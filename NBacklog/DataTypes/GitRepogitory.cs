using NBacklog.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NBacklog.DataTypes
{
    public class GitRepoSummary : BacklogItem
    {
        public Project Project { get; }
        public string Name { get; }
        public string Description { get; }

        internal GitRepoSummary(_GitRepoSummary data, Project project)
            : base(data.id)
        {
            Project = project;
            Name = data.name;
            Description = data.description;
        }
    }

    public class GitRepository : BacklogItem
    {
        public Project Project { get; }
        public int ProjectId { get; }
        public string Name { get; }
        public string Description { get; }
        public string HookUrl { get; }
        public string HttpUrl { get; }
        public string SshUrl { get; }
        public int DisplayOrder { get; }
        public DateTime Pushed { get; }
        public User Creator { get; }
        public DateTime Created { get; }
        public User Updater { get; }
        public DateTime Updated { get; }

        internal GitRepository(_GitRepository data, Project project)
            : base(data.id)
        {
            var client = project.Client;

            ProjectId = data.projectId;
            Name = data.name;
            Description = data.description;
            HookUrl = data.hookUrl;
            HttpUrl = data.httpUrl;
            SshUrl = data.sshUrl;
            DisplayOrder = data.displayOrder;
            Pushed = data.pushedAt ?? default;
            Creator = client.ItemsCache.Update(data.createdUser?.id, () => new User(data.createdUser, client));
            Created = data.created ?? default;
            Updater = client.ItemsCache.Update(data.updater?.id, () => new User(data.updater, client));
            Updated = data.updated ?? default;

            Project = project;
        }

        public async Task<BacklogResponse<int>> GetPullRequestCountAsync(PullRequestQuery query = null)
        {
            query = query ?? new PullRequestQuery();

            var client = Project.Client;
            var response = await client.GetAsync($"/api/v2/projects/{Project.Id}/git/repositories/{Id}/pullRequests/count", query.Build()).ConfigureAwait(false);
            return await client.CreateResponseAsync<int, _Count>(
                response,
                HttpStatusCode.OK,
                data => data.count).ConfigureAwait(false);
        }

        public async Task<BacklogResponse<PullRequest[]>> GetPullRequestsAsync(PullRequestQuery query = null)
        {
            query = query ?? new PullRequestQuery();

            var client = Project.Client;
            var response = await client.GetAsync($"/api/v2/projects/{Project.Id}/git/repositories/{Id}/pullRequests", query.Build()).ConfigureAwait(false);
            return await client.CreateResponseAsync<PullRequest[], List<_PullRequest>>(
                response,
                HttpStatusCode.OK,
                data => data.Select(x => new PullRequest(x, this)).ToArray()).ConfigureAwait(false);
        }

        public async Task<BacklogResponse<PullRequest>> AddPullRequestAsync(PullRequest pullRequest, IEnumerable<User> notifiedUsers = null, IEnumerable<Attachment> attachments = null)
        {
            var parameters = new List<(string, object)>()
            {
                ("sumary", pullRequest.Summary),
                ("description", pullRequest.Description),
                ("base", pullRequest.Base),
                ("branch", pullRequest.Branch),
            };

            if (pullRequest.TicketId != default)
            {
                parameters.Add(("issueId", pullRequest.TicketId));
            }
            if (pullRequest.Assignee != default)
            {
                parameters.Add(("assigneeId", pullRequest.Assignee.Id));
            }
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

            var client = Project.Client;
            var response = await client.PostAsync($"/api/v2/projects/{Project.Id}/git/repositories/{Id}/pullRequests", parameters).ConfigureAwait(false);
            return await client.CreateResponseAsync<PullRequest, _PullRequest>(
                response,
                HttpStatusCode.OK,
                data => new PullRequest(data, this)).ConfigureAwait(false);
        }
    }
}
