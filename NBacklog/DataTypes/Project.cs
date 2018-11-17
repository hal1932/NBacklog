using NBacklog.Query;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NBacklog.DataTypes
{
    public class Project : CachableBacklogItem
    {
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

            _client = client;
        }

        #region users
        public async Task<BacklogResponse<User[]>> GetUsersAsync(bool excludeGroupMembers = true)
        {
            var parameters = new
            {
                excludeGroupMembers = excludeGroupMembers,
            };

            var response = await _client.GetAsync<List<_User>>($"/api/v2/projects/{Id}/users", parameters).ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<User[]>.Create(
                response,
                HttpStatusCode.OK,
                data.Select(x => _client.ItemsCache.Get(x.id, () => new User(x, _client))).ToArray());
        }

        public async Task<BacklogResponse<User>> AddUserAsync(User user)
        {
            var parameters = new
            {
                userId = user.Id,
            };

            var response = await _client.PostAsync<_User>($"/api/v2/projects/{Id}/users", parameters).ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<User>.Create(
                response,
                HttpStatusCode.Created,
                _client.ItemsCache.Get(data.id, () => new User(data, _client)));
        }

        public async Task<BacklogResponse<User>> DeleteUserAsync(User user)
        {
            var parameters = new
            {
                userId = user.Id,
            };

            var response = await _client.DeleteAsync<_User>($"/api/v2/projects/{Id}/users", parameters).ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<User>.Create(
                response,
                HttpStatusCode.OK,
                _client.ItemsCache.Get(data.id, () => new User(data, _client)));
        }
        #endregion

        #region tickets
        public async Task<BacklogResponse<TicketType[]>> GetTicketTypesAsync()
        {
            var response = await _client.GetAsync<List<_TicketType>>($"/api/v2/projects/{Id}/issueTypes").ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<TicketType[]>.Create(
                response,
                HttpStatusCode.OK,
                data.Select(x => _client.ItemsCache.Get(x.id, () => new TicketType(x, this))).ToArray());
        }

        public async Task<BacklogResponse<Category[]>> GetCategoriesAsync()
        {
            var response = await _client.GetAsync<List<_Category>>($"/api/v2/projects/{Id}/categories").ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<Category[]>.Create(
                response,
                HttpStatusCode.OK,
                data.Select(x => _client.ItemsCache.Get(x.id, () => new Category(x))).ToArray());
        }

        public async Task<BacklogResponse<Milestone[]>> GetMilestonesAsync()
        {
            var response = await _client.GetAsync<List<_Milestone>>($"/api/v2/projects/{Id}/versions").ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<Milestone[]>.Create(
                response,
                HttpStatusCode.OK,
                data.Select(x => _client.ItemsCache.Get(x.id, () => new Milestone(x, this))).ToArray());
        }

        public async Task<BacklogResponse<Ticket[]>> GetTicketsAsync(TicketQuery query = null)
        {
            query = query ?? new TicketQuery();
            query.Project(this);

            var response = await _client.GetAsync<List<_Ticket>>("/api/v2/issues", query.Build());
            var data = response.Data;
            return BacklogResponse<Ticket[]>.Create(
                response,
                HttpStatusCode.OK,
                data.Select(x => _client.ItemsCache.Get(x.id, () => new Ticket(x, this, _client))).ToArray());
        }

        public async Task<BacklogResponse<int>> GetTicketCountAsync(TicketQuery query = null)
        {
            query = query ?? new TicketQuery();
            query.Project(this);

            var response = await _client.GetAsync<_Count>("/api/v2/issues/count", query.Build());
            var data = response.Data.count;
            return BacklogResponse<int>.Create(response, HttpStatusCode.OK, data);
        }
        #endregion

        public async Task<BacklogResponse<CustomField[]>> GetCustomFieldsAsync()
        {
            var response = await _client.GetAsync<List<_CustomField>>($"/api/v2/projects/{Id}/customFields").ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<CustomField[]>.Create(
                response,
                HttpStatusCode.OK,
                data.Select(x => _client.ItemsCache.Get(x.id, () => CustomField.Create(x, this))).ToArray());
        }

        private BacklogClient _client;
    }
}
