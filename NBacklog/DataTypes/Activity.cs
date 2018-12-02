using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace NBacklog.DataTypes
{
    public enum ActivityType
    {
        Undefined = -1,
        TicketCreated = 1,
        TicketUpdated = 2,
        TicketCommented = 3,
        TicketDeleted = 4,
        WikiCreated = 5,
        WikiUpdated = 6,
        WikiDeleted = 7,
        FileAdded = 8,
        FileUpdated = 9,
        FileDeleted = 10,
        SvnCommitted = 11,
        GitPushed = 12,
        GitRepositoryCreated = 13,
        TicketMultiUpdated = 14,
        ProjectUserAdded = 15,
        ProjectUserRemoved = 16,
        NotifyAdded = 17,
        GitPullRequestAdded = 18,
        GitPullRequestUpdated = 19,
        GitPullRequestCommented = 20,
        GitPullRequestMerged = 21,
        MilestoneAdded = 22,
        MilestoneUpdated = 23,
        MilestoneDeleted = 24,
        ProjectGroupAdded = 25,
        ProjectGroupRemoved = 26,
    }

    public class Activity : BacklogItem
    {
        public Project Project { get; }
        public ActivityType Type { get; }
        public ActivityContent Content { get; }
        public User Creator { get; }
        public DateTime Created { get; }

        public static ActivityType[] GetAllTypes()
        {
            return Enum.GetValues(typeof(ActivityType))
                .Cast<ActivityType>()
                .Where(x => x > 0)
                .ToArray();
        }

        internal Activity(_Activity data, BacklogClient client)
            : base(data.id)
        {
            Project = client.ItemsCache.Get(data.project.id, () => new Project(data.project, client));
            Type = (ActivityType)data.type;
            Creator = client.ItemsCache.Get(data.createdUser.id, () => new User(data.createdUser, client));
            Created = data.created;

            var contentData = data.content;

            switch (Type)
            {
                case ActivityType.FileAdded:
                case ActivityType.FileDeleted:
                case ActivityType.FileUpdated:
                    Content = new FileActivityContent(contentData);
                    break;

                case ActivityType.GitPullRequestAdded:
                case ActivityType.GitPullRequestCommented:
                case ActivityType.GitPullRequestMerged:
                case ActivityType.GitPullRequestUpdated:
                    Content = new GitPullRequestActivityContent(contentData, client);
                    break;

                case ActivityType.GitPushed:
                    Content = new GitPushedActivityContent(contentData, client);
                    break;

                case ActivityType.GitRepositoryCreated:
                    Content = new GitRepositoryCreatedActivityContent(contentData, client);
                    break;

                case ActivityType.NotifyAdded:
                    Content = new NotifyAddedActivityContent(contentData);
                    break;

                case ActivityType.ProjectUserAdded:
                case ActivityType.ProjectUserRemoved:
                    Content = new ProjectUserActivityContent(contentData, client);
                    break;

                case ActivityType.SvnCommitted:
                    Content = new SvnCommittedActivityContent(contentData);
                    break;

                case ActivityType.TicketCommented:
                    Content = new TicketCommentedActivityContent(contentData);
                    break;

                case ActivityType.TicketCreated:
                    Content = new TicketCreatedActivityContent(contentData);
                    break;

                case ActivityType.TicketDeleted:
                    Content = new TicketDeletedActivityContent(contentData);
                    break;

                case ActivityType.TicketMultiUpdated:
                    Content = new TicketMultiUpdatedActivityContent(contentData);
                    break;

                case ActivityType.TicketUpdated:
                    Content = new TicketUpdatedActivityContent(contentData);
                    break;

                case ActivityType.WikiCreated:
                case ActivityType.WikiDeleted:
                case ActivityType.WikiUpdated:
                    Content = new WikiActivityContent(contentData, Project);
                    break;

                case ActivityType.MilestoneAdded:
                case ActivityType.MilestoneDeleted:
                case ActivityType.MilestoneUpdated:
                    Content = new MilestoneActivityContent(contentData);
                    break;

                case ActivityType.ProjectGroupAdded:
                case ActivityType.ProjectGroupRemoved:
                    Content = new GroupActivityContent(contentData, client);
                    break;

                default:
                    throw new ArgumentException($"invalid content type: {Type}");
            }
        }
    }


    public abstract class ActivityContent
    { }

    public class TicketCreatedActivityContent : ActivityContent
    {
        public int Id { get; }
        public int KeyId { get; }
        public string Summary { get; }
        public string Description { get; }

        internal TicketCreatedActivityContent(JObject data)
        {
            Id = data.Value<int>("id");
            KeyId = data.Value<int>("key_id");
            Summary = data.Value<string>("summary");
            Description = data.Value<string>("description");
        }
    }

    public class Change
    {
        public string Field { get; }
        public string OldValue { get; }
        public string NewValue { get; }
        public string Type { get; }

        internal Change(JObject data)
        {
            Field = data.Value<string>("field");
            OldValue = data.Value<string>("old_value");
            NewValue = data.Value<string>("new_value");
            Type = data.Value<string>("type");
        }
    }

    public class TicketUpdatedActivityContent : ActivityContent
    {
        public int Id { get; }
        public int KeyId { get; }
        public string Summary { get; }
        public string Description { get; }
        public Comment Comment { get; }
        public Attachment[] Attachments { get; }
        public Change[] Changes { get; }

        internal TicketUpdatedActivityContent(JObject data)
        {
            Id = data.Value<int>("id");
            KeyId = data.Value<int>("key_id");
            Summary = data.Value<string>("summary");
            Description = data.Value<string>("description");

            var comment = data.Value<JObject>("comment");
            if (comment != null)
            {
                Comment = new Comment(comment.Value<int>("id"), comment.Value<string>("content"));
            }

            Attachments = (data.Value<JArray>("attachments") ?? Enumerable.Empty<object>()).Cast<JObject>()
                .Select(x => new Attachment(x, null))
                .ToArray();

            Changes = (data.Value<JArray>("changes") ?? Enumerable.Empty<object>()).Cast<JObject>()
                .Select(x => new Change(x))
                .ToArray();
        }
    }

    public class TicketCommentedActivityContent : TicketUpdatedActivityContent
    {
        internal TicketCommentedActivityContent(JObject data)
            : base(data)
        { }
    }

    public class TicketMultiUpdatedActivityContent : ActivityContent
    {
        public int TxId { get; }
        public Comment Comment { get; }
        public Link[] Links { get; }
        public Change[] Changes { get; }

        internal TicketMultiUpdatedActivityContent(JObject data)
        {
            TxId = data.Value<int>("tx_id");

            var comment = data.Value<JObject>("comment");
            if (comment != null)
            {
                Comment = new Comment(comment.Value<int>("id"), comment.Value<string>("content"));
            }

            Links = (data.Value<JArray>("link") ?? Enumerable.Empty<object>()).Cast<JObject>()
                .Select(x => new Link(x))
                .ToArray();
            Changes = (data.Value<JArray>("changes") ?? Enumerable.Empty<object>()).Cast<JObject>()
                .Select(x => new Change(x))
                .ToArray();
        }
    }

    public class TicketDeletedActivityContent : ActivityContent
    {
        public int Id { get; }
        public int KeyId { get; }

        internal TicketDeletedActivityContent(JObject data)
        {
            Id = data.Value<int>("id");
            KeyId = data.Value<int>("key_id");
        }
    }

    public class WikiActivityContent : ActivityContent
    {
        public int Id { get; }
        public string Name { get; }
        public string Content { get; }
        public string Diff { get; }
        public int Version { get; }
        public Attachment[] Attachments { get; }
        public SharedFile[] SharedFiles { get; }

        internal WikiActivityContent(JObject data, Project project)
        {
            Id = data.Value<int>("id");
            Name = data.Value<string>("name");
            Content = data.Value<string>("content");
            Diff = data.Value<string>("diff");
            Version = data.Value<int?>("version") ?? default(int);

            Attachments = (data.Value<JArray>("attachments") ?? Enumerable.Empty<object>()).Cast<JObject>()
                .Select(x => new Attachment(x, null))
                .ToArray();
            SharedFiles = (data.Value<JArray>("shared_files") ?? Enumerable.Empty<object>()).Cast<JObject>()
                .Select(x => new SharedFile(x.Value<int>("id"), x.Value<string>("name"), x.Value<long>("size"), project))
                .ToArray();
        }
    }

    public class FileActivityContent : ActivityContent
    {
        public int Id { get; }
        public string Directory { get; }
        public string Name { get; }
        public long Size { get; }

        internal FileActivityContent(JObject data)
        {
            Id = data.Value<int>("id");
            Directory = data.Value<string>("dir");
            Name = data.Value<string>("name");
            Size = data.Value<long>("size");
        }
    }

    public class SvnCommittedActivityContent : ActivityContent
    {
        public int Revision { get; }
        public string Comment { get; }

        internal SvnCommittedActivityContent(JObject data)
        {
            Revision = data.Value<int>("rev");
            Comment = data.Value<string>("comment");
        }
    }

    public class GitRevision
    {
        public string Hash { get; }
        public string Comment { get; }

        internal GitRevision(JObject data)
        {
            Hash = data.Value<string>("rev");
            Comment = data.Value<string>("comment");
        }
    }

    public class GitRepogitory
    {
        public int Id { get; }
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

        internal GitRepogitory(JObject data, BacklogClient client)
        {
            Id = data.Value<int>("id");
            ProjectId = data.Value<int>("projectId");
            Name = data.Value<string>("name");
            Description = data.Value<string>("description");
            HookUrl = data.Value<string>("hookUrl");
            HttpUrl = data.Value<string>("httpUrl");
            SshUrl = data.Value<string>("sshUrl");
            DisplayOrder = data.Value<int>("displayOrder");
            Pushed = data.Value<DateTime>("pushedAt");

            var creator = data.Value<JObject>("createdUser");
            if (creator != null)
            {
                Creator = client.ItemsCache.Get(creator.Value<int>("id"), () => new User(creator, client));
            }
            Created = data.Value<DateTime>("created");

            var updater = data.Value<JObject>("updater");
            if (updater != null)
            {
                Updater = client.ItemsCache.Get((int)updater["id"], () => new User(updater, client));
            }
            Updated = data.Value<DateTime>("updated");
        }
    }

    public class GitPushedActivityContent : ActivityContent
    {
        public string ChangeType { get; }
        public string Ref { get; }
        public string RevisionType { get; }
        public GitRepogitory Repogitory { get; }
        public GitRevision[] Revisions { get; }
        public int RevisionCount { get; }

        internal GitPushedActivityContent(JObject data, BacklogClient client)
        {
            ChangeType = data.Value<string>("change_type");
            Ref = data.Value<string>("ref");
            RevisionType = data.Value<string>("revision_type");
            Repogitory = new GitRepogitory(data.Value<JObject>("repogitory"), client);
            Revisions = (data.Value<JArray>("revisions") ?? Enumerable.Empty<object>()).Cast<JObject>()
                .Select(x => new GitRevision(x))
                .ToArray();
            RevisionCount = data.Value<int>("revision_count");
        }
    }

    public class GitRepositoryCreatedActivityContent : ActivityContent
    {
        public GitRepogitory Repogitory { get; }

        internal GitRepositoryCreatedActivityContent(JObject data, BacklogClient client)
        {
            Repogitory = new GitRepogitory(data.Value<JObject>("repogitory"), client);
        }
    }

    public class ProjectUserActivityContent : ActivityContent
    {
        public User[] Users { get; }
        public string Comment { get; }

        internal ProjectUserActivityContent(JObject data, BacklogClient client)
        {
            Users = (data.Value<JArray>("revisions") ?? Enumerable.Empty<object>()).Cast<JObject>()
                .Select(x => new User(x, client))
                .ToArray();
            Comment = data.Value<string>("comment");
        }
    }

    public class NotifyAddedActivityContent : TicketCommentedActivityContent
    {
        internal NotifyAddedActivityContent(JObject data)
            : base(data)
        { }
    }

    public class GitPullRequestActivityContent : ActivityContent
    {
        public int Id { get; }
        public int Number { get; }
        public string Summary { get; }
        public string Description { get; }
        public Comment Comment { get; }
        public Change[] Changes { get; }
        public GitRepogitory Repogitory { get; }
        public Ticket Ticket { get; }

        internal GitPullRequestActivityContent(JObject data, BacklogClient client)
        {
            Id = data.Value<int>("id");
            Number = data.Value<int>("number");
            Summary = data.Value<string>("summary");
            Description = data.Value<string>("description");

            var comment = data.Value<JObject>("comment");
            if (comment != null)
            {
                Comment = new Comment(comment.Value<int>("id"), comment.Value<string>("content"));
            }

            Changes = (data.Value<JArray>("changes") ?? Enumerable.Empty<object>()).Cast<JObject>()
                .Select(x => new Change(x))
                .ToArray();

            Repogitory = new GitRepogitory(data.Value<JObject>("repository"), client);
        }
    }

    public class MilestoneActivityContent : ActivityContent
    {
        public int Id { get; }
        public string Name { get; }
        public string Description { get; }
        public DateTime StartDate { get; }
        public DateTime DueDate { get; }

        internal MilestoneActivityContent(JObject data)
        {
            Id = data.Value<int>("id");
            Name = data.Value<string>("name");
            Description = data.Value<string>("description");
            StartDate = data.Value<DateTime>("start_date");
            DueDate = data.Value<DateTime>("reference_date");
        }
    }

    public class GroupActivityContent : ActivityContent
    {
        public Group[] Groups { get; }

        internal GroupActivityContent(JObject data, BacklogClient client)
        {
            Groups = (data.Value<JArray>("parties") ?? Enumerable.Empty<object>()).Cast<JObject>()
                .Select(x => new Group(x.Value<int>("id"), x.Value<string>("name"), client))
                .ToArray();
        }
    }
}
