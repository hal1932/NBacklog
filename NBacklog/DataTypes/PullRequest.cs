using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBacklog.DataTypes
{
    public class PullRequestSummary : BacklogItem
    {
        public GitRepoSummary Repository { get; }
        public int Number { get; }
        public string Summary { get; }
        public string Description { get; }

        internal PullRequestSummary(_PullRequestSummary data, GitRepoSummary repo)
            : base(data.id)
        {
            Repository = repo;
            Number = data.number;
            Summary = data.summary;
            Description = data.description;
        }
    }

    public class PullRequestStatus : BacklogItem
    {
        public string Name { get; }

        internal PullRequestStatus(_PullRequestStatus data)
            : base(data.id)
        {
            Name = data.name;
        }
    }

    public class PullRequest : BacklogItem
    {
        public GitRepository Repository { get; set; }

        public int Number { get; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string Base { get; set; }
        public string Branch { get; set; }
        public PullRequestStatus Status { get; }
        public User Assignee { get; set; }
        public int TicketId { get; set; }
        public string BaseCommit { get; }
        public string BranchCommit { get; }
        public DateTime Closed { get; }
        public DateTime Merged { get; }
        public User Creator { get; }
        public DateTime Created { get; }
        public User Updater { get; }
        public DateTime Updated { get; }
        public Attachment[] Attachments { get; }
        public Star[] Stars { get; }

        internal PullRequest(_PullRequest data, GitRepository repo)
            : base(data.id)
        {
            Repository = repo;

            var client = repo.Project.Client;

            Number = data.number;
            Summary = data.summary;
            Description = data.description;
            Base = data.@base;
            Branch = data.branch;
            Status = new PullRequestStatus(data.status);
            Assignee = client.ItemsCache.Update(data.assignee?.id, () => new User(data.assignee, client));
            TicketId = data.issue.id;
            BaseCommit = data.baseCommit;
            BranchCommit = data.branchCommit;
            Closed = data.closeAt ?? default;
            Merged = data.mergeAt ?? default;
            Creator = client.ItemsCache.Update(data.createdUser?.id, () => new User(data.createdUser, client));
            Created = data.created ?? default;
            Updater = client.ItemsCache.Update(data.updatedUser?.id, () => new User(data.updatedUser, client));
            Updated = data.updated ?? default;
            Attachments = data.attachments.Select(x => new Attachment(x, this)).ToArray();
            Stars = data.stars.Select(x => new Star(x, client)).ToArray();
        }
    }
}
