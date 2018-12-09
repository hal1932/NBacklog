using System;

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
    }
}
