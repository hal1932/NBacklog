using NBacklog.Query;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NBacklog.DataTypes
{
    public class ProjectDiskUsage : BacklogItem
    {
        public int Issue { get; }
        public int Wiki { get; }
        public int File { get; }
        public int Subversion { get; }
        public int Git { get; }

        internal ProjectDiskUsage(_ProjectDiskUsage data)
            : base(-1)
        {
            Issue = data.issue;
            Wiki = data.wiki;
            File = data.file;
            Subversion = data.subversion;
            Git = data.git;
        }
    }

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
                data => data.Select(x => Client.ItemsCache.Update(new User(x, Client))).ToArray());
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
                data => Client.ItemsCache.Update(new User(data, Client)));
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
                data => Client.ItemsCache.Delete(new User(data, Client)));
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

        #region type
        public async Task<BacklogResponse<TicketType[]>> GetTicketTypesAsync()
        {
            var response = await Client.GetAsync($"/api/v2/projects/{Id}/issueTypes").ConfigureAwait(false);
            return Client.CreateResponse<TicketType[], List<_TicketType>>(
                response,
                HttpStatusCode.OK,
                data => data.Select(x => Client.ItemsCache.Update(new TicketType(x, this))).ToArray());
        }

        public async Task<BacklogResponse<TicketType>> AddTicketTypeAsync(TicketType type)
        {
            var parameters = type.ToApiParameters();
            var response = await Client.PostAsync($"/api/v2/projects/{Id}/issueTypes", parameters.Build()).ConfigureAwait(false);
            return Client.CreateResponse<TicketType, _TicketType>(
                response,
                HttpStatusCode.OK,
                data => Client.ItemsCache.Update(new TicketType(data, this)));
        }

        public async Task<BacklogResponse<TicketType>> UpdateTicketTypeAsync(TicketType type)
        {
            var parameters = type.ToApiParameters();
            var response = await Client.PatchAsync($"/api/v2/projects/{Id}/issueTypes/{type.Id}", parameters.Build()).ConfigureAwait(false);
            return Client.CreateResponse<TicketType, _TicketType>(
                response,
                HttpStatusCode.OK,
                data => Client.ItemsCache.Update(new TicketType(data, this)));
        }

        public async Task<BacklogResponse<TicketType>> DeleteTicketAsync(TicketType type)
        {
            var response = await Client.DeleteAsync($"/api/v2/projects/{Id}/issueTypes/{type.Id}").ConfigureAwait(false);
            return Client.CreateResponse<TicketType, _TicketType>(
                response,
                HttpStatusCode.OK,
                data => Client.ItemsCache.Delete(new TicketType(data, this)));
        }
        #endregion

        #region category
        public async Task<BacklogResponse<Category[]>> GetCategoriesAsync()
        {
            var response = await Client.GetAsync($"/api/v2/projects/{Id}/categories").ConfigureAwait(false);
            return Client.CreateResponse<Category[], List<_Category>>(
                response,
                HttpStatusCode.OK,
                data => data.Select(x => Client.ItemsCache.Update(new Category(x))).ToArray());
        }

        public async Task<BacklogResponse<Category>> AddCategoryAsync(Category category)
        {
            var parameters = category.ToApiParameters();
            var response = await Client.PostAsync($"/api/v2/projects/{Id}/categories", parameters.Build()).ConfigureAwait(false);
            return Client.CreateResponse<Category, _Category>(
                response,
                HttpStatusCode.OK,
                data => Client.ItemsCache.Update(new Category(data)));
        }

        public async Task<BacklogResponse<Category>> UpdateCategoryAsync(Category category)
        {
            var parameters = category.ToApiParameters();
            var response = await Client.PatchAsync($"/api/v2/projects/{Id}/categories", parameters.Build()).ConfigureAwait(false);
            return Client.CreateResponse<Category, _Category>(
                response,
                HttpStatusCode.OK,
                data => Client.ItemsCache.Update(new Category(data)));
        }

        public async Task<BacklogResponse<Category>> DeleteCategoryAsync(Category category)
        {
            var parameters = category.ToApiParameters();
            var response = await Client.DeleteAsync($"/api/v2/projects/{Id}/categories").ConfigureAwait(false);
            return Client.CreateResponse<Category, _Category>(
                response,
                HttpStatusCode.OK,
                data => Client.ItemsCache.Update(new Category(data)));
        }
        #endregion

        #region milestone
        public async Task<BacklogResponse<Milestone[]>> GetMilestonesAsync()
        {
            var response = await Client.GetAsync($"/api/v2/projects/{Id}/versions").ConfigureAwait(false);
            return Client.CreateResponse<Milestone[], List<_Milestone>>(
                response,
                HttpStatusCode.OK,
                data => data.Select(x => Client.ItemsCache.Update(new Milestone(x, this))).ToArray());
        }

        public async Task<BacklogResponse<Milestone>> AddMilestoneAsync(Milestone milestone)
        {
            var parameters = milestone.ToApiParameters();
            var response = await Client.PostAsync($"/api/v2/projects/{Id}/versions", parameters.Build()).ConfigureAwait(false);
            return Client.CreateResponse<Milestone, _Milestone>(
                response,
                HttpStatusCode.OK,
                data => Client.ItemsCache.Update(new Milestone(data, this)));
        }

        public async Task<BacklogResponse<Milestone>> UpdateMilestoneAsync(Milestone milestone)
        {
            var parameters = milestone.ToApiParameters();
            var response = await Client.PatchAsync($"/api/v2/projects/{Id}/versions/{milestone.Id}", parameters.Build()).ConfigureAwait(false);
            return Client.CreateResponse<Milestone, _Milestone>(
                response,
                HttpStatusCode.OK,
                data => Client.ItemsCache.Update(new Milestone(data, this)));
        }

        public async Task<BacklogResponse<Milestone>> DeleteMilestoneAsync(Milestone milestone)
        {
            var response = await Client.DeleteAsync($"/api/v2/projects/{Id}/versions/{milestone.Id}").ConfigureAwait(false);
            return Client.CreateResponse<Milestone, _Milestone>(
                response,
                HttpStatusCode.OK,
                data => Client.ItemsCache.Delete(new Milestone(data, this)));
        }
        #endregion

        #region custom filed
        public async Task<BacklogResponse<CustomField[]>> GetCustomFieldsAsync()
        {
            var response = await Client.GetAsync($"/api/v2/projects/{Id}/customFields").ConfigureAwait(false);
            return Client.CreateResponse<CustomField[], List<_CustomField>>(
                response,
                HttpStatusCode.OK,
                data => data.Select(x => Client.ItemsCache.Update(CustomField.Create(x, this))).ToArray());
        }
        #endregion

        public async Task<BacklogResponse<SharedFile[]>> GetSharedFilesAsync(string directory = "", SharedFileQuery query = null)
        {
            query = query ?? new SharedFileQuery();

            var response = await Client.GetAsync($"/api/v2/projects/{Id}/files/metadata/{directory}", query.Build()).ConfigureAwait(false);
            var result = Client.CreateResponse<SharedFile[], List<_SharedFile>>(
                response,
                HttpStatusCode.OK,
                data => data.Select(x => Client.ItemsCache.Update(new SharedFile(x, this))).ToArray());

            if (result.Errors != null)
            {
                return result;
            }

            var content = result.Content.Where(x => query.TypeNames.Contains(x.TypeName)).ToArray();
            if (content.Length == result.Content.Length)
            {
                return result;
            }

            return new BacklogResponse<SharedFile[]>(result.StatusCode, content);
        }

        public async Task<BacklogResponse<MemoryStream>> GetIconAsync()
        {
            var response = await Client.GetAsync($"/api/v2/projects/{Id}/icon");
            return Client.CreateResponse(
                response,
                HttpStatusCode.OK,
                data => new MemoryStream(data));
        }

        public async Task<BacklogResponse<Activity[]>> GetActivitiesAsync(ActivityQuery query = null)
        {
            query = query ?? new ActivityQuery();
            var response = await Client.GetAsync($"/api/v2/projects/{Id}/activities", query.Build()).ConfigureAwait(false);
            return Client.CreateResponse<Activity[], List<_Activity>>(
                response,
                HttpStatusCode.OK,
                data => data.Select(x => new Activity(x, Client)).ToArray());
        }

        public async Task<BacklogResponse<ProjectDiskUsage>> GetDiskUsageAsync()
        {
            var response = await Client.GetAsync($"/api/v2/projects/{Id}/diskUsage").ConfigureAwait(false);
            return Client.CreateResponse<ProjectDiskUsage, _ProjectDiskUsage>(
                response,
                HttpStatusCode.OK,
                data => new ProjectDiskUsage(data));
        }
    }
}
