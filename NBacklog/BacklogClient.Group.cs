using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NBacklog
{
    public partial class BacklogClient
    {
        public async Task<BacklogResponse<Group[]>> GetGroupsAsync(string order = "desc", int offset = 0, int count = 20)
        {
            var parameters = new
            {
                order = order,
                offset = offset,
                count = count,
            };

            var response = await GetAsync<List<_Group>>($"/api/v2/groups", parameters).ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<Group[]>.Create(
                response,
                data.Select(x => new Group(x, this)).ToArray());
        }

        public async Task<BacklogResponse<Group>> AddGroupAsync(Group group)
        {
            var parameters = new
            {
                name = group.Name,
                members = group.Members.Select(x => x.Id).ToArray(),
            };

            var response = await PostAsync<_Group>($"/api/v2/groups", parameters).ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<Group>.Create(
                response,
                new Group(data, this));
        }

        public async Task<BacklogResponse<Group>> GetGroupAsync(int id)
        {
            var response = await GetAsync<_Group>($"/api/v2/groups/{id}").ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<Group>.Create(
                response,
                new Group(data, this));
        }

        public async Task<BacklogResponse<Group>> UpdateGroupAsync(Group group)
        {
            var parameters = new
            {
                name = group.Name,
                members = group.Members.Select(x => x.Id).ToArray(),
            };

            var response = await PatchAsync<_Group>($"/api/v2/groups/{group.Id}", parameters).ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<Group>.Create(
                response,
                new Group(data, this));
        }

        public async Task<BacklogResponse<Group>> DeleteGroupAsync(Group group)
        {
            var response = await DeleteAsync<_Group>($"/api/v2/groups/{group.Id}").ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<Group>.Create(
                response,
                new Group(data, this));
            }
        }
}
