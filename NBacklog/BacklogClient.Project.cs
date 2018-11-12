using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBacklog
{
    public partial class BacklogClient
    {
        public async Task<BacklogResponse<Project[]>> GetProjectsAsync()
        {
            var response = await GetAsync<List<_Project>>("/api/v2/projects").ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<Project[]>.Create(
                response,
                data.Select(x => new Project(x, this)).ToArray());
        }

        public async Task<BacklogResponse<Project>> GetProjectAsync(int id)
        {
            return await GetProjectAsync(id.ToString()).ConfigureAwait(false);
        }

        public async Task<BacklogResponse<Project>> GetProjectAsync(string key)
        {
            var response = await GetAsync<_Project>($"/api/v2/projects/{key}").ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<Project>.Create(
                response,
                new Project(data, this));
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

            var response = await PatchAsync<_Project>($"/api/v2/projects/{project.Id}", parameters).ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<Project>.Create(
                response,
                new Project(data, this));
        }

        public async Task<BacklogResponse<Project>> DeleteProjectAsync(int id)
        {
            return await DeleteProjectAsync(id.ToString()).ConfigureAwait(false);
        }

        public async Task<BacklogResponse<Project>> DeleteProjectAsync(string key)
        {
            var response = await DeleteAsync<_Project>($"/api/v2/projects/{key}").ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<Project>.Create(
                response,
                new Project(data, this));
        }
    }

    class _Project
    {
        public int id { get; set; }
        public string projectKey { get; set; }
        public string name { get; set; }
        public bool chartEnabled { get; set; }
        public bool subtaskingEnabled { get; set; }
        public bool projectLeaderCanEditProjectLeader { get; set; }
        public string textFormattingRule { get; set; }
        public bool archived { get; set; }
    }
}
