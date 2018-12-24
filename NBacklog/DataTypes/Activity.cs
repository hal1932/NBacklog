using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace NBacklog.DataTypes
{
    public enum ActivityEvent
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

    public enum ActivityAction
    {
        Created,
        Added,
        NotifyAdded,
        Updated,
        MultiUpdated,
        Merged,
        Pushed,
        Commited,
        Commented,
        Removed,
        Deleted,
    }

    public class Activity : BacklogItem
    {
        public Project Project { get; }
        public ActivityEvent Type { get; }
        public ActivityContent Content { get; }
        public User Creator { get; }
        public DateTime Created { get; }

        public static ActivityEvent[] GetAllTypes()
        {
            return Enum.GetValues(typeof(ActivityEvent))
                .Cast<ActivityEvent>()
                .Where(x => x > 0)
                .ToArray();
        }

        internal Activity(_Activity data, BacklogClient client)
            : base(data.id)
        {
            Project = client.ItemsCache.Update(data.project.id, () => new Project(data.project, client));
            Type = (ActivityEvent)data.type;
            Creator = client.ItemsCache.Update(data.createdUser.id, () => new User(data.createdUser, client));
            Created = data.created;

            var contentData = data.content;

            switch (Type)
            {
                #region File
                case ActivityEvent.FileAdded:
                    Content = new FileActivityContent(contentData, ActivityAction.Added);
                    break;

                case ActivityEvent.FileDeleted:
                    Content = new FileActivityContent(contentData, ActivityAction.Deleted);
                    break;

                case ActivityEvent.FileUpdated:
                    Content = new FileActivityContent(contentData, ActivityAction.Updated);
                    break;
                #endregion

                #region Git
                case ActivityEvent.GitPullRequestAdded:
                    Content = new GitPullRequestActivityContent(contentData, Project, ActivityAction.Added);
                    break;

                case ActivityEvent.GitPullRequestCommented:
                    Content = new GitPullRequestActivityContent(contentData, Project, ActivityAction.Commented);
                    break;

                case ActivityEvent.GitPullRequestMerged:
                    Content = new GitPullRequestActivityContent(contentData, Project, ActivityAction.Merged);
                    break;

                case ActivityEvent.GitPullRequestUpdated:
                    Content = new GitPullRequestActivityContent(contentData, Project, ActivityAction.Updated);
                    break;

                case ActivityEvent.GitPushed:
                    Content = new GitPushedActivityContent(contentData, Project);
                    break;

                case ActivityEvent.GitRepositoryCreated:
                    Content = new GitRepositoryCreatedActivityContent(contentData, Project);
                    break;
                #endregion

                #region SVN
                case ActivityEvent.SvnCommitted:
                    Content = new SvnCommittedActivityContent(contentData);
                    break;
                #endregion

                #region Project
                case ActivityEvent.ProjectUserAdded:
                    Content = new ProjectUserActivityContent(contentData, client, ActivityAction.Added);
                    break;

                case ActivityEvent.ProjectUserRemoved:
                    Content = new ProjectUserActivityContent(contentData, client, ActivityAction.Removed);
                    break;

                case ActivityEvent.ProjectGroupAdded:
                    Content = new GroupActivityContent(contentData, client, ActivityAction.Added);
                    break;

                case ActivityEvent.ProjectGroupRemoved:
                    Content = new GroupActivityContent(contentData, client, ActivityAction.Removed);
                    break;
                #endregion

                #region Ticket
                case ActivityEvent.NotifyAdded:
                    Content = new TicketActivityContent(contentData, ActivityAction.NotifyAdded);
                    break;

                case ActivityEvent.TicketCommented:
                    Content = new TicketActivityContent(contentData, ActivityAction.Commented);
                    break;

                case ActivityEvent.TicketCreated:
                    Content = new TicketActivityContent(contentData, ActivityAction.Created);
                    break;

                case ActivityEvent.TicketDeleted:
                    Content = new TicketActivityContent(contentData, ActivityAction.Deleted);
                    break;

                case ActivityEvent.TicketMultiUpdated:
                    Content = new TicketActivityContent(contentData, ActivityAction.MultiUpdated);
                    break;

                case ActivityEvent.TicketUpdated:
                    Content = new TicketActivityContent(contentData, ActivityAction.Updated);
                    break;
                #endregion

                #region Wiki
                case ActivityEvent.WikiCreated:
                    Content = new WikiActivityContent(contentData, Project, ActivityAction.Created);
                    break;

                case ActivityEvent.WikiDeleted:
                    Content = new WikiActivityContent(contentData, Project, ActivityAction.Deleted);
                    break;

                case ActivityEvent.WikiUpdated:
                    Content = new WikiActivityContent(contentData, Project, ActivityAction.Updated);
                    break;
                #endregion

                #region Milestone
                case ActivityEvent.MilestoneAdded:
                    Content = new MilestoneActivityContent(contentData, ActivityAction.Added);
                    break;

                case ActivityEvent.MilestoneDeleted:
                    Content = new MilestoneActivityContent(contentData, ActivityAction.Deleted);
                    break;

                case ActivityEvent.MilestoneUpdated:
                    Content = new MilestoneActivityContent(contentData, ActivityAction.Updated);
                    break;
                #endregion

                default:
                    throw new ArgumentException($"invalid content type: {Type}");
            }
        }
    }


    public abstract class ActivityContent
    {
        public ActivityAction Action { get; }

        internal protected ActivityContent(ActivityAction action)
        {
            Action = action;
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

    public class TicketActivityContent : ActivityContent
    {
        // チケット単体に対する操作
        public TicketSummary Ticket { get; }

        // チケット複数に対する操作
        public int TransactionId { get; }
        public TicketSummary[] Tickets { get; }

        // 共通
        public CommentSummary Comment { get; }
        public Attachment[] Attachments { get; }
        public Change[] Changes { get; }

        internal TicketActivityContent(JObject data, ActivityAction action)
            : base(action)
        {
            if (action == ActivityAction.MultiUpdated)
            {
                TransactionId = data.Value<int>("tx_id");

                Tickets = (data.Value<JArray>("link") ?? Enumerable.Empty<object>()).Cast<JObject>()
                    .Select(x => new TicketSummary(new _TicketSummary()
                    {
                        id = x.Value<int>("id"),
                        keyId = x.Value<int>("key_id"),
                        summary = x.Value<string>("title"),
                    }))
                    .ToArray();
            }
            else
            {
                Ticket = new TicketSummary(new _TicketSummary()
                {
                    id = data.Value<int>("id"),
                    keyId = data.Value<int>("key_id"),
                    summary = data.Value<string>("summary"),
                    description = data.Value<string>("description"),
                });
            }

            var comment = data.Value<JObject>("comment")?.ToObject<_CommentSummary>();
            if (comment?.id != default)
            {
                Comment = new CommentSummary(comment, Ticket);
            }

            Attachments = (data.Value<JArray>("attachments") ?? Enumerable.Empty<object>()).Cast<JObject>()
                .Select(x => new Attachment(x, null))
                .ToArray();

            Changes = (data.Value<JArray>("changes") ?? Enumerable.Empty<object>()).Cast<JObject>()
                .Select(x => new Change(x))
                .ToArray();
        }
    }

    public class WikiActivityContent : ActivityContent
    {
        public WikipageSummary Wikipage { get; }
        public string Diff { get; }
        public int Version { get; }

        internal WikiActivityContent(JObject data, Project project, ActivityAction action)
            : base(action)
        {
            Wikipage = new WikipageSummary(data.ToObject<_WikipageSummary>());
            //Id = data.Value<int>("id");
            //Name = data.Value<string>("name");
            //Content = data.Value<string>("content");
            Diff = data.Value<string>("diff");
            Version = data.Value<int?>("version") ?? default;

            //Attachments = (data.Value<JArray>("attachments") ?? Enumerable.Empty<object>()).Cast<JObject>()
            //    .Select(x => new Attachment(x, null))
            //    .ToArray();
            //SharedFiles = (data.Value<JArray>("shared_files") ?? Enumerable.Empty<object>()).Cast<JObject>()
            //    .Select(x => new SharedFile(x.Value<int>("id"), x.Value<string>("name"), x.Value<long>("size"), project))
            //    .ToArray();
        }
    }

    public class FileActivityContent : ActivityContent
    {
        public int Id { get; }
        public string Directory { get; }
        public string Name { get; }
        public long Size { get; }

        internal FileActivityContent(JObject data, ActivityAction action)
            : base(action)
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
            : base(ActivityAction.Commited)
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

    public class GitPushedActivityContent : ActivityContent
    {
        public string ChangeType { get; }
        public string Ref { get; }
        public string RevisionType { get; }
        public GitRepoSummary Repository { get; }
        public GitRevision[] Revisions { get; }
        public int RevisionCount { get; }

        internal GitPushedActivityContent(JObject data, Project project)
            : base(ActivityAction.Pushed)
        {
            ChangeType = data.Value<string>("change_type");
            Ref = data.Value<string>("ref");
            RevisionType = data.Value<string>("revision_type");
            Repository = new GitRepoSummary(data["repository"].ToObject<_GitRepoSummary>(), project);
            Revisions = (data.Value<JArray>("revisions") ?? Enumerable.Empty<object>()).Cast<JObject>()
                .Select(x => new GitRevision(x))
                .ToArray();
            RevisionCount = data.Value<int>("revision_count");
        }
    }

    public class GitRepositoryCreatedActivityContent : ActivityContent
    {
        public GitRepoSummary Repository { get; }

        internal GitRepositoryCreatedActivityContent(JObject data, Project project)
            : base(ActivityAction.Created)
        {
            Repository = new GitRepoSummary(data["repository"].ToObject<_GitRepoSummary>(), project);
        }
    }

    public class ProjectUserActivityContent : ActivityContent
    {
        public User[] Users { get; }
        public string Comment { get; }

        internal ProjectUserActivityContent(JObject data, BacklogClient client, ActivityAction action)
            : base(action)
        {
            Users = (data.Value<JArray>("revisions") ?? Enumerable.Empty<object>()).Cast<JObject>()
                .Select(x => new User(x, client))
                .ToArray();
            Comment = data.Value<string>("comment");
        }
    }

    public class GitPullRequestActivityContent : ActivityContent
    {
        public PullRequestSummary PullRequest { get; }
        public CommentSummary Comment { get; }
        public Change[] Changes { get; }
        public GitRepoSummary Repository { get; }
        public TicketSummary Ticket { get; }

        internal GitPullRequestActivityContent(JObject data, Project project, ActivityAction action)
            : base(action)
        {
            Changes = (data.Value<JArray>("changes") ?? Enumerable.Empty<object>()).Cast<JObject>()
                .Select(x => new Change(x))
                .ToArray();

            Repository = new GitRepoSummary(data["repository"].ToObject<_GitRepoSummary>(), project);
            PullRequest = new PullRequestSummary(data.ToObject<_PullRequestSummary>(), Repository);

            var comment = data["comment"].ToObject<_CommentSummary>();
            if (comment?.id != default)
            {
                Comment = new CommentSummary(comment, PullRequest);
            }
        }
    }

    public class MilestoneActivityContent : ActivityContent
    {
        public MilestoneSummary Milestone { get; }

        internal MilestoneActivityContent(JObject data, ActivityAction action)
            : base(action)
        {
            Milestone = new MilestoneSummary(data.ToObject<_MilestoneSummary>());
        }
    }

    public class GroupActivityContent : ActivityContent
    {
        public Group[] Groups { get; }

        internal GroupActivityContent(JObject data, BacklogClient client, ActivityAction action)
            : base(action)
        {
            Groups = (data.Value<JArray>("parties") ?? Enumerable.Empty<object>()).Cast<JObject>()
                .Select(x => new Group(x.Value<int>("id"), x.Value<string>("name"), client))
                .ToArray();
        }
    }
}
