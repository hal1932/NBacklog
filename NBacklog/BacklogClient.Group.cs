using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NBacklog
{
    public struct Group
    {
        public int Id;
        public string Name;
        public User[] Members;
        public int DisplayOrder;
        public User Creator;
        public DateTime Created;
        public User LastUpdater;
        public DateTime LastUpdated;
    }

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
                data.Select(x => new Group()
                {
                    Id = x.id,
                    Name = x.name,
                    DisplayOrder = x.displayOrder,
                    Members = x.members.Select(y => new User(y)).ToArray(),
                    Creator = (x.createdUser != null) ? new User(x.createdUser) : null,
                    Created = x.created,
                    LastUpdater = (x.updatedUser != null) ? new User(x.updatedUser) : null,
                    LastUpdated = x.updated,
                }).ToArray());
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
                new Group()
                {
                    Id = data.id,
                    Name = data.name,
                    DisplayOrder = data.displayOrder,
                    Members = data.members.Select(y => new User(y)).ToArray(),
                    Creator = (data.createdUser != null) ? new User(data.createdUser) : null,
                    Created = data.created,
                });
        }

        public async Task<BacklogResponse<Group>> GetGroupAsync(int id)
        {
            var response = await GetAsync<_Group>($"/api/v2/groups/{id}").ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<Group>.Create(
                response,
                new Group()
                {
                    Id = data.id,
                    Name = data.name,
                    DisplayOrder = data.displayOrder,
                    Members = data.members.Select(y => new User(y)).ToArray(),
                    Creator = (data.createdUser != null) ? new User(data.createdUser) : null,
                    Created = data.created,
                });
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
                new Group()
                {
                    Id = data.id,
                    Name = data.name,
                    DisplayOrder = data.displayOrder,
                    Members = data.members.Select(y => new User(y)).ToArray(),
                    Creator = (data.createdUser != null) ? new User(data.createdUser) : null,
                    Created = data.created,
                });
        }

        public async Task<BacklogResponse<Group>> DeleteGroupAsync(Group group)
        {
            var response = await DeleteAsync<_Group>($"/api/v2/groups/{group.Id}").ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<Group>.Create(
                response,
                new Group()
                {
                    Id = data.id,
                    Name = data.name,
                    DisplayOrder = data.displayOrder,
                    Members = data.members.Select(y => new User(y)).ToArray(),
                    Creator = (data.createdUser != null) ? new User(data.createdUser) : null,
                    Created = data.created,
                });
        }

        struct _Group
        {
            public int id { get; set; }
            public string name { get; set; }
            public List<_User> members { get; set; }
            public int displayOrder { get; set; }
            public _User createdUser { get; set; }
            public DateTime created { get; set; }
            public _User updatedUser { get; set; }
            public DateTime updated { get; set; }
        }
    }
}
