using NBacklog.DataTypes;
using NBacklog.Query;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NBacklog
{
    public partial class BacklogClient
    {
        public async Task<BacklogResponse<Team[]>> GetTeamsAsync(TeamQuery query = null)
        {
            query = query ?? new TeamQuery();

            var response = await GetAsync($"/api/v2/teams", query.Build()).ConfigureAwait(false);
            return await CreateResponseAsync<Team[], List<_Team>>(
                response,
                HttpStatusCode.OK,
                data => data.Select(x => new Team(x, this)).ToArray()).ConfigureAwait(false);
        }

        public async Task<BacklogResponse<Team>> AddTeamAsync(Team team)
        {
            var parameters = new
            {
                name = team.Name,
                members = team.Members.Select(x => x.Id).ToArray(),
            };

            var response = await PostAsync($"/api/v2/teams", parameters).ConfigureAwait(false);
            return await CreateResponseAsync<Team, _Team>(
                response,
                HttpStatusCode.Created,
                data => new Team(data, this)).ConfigureAwait(false);
        }

        public async Task<BacklogResponse<Team>> GetTeamAsync(int id)
        {
            var response = await GetAsync($"/api/v2/teams/{id}").ConfigureAwait(false);
            return await CreateResponseAsync<Team, _Team>(
                response,
                HttpStatusCode.OK,
                data => new Team(data, this)).ConfigureAwait(false);
        }

        public async Task<BacklogResponse<Team>> UpdateTeamAsync(Team team)
        {
            var parameters = new
            {
                name = team.Name,
                members = team.Members.Select(x => x.Id).ToArray(),
            };

            var response = await PatchAsync($"/api/v2/teams/{team.Id}", parameters).ConfigureAwait(false);
            return await CreateResponseAsync<Team, _Team>(
                response,
                HttpStatusCode.OK,
                data => new Team(data, this)).ConfigureAwait(false);
        }

        public async Task<BacklogResponse<Team>> DeleteTeamAsync(Team team)
        {
            var response = await DeleteAsync($"/api/v2/teams/{team.Id}").ConfigureAwait(false);
            return await CreateResponseAsync<Team, _Team>(
                response,
                HttpStatusCode.OK,
                data => new Team(data, this)).ConfigureAwait(false);
        }
    }
}
