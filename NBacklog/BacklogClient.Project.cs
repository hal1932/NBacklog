using NBacklog.DataTypes;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NBacklog
{
    public partial class BacklogClient
    {
        public async Task<BacklogResponse<Project[]>> GetProjectsAsync()
        {
            var response = await GetAsync("/api/v2/projects").ConfigureAwait(false);
            return await CreateResponseAsync<Project[], List<_Project>>(
                response,
                HttpStatusCode.OK,
                data => data.Select(x => new Project(x, this)).ToArray()).ConfigureAwait(false);
        }

        public async Task<BacklogResponse<Project>> GetProjectAsync(int id)
        {
            return await GetProjectAsync(id.ToString()).ConfigureAwait(false);
        }

        public async Task<BacklogResponse<Project>> GetProjectAsync(string key)
        {
            var response = await GetAsync($"/api/v2/projects/{key}").ConfigureAwait(false);
            return await CreateResponseAsync<Project, _Project>(
                response,
                HttpStatusCode.OK,
                data => new Project(data, this)).ConfigureAwait(false);
        }

        public async Task<BacklogResponse<Project>> UpdateProjectAsync(Project project)
        {
            var parameters = new
            {
                name = project.Name,
                key = project.Key,
                chartEnabled = project.IsChartEnabled,
                subtaskingEnabled = project.IsSubtaskingEnabled,
                projectLeaderCanEditProjectLeader = project.CanProjectLeaderEditProjectLeader,
                textFormattingRule = project.TextFormattingRule,
                archived = project.IsArchived,
            };

            var response = await PatchAsync($"/api/v2/projects/{project.Id}", parameters).ConfigureAwait(false);
            return await CreateResponseAsync<Project, _Project>(
                response,
                HttpStatusCode.OK,
                data => new Project(data, this)).ConfigureAwait(false);
        }

        public async Task<BacklogResponse<Project>> DeleteProjectAsync(Project project)
        {
            var response = await DeleteAsync($"/api/v2/projects/{project.Id}").ConfigureAwait(false);
            return await CreateResponseAsync<Project, _Project>(
                response,
                HttpStatusCode.OK,
                data => new Project(data, this)).ConfigureAwait(false);
        }
    }
}
