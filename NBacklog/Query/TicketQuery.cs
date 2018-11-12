using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBacklog.Query
{
    public class TicketQuery
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

        public enum OrderType
        {
            [Description("desc")] Desc,
            [Description("asc")] Asc,
        }

        internal TicketQuery Project(params Project[] values)
        {
            foreach (var value in values)
            {
                _parameters.Add(("projectId[]", value.Id));
            }
            return this;
        }

        public TicketQuery TicketType(params TicketType[] values)
        {
            foreach (var value in values)
            {
                _parameters.Add(("issueTypeid[]", value.Id));
            }
            return this;
        }

        public TicketQuery Category(params Category[] values)
        {
            foreach (var value in values)
            {
                _parameters.Add(("categoryId[]", value.Id));
            }
            return this;
        }

        public TicketQuery Versions(params Milestone[] values)
        {
            foreach (var value in values)
            {
                _parameters.Add(("versionId[]", value.Id));
            }
            return this;
        }

        public TicketQuery Milestone(params Milestone[] values)
        {
            foreach (var value in values)
            {
                _parameters.Add(("milestoneId[]", value.Id));
            }
            return this;
        }

        public TicketQuery Status(params StatusType[] values)
        {
            foreach (var value in values)
            {
                _parameters.Add(("statusId[]", value.Id));
            }
            return this;
        }

        public TicketQuery Priority(params PriorityType[] values)
        {
            foreach (var value in values)
            {
                _parameters.Add(("priorityId[]", value.Id));
            }
            return this;
        }

        public TicketQuery Assignee(params User[] values)
        {
            foreach (var value in values)
            {
                _parameters.Add(("assigneeId[]", value.Id));
            }
            return this;
        }

        public TicketQuery Creator(params User[] values)
        {
            foreach (var value in values)
            {
                _parameters.Add(("createdUserId[]", value.Id));
            }
            return this;
        }

        public TicketQuery Resolution(params ResolutionType[] values)
        {
            foreach (var value in values)
            {
                _parameters.Add(("resolutionId[]", value.Id));
            }
            return this;
        }

        public TicketQuery ParentChild(ParentChildType value)
        {
            _parameters.Add(("parentChild", (int)value));
            return this;
        }

        public TicketQuery HasAttachment(bool value)
        {
            _parameters.Add(("attachment", value));
            return this;
        }

        public TicketQuery HasSharedFile(bool value)
        {
            _parameters.Add(("sharedFile", value));
            return this;
        }

        public TicketQuery SortBy(SortType value)
        {
            var attribute = typeof(SortType).GetMember(value.ToString())[0]
                .GetCustomAttributes(typeof(DescriptionAttribute), false)[0] as DescriptionAttribute;
            _parameters.Add(("sort", attribute.Description));
            return this;
        }

        public TicketQuery OrderBy(OrderType value)
        {
            var attribute = typeof(OrderType).GetMember(value.ToString())[0]
                .GetCustomAttributes(typeof(DescriptionAttribute), false)[0] as DescriptionAttribute;
            _parameters.Add(("order", attribute.Description));
            return this;
        }

        public TicketQuery Offset(int value)
        {
            _parameters.Add(("offset", value));
            return this;
        }

        public TicketQuery Count(int value)
        {
            _parameters.Add(("count", value));
            return this;
        }

        public TicketQuery CreatedSince(DateTime value)
        {
            _parameters.Add(("createdSince", value.ToString("yyyy-MM-dd")));
            return this;
        }

        public TicketQuery CreatedUntil(DateTime value)
        {
            _parameters.Add(("createdUntil", value.ToString("yyyy-MM-dd")));
            return this;
        }

        public TicketQuery LastUpdatedSince(DateTime value)
        {
            _parameters.Add(("updatedSince", value.ToString("yyyy-MM-dd")));
            return this;
        }

        public TicketQuery LastUpdatedUntil(DateTime value)
        {
            _parameters.Add(("updatedUntil", value.ToString("yyyy-MM-dd")));
            return this;
        }

        public TicketQuery StartDateSince(DateTime value)
        {
            _parameters.Add(("startDateSince", value.ToString("yyyy-MM-dd")));
            return this;
        }

        public TicketQuery StartDateUntil(DateTime value)
        {
            _parameters.Add(("startDateUntil", value.ToString("yyyy-MM-dd")));
            return this;
        }

        public TicketQuery DueDateSince(DateTime value)
        {
            _parameters.Add(("dueDateSince", value.ToString("yyyy-MM-dd")));
            return this;
        }

        public TicketQuery DueDateUntil(DateTime value)
        {
            _parameters.Add(("dueDateUntil", value.ToString("yyyy-MM-dd")));
            return this;
        }

        public TicketQuery Ids(params int[] values)
        {
            foreach (var value in values)
            {
                _parameters.Add(("id[]", value));
            }
            return this;
        }

        public TicketQuery ParentTickets(params Ticket[] values)
        {
            foreach (var value in values)
            {
                _parameters.Add(("parentIssueId[]", value.Id));
            }
            return this;
        }

        public TicketQuery Keyword(string value)
        {
            _parameters.Add(("keyword", value));
            return this;
        }

        internal List<(string, object)> Build()
        {
            return _parameters;
        }

        private List<(string, object)> _parameters = new List<(string, object)>();
    }
}
