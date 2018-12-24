using NBacklog.DataTypes;

namespace NBacklog.Query
{
    public class PullRequestQuery : Query<PullRequestQuery>
    {
        public const int MaxCount = 100;

        public PullRequestQuery Status(PullRequestStatus status)
        {
            return AddParameter("statusId[]", status.Id);
        }

        public PullRequestQuery Assignee(User assignee)
        {
            return AddParameter("assigneeId[]", assignee.Id);
        }

        public PullRequestQuery Ticket(Ticket ticket)
        {
            return AddParameter("issueId[]", ticket.Id);
        }

        public PullRequestQuery Creator(User user)
        {
            return AddParameter("createdUserId[]", user.Id);
        }

        public PullRequestQuery Offset(int value)
        {
            return ReplaceParameter("offset", value);
        }

        public PullRequestQuery Count(int value)
        {
            return ReplaceParameter("count", value);
        }
    }
}
