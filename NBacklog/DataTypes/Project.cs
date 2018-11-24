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
            var response = await Client.GetAsync($"/api/v2/projects/{Id}/users").ConfigureAwait(false);
            return Client.CreateResponse<User[], List <_User>>(
                response,
                HttpStatusCode.OK,
                data => data.Select(x => Client.ItemsCache.Update(new User(x))).ToArray());
        }

        public async Task<BacklogResponse<User>> AddUserAsync(User user)
        {
            var parameters = new
            {
                userId = user.Id,
            };

            var response = await Client.PostAsync($"/api/v2/projects/{Id}/users", parameters).ConfigureAwait(false);
            return Client.CreateResponse<User, _User>(
                response,
                HttpStatusCode.Created,
                data => Client.ItemsCache.Update(new User(data)));
        }

        public async Task<BacklogResponse<User>> DeleteUserAsync(User user)
        {
            var parameters = new
            {
                userId = user.Id,
            };

            var response = await Client.DeleteAsync($"/api/v2/projects/{Id}/users", parameters).ConfigureAwait(false);
            return Client.CreateResponse<User, _User>(
                response,
                HttpStatusCode.OK,
                data => Client.ItemsCache.Delete(new User(data)));
        }
        #endregion

        #region tickets
        public async Task<BacklogResponse<Ticket[]>> GetTicketsAsync(TicketQuery query = null)
        {
            query = query ?? new TicketQuery();
            query.Project(this);

            var response = await Client.GetAsync("/api/v2/issues", query.Build());
            return Client.CreateResponse<Ticket[], List<_Ticket>>(
                response,
                HttpStatusCode.OK,
                data => data.Select(x => Client.ItemsCache.Update(new Ticket(x, this, Client))).ToArray());
        }

        public async Task<BacklogResponse<int>> GetTicketCountAsync(TicketQuery query = null)
        {
            query = query ?? new TicketQuery();
            query.Project(this);

            var response = await Client.GetAsync("/api/v2/issues/count", query.Build());
            return Client.CreateResponse<int, _Count>(
                response,
                HttpStatusCode.OK,
                data => data.count);
        }

        public async Task<BacklogResponse<Ticket>> AddTicketAsync(Ticket ticket)
        {
            var parameters = ticket.ToApiParameters();
            parameters.Replace("projectId", Id);

            var response = await Client.PostAsync("/api/v2/issues", parameters.Build());
            return Client.CreateResponse<Ticket, _Ticket>(
                response,
                HttpStatusCode.Created,
                data => Client.ItemsCache.Update(new Ticket(data, this, Client)));
        }

        public async Task<BacklogResponse<Ticket>> UpdateTicketAsync(Ticket ticket)
        {
            var parameters = ticket.ToApiParameters();
            parameters.Replace("projectId", Id);

            var response = await Client.GetAsync($"/api/v2/issues/{ticket.Id}", parameters.Build());
            return Client.CreateResponse<Ticket, _Ticket>(
                response,
                HttpStatusCode.OK,
                data => Client.ItemsCache.Update(new Ticket(data, this, Client)));
        }

        public async Task<BacklogResponse<Ticket>> DeleteTicketAsync(Ticket ticket)
        {
            var response = await Client.DeleteAsync($"/api/v2/issues/{ticket.Id}");
            return Client.CreateResponse<Ticket, _Ticket>(
                response,
                HttpStatusCode.OK,
                data => Client.ItemsCache.Delete(new Ticket(data, this, Client)));
        }
        #endregion

        #region metadata
        public async Task<BacklogResponse<TicketType[]>> GetTicketTypesAsync()
        {
            var response = await Client.GetAsync($"/api/v2/projects/{Id}/issueTypes").ConfigureAwait(false);
            return Client.CreateResponse<TicketType[], List<_TicketType>>(
                response,
                HttpStatusCode.OK,
                data => data.Select(x => Client.ItemsCache.Update(new TicketType(x, this))).ToArray());
        }

        public async Task<BacklogResponse<Category[]>> GetCategoriesAsync()
        {
            var response = await Client.GetAsync($"/api/v2/projects/{Id}/categories").ConfigureAwait(false);
            return Client.CreateResponse<Category[], List<_Category>>(
                response,
                HttpStatusCode.OK,
                data => data.Select(x => Client.ItemsCache.Update(new Category(x))).ToArray());
        }

        public async Task<BacklogResponse<Milestone[]>> GetMilestonesAsync()
        {
            var response = await Client.GetAsync($"/api/v2/projects/{Id}/versions").ConfigureAwait(false);
            return Client.CreateResponse<Milestone[], List<_Milestone>>(
                response,
                HttpStatusCode.OK,
                data => data.Select(x => Client.ItemsCache.Update(new Milestone(x, this))).ToArray());
        }

        public async Task<BacklogResponse<CustomField[]>> GetCustomFieldsAsync()
        {
            var response = await Client.GetAsync($"/api/v2/projects/{Id}/customFields").ConfigureAwait(false);
            return Client.CreateResponse<CustomField[], List<_CustomField>>(
                response,
                HttpStatusCode.OK,
                data => data.Select(x => Client.ItemsCache.Update(CustomField.Create(x, this))).ToArray());
        }
        #endregion
    }
}
