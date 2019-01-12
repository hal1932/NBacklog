using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NBacklog.DataTypes
{
    public class Team : CachableBacklogItem
    {
        public string Name { get; set; }
        public User[] Members { get; set; }
        public int DisplayOrder { get; }
        public User Creator { get; }
        public DateTime Created { get; }
        public User LastUpdater { get; }
        public DateTime LastUpdated { get; }

        internal Team(_Team data, BacklogClient client)
            : base(data.id)
        {
            Name = data.name;
            Members = data.members.Select(x => client.ItemsCache.Update(x.id, () => new User(x, client))).ToArray();
            DisplayOrder = data.displayOrder;
            Creator = client.ItemsCache.Update(data.createdUser?.id, () => new User(data.createdUser, client));
            Created = data.created ?? default;
            LastUpdater = client.ItemsCache.Update(data.updatedUser?.id, () => new User(data.updatedUser, client));
            LastUpdated = data.updated ?? default;
            _client = client;
        }

        public async Task<BacklogResponse<MemoryStream>> GetIconAsync()
        {
            var response = await _client.GetAsync($"/api/v2/teams/{Id}/icon").ConfigureAwait(false);
            return await _client.CreateResponseAsync(
                response,
                HttpStatusCode.OK,
                data => new MemoryStream(data)).ConfigureAwait(false);
        }

        private BacklogClient _client;
    }
}
