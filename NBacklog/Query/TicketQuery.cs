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
            return AddParameter("parentChild", (int)value);
        }

        public TicketQuery HasAttachment(bool value)
        {
            return AddParameter("attachment", value);
        }

        public TicketQuery HasSharedFile(bool value)
        {
            return AddParameter("sharedFile", value);
        }

        public TicketQuery SortBy(SortType value)
        {
            var attribute = typeof(SortType).GetMember(value.ToString())[0]
                .GetCustomAttributes(typeof(DescriptionAttribute), false)[0] as DescriptionAttribute;
            return AddParameter("sort", attribute.Description);
        }

        public TicketQuery OrderBy(OrderType value)
        {
            var attribute = typeof(OrderType).GetMember(value.ToString())[0]
                .GetCustomAttributes(typeof(DescriptionAttribute), false)[0] as DescriptionAttribute;
            return AddParameter("order", attribute.Description);
        }

        public TicketQuery Offset(int value)
        {
            return AddParameter("offset", value);
        }

        public TicketQuery Count(int value)
        {
            return AddParameter("count", value);
        }

        public TicketQuery CreatedSince(DateTime value)
        {
            return AddParameter("createdSince", value);
        }

        public TicketQuery CreatedUntil(DateTime value)
        {
            return AddParameter("createdUntil", value);
        }

        public TicketQuery LastUpdatedSince(DateTime value)
        {
            return AddParameter("updatedSince", value);
        }

        public TicketQuery LastUpdatedUntil(DateTime value)
        {
            return AddParameter("updatedUntil", value);
        }

        public TicketQuery StartDateSince(DateTime value)
        {
            return AddParameter("startDateSince", value);
        }

        public TicketQuery StartDateUntil(DateTime value)
        {
            return AddParameter("startDateUntil", value);
        }

        public TicketQuery DueDateSince(DateTime value)
        {
            return AddParameter("dueDateSince", value);
        }

        public TicketQuery DueDateUntil(DateTime value)
        {
            return AddParameter("dueDateUntil", value);
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
            return AddParameter("keyword", value);
        }
    }
}
