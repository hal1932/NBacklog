using NBacklog.Query;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NBacklog.DataTypes
{
    public class Project : CachableBacklogItem
    {
        public BacklogClient Client { get; }

        public string Key { get; set; }
        public string Name { get; set; }
        public bool IsChartEnabled { get; set; }
        public bool IsSubtaskingEnabled { get; set; }
        public bool CanProjectLeaderEditProjectLeader { get; set; }
        public string TextFormattingRule { get; set; }
        public bool IsArchived { get; set; }

        internal Project(_Project data, BacklogClient client)
            : base(data.id)
        {
            Key = data.projectKey;
            Name = data.name;
            IsChartEnabled = data.chartEnabled;
            IsSubtaskingEnabled = data.subtaskingEnabled;
            CanProjectLeaderEditProjectLeader = data.projectLeaderCanEditProjectLeader;
            TextFormattingRule = data.textFormattingRule;
            IsArchived = data.archived;

            Client = client;
        }

        #region users
        public async Task<BacklogResponse<User[]>> GetUsersAsync()
        {
            var response = await Client.GetAsync<List<_User>>($"/api/v2/projects/{Id}/users").ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<User[]>.Create(
                response,
                HttpStatusCode.OK,
                data.Select(x => Client.ItemsCache.Get(x.id, () => new User(x))).ToArray());
        }

        public async Task<BacklogResponse<User>> AddUserAsync(User user)
        {
            var parameters = new
            {
                userId = user.Id,
            };

            var response = await Client.PostAsync<_User>($"/api/v2/projects/{Id}/users", parameters).ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<User>.Create(
                response,
                HttpStatusCode.Created,
                Client.ItemsCache.Get(data.id, () => new User(data)));
        }

        public async Task<BacklogResponse<User>> DeleteUserAsync(User user)
        {
            var parameters = new
            {
                userId = user.Id,
            };

            var response = await Client.DeleteAsync<_User>($"/api/v2/projects/{Id}/users", parameters).ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<User>.Create(
                response,
                HttpStatusCode.OK,
                Client.ItemsCache.Get(data.id, () => new User(data)));
        }
        #endregion

        #region tickets
        public async Task<BacklogResponse<Ticket[]>> GetTicketsAsync(TicketQuery query = null)
        {
            query = query ?? new TicketQuery();
            query.Project(this);

            var response = await Client.GetAsync<List<_Ticket>>("/api/v2/issues", query.Build());
            var data = response.Data;
            return BacklogResponse<Ticket[]>.Create(
                response,
                HttpStatusCode.OK,
                data.Select(x => Client.ItemsCache.Get(x.id, () => new Ticket(x, this, Client))).ToArray());
        }

        public async Task<BacklogResponse<int>> GetTicketCountAsync(TicketQuery query = null)
        {
            query = query ?? new TicketQuery();
            query.Project(this);

            var response = await Client.GetAsync<_Count>("/api/v2/issues/count", query.Build());
            var data = response.Data.count;
            return BacklogResponse<int>.Create(response, HttpStatusCode.OK, data);
        }

        public async Task<BacklogResponse<Ticket>> UpdateTicketAsync(Ticket ticket)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "summary", ticket.Summary },
                { "description", ticket.Description },
                { "statusId", ticket.Status.Id },
                { "startDate", ticket.StateDate },
                { "dueDate", ticket.DueDate },
                { "issueTypeId", ticket.Type.Id },
                { "categoryId", ticket.Categories.Select(x => x.Id).ToArray() },
                { "versionId", ticket.Versions.Select(x => x.Id).ToArray() },
                { "milestoneId", ticket.Milestones.Select(x => x.Id).ToArray() },
                { "priorityId", ticket.Priority.Id },
                { "attachmentId", ticket.Attachments.Select(x => x.Id).ToArray() },
            };

            if (ticket.ParentTicketId > 0) parameters["parentIssueId"] = ticket.ParentTicketId;
            if (ticket.Resolution != null) parameters["resolutionId"] = ticket.Resolution.Id;
            if (ticket.EstimatedHours > 0) parameters["estimatedHours"] = ticket.EstimatedHours;
            if (ticket.ActualHours > 0) parameters["actualHours"] = ticket.ActualHours;
            if (ticket.Assignee != null) parameters["assigneeId"] = ticket.Assignee.Id;

            foreach (var field in ticket.CustomFields)
            {
                parameters[$"customField_{field.Id}"] = field.ToJsonValue();
                if (field.OtherValue != null)
                {
                    parameters[$"customField_{field.Id}_otherValue"] = field.OtherValue;
                }
            }

            var response = await Client.GetAsync<_Ticket>($"/api/v2/issues/{ticket.Id}", parameters);
            var data = response.Data;
            return BacklogResponse<Ticket>.Create(
                response,
                HttpStatusCode.OK,
                Client.ItemsCache.Update(new Ticket(data, this, Client)));
        }
        #endregion

        #region metadata
        public async Task<BacklogResponse<TicketType[]>> GetTicketTypesAsync()
        {
            var response = await Client.GetAsync<List<_TicketType>>($"/api/v2/projects/{Id}/issueTypes").ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<TicketType[]>.Create(
                response,
                HttpStatusCode.OK,
                data.Select(x => Client.ItemsCache.Get(x.id, () => new TicketType(x, this))).ToArray());
        }

        public async Task<BacklogResponse<Category[]>> GetCategoriesAsync()
        {
            var response = await Client.GetAsync<List<_Category>>($"/api/v2/projects/{Id}/categories").ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<Category[]>.Create(
                response,
                HttpStatusCode.OK,
                data.Select(x => Client.ItemsCache.Get(x.id, () => new Category(x))).ToArray());
        }

        public async Task<BacklogResponse<Milestone[]>> GetMilestonesAsync()
        {
            var response = await Client.GetAsync<List<_Milestone>>($"/api/v2/projects/{Id}/versions").ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<Milestone[]>.Create(
                response,
                HttpStatusCode.OK,
                data.Select(x => Client.ItemsCache.Get(x.id, () => new Milestone(x, this))).ToArray());
        }

        public async Task<BacklogResponse<CustomField[]>> GetCustomFieldsAsync()
        {
            var response = await Client.GetAsync<List<_CustomField>>($"/api/v2/projects/{Id}/customFields").ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<CustomField[]>.Create(
                response,
                HttpStatusCode.OK,
                data.Select(x => Client.ItemsCache.Get(x.id, () => CustomField.Create(x, this))).ToArray());
        }
        #endregion
    }
}
