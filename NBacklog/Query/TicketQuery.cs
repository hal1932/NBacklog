using NBacklog.DataTypes;
using System;
using System.ComponentModel;

namespace NBacklog.Query
{
    public class TicketQuery : Query<TicketQuery>
    {
        public enum ParentChildType
        {
            All = 0,
            ParentOrIndependent = 1,
            Child = 2,
            Independent = 3,
            Parent = 4,
        }

        public enum SortType
        {
            [Description("issueType")] TicketType,
            [Description("category")] Category,
            [Description("version")] Version,
            [Description("milestone")] Milestone,
            [Description("summary")] Summary,
            [Description("status")] Status,
            [Description("priority")] Priority,
            [Description("attachment")] Attachment,
            [Description("sharedFile")] SharedFile,
            [Description("created")] Created,
            [Description("createdUser")] Creator,
            [Description("updated")] LastUpdated,
            [Description("updatedUser")] LastUpdater,
            [Description("assignee")] Assignee,
            [Description("startDate")] StartDate,
            [Description("dueDate")] DueDate,
            [Description("estimatedHours")] EstimatedHours,
            [Description("actualHours")] ActualHours,
            [Description("childIssue")] ChildTicket,
        }

        public const int MaxCount = 100;

        internal TicketQuery Project(params Project[] values)
        {
            return AddParameterRange("projectId[]", values, x => x.Id);
        }

        public TicketQuery TicketType(params TicketType[] values)
        {
            return AddParameterRange("issueTypeid[]", values, x => x.Id);
        }

        public TicketQuery Category(params Category[] values)
        {
            return AddParameterRange("categoryId[]", values, x => x.Id);
        }

        public TicketQuery Versions(params Milestone[] values)
        {
            return AddParameterRange("versionId[]", values, x => x.Id);
        }

        public TicketQuery Milestone(params Milestone[] values)
        {
            return AddParameterRange("milestoneId[]", values, x => x.Id);
        }

        public TicketQuery Status(params Status[] values)
        {
            return AddParameterRange("statusId[]", values, x => x.Id);
        }

        public TicketQuery Priority(params Priority[] values)
        {
            return AddParameterRange("priorityId[]", values, x => x.Id);
        }

        public TicketQuery Assignee(params User[] values)
        {
            return AddParameterRange("assigneeId[]", values, x => x.Id);
        }

        public TicketQuery Creator(params User[] values)
        {
            return AddParameterRange("createdUserId[]", values, x => x.Id);
        }

        public TicketQuery Resolution(params Resolution[] values)
        {
            return AddParameterRange("resolutionId[]", values, x => x.Id);
        }

        public TicketQuery ParentChild(ParentChildType value)
        {
            return ReplaceParameter("parentChild", (int)value);
        }

        public TicketQuery HasAttachment(bool value)
        {
            return ReplaceParameter("attachment", value);
        }

        public TicketQuery HasSharedFile(bool value)
        {
            return ReplaceParameter("sharedFile", value);
        }

        public TicketQuery SortBy(SortType value)
        {
            return ReplaceParameter("sort", GetEnumDesc(value));
        }

        public TicketQuery OrderBy(OrderType value)
        {
            return ReplaceParameter("order", GetEnumDesc(value));
        }

        public TicketQuery Offset(int value)
        {
            return ReplaceParameter("offset", value);
        }

        public TicketQuery Count(int value)
        {
            return ReplaceParameter("count", value);
        }

        public TicketQuery CreatedSince(DateTime value)
        {
            return ReplaceParameter("createdSince", value);
        }

        public TicketQuery CreatedUntil(DateTime value)
        {
            return ReplaceParameter("createdUntil", value);
        }

        public TicketQuery LastUpdatedSince(DateTime value)
        {
            return ReplaceParameter("updatedSince", value);
        }

        public TicketQuery LastUpdatedUntil(DateTime value)
        {
            return ReplaceParameter("updatedUntil", value);
        }

        public TicketQuery StartDateSince(DateTime value)
        {
            return ReplaceParameter("startDateSince", value.Date);
        }

        public TicketQuery StartDateUntil(DateTime value)
        {
            return ReplaceParameter("startDateUntil", value.Date);
        }

        public TicketQuery DueDateSince(DateTime value)
        {
            return ReplaceParameter("dueDateSince", value.Date);
        }

        public TicketQuery DueDateUntil(DateTime value)
        {
            return ReplaceParameter("dueDateUntil", value.Date);
        }

        public TicketQuery Ids(params int[] values)
        {
            return AddParameterRange("id[]", values);
        }

        public TicketQuery ParentTickets(params Ticket[] values)
        {
            return AddParameterRange("parentIssueId[]", values, x => x.Id);
        }

        public TicketQuery Keyword(string value)
        {
            return ReplaceParameter("keyword", value);
        }
    }
}
