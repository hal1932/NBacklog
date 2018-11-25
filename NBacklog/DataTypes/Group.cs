﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NBacklog.DataTypes
{
    public class Group : CachableBacklogItem
    {
        public string Name { get; set; }
        public User[] Members { get; set; }
        public int DisplayOrder { get; }
        public User Creator { get; }
        public DateTime Created { get; }
        public User LastUpdater { get; }
        public DateTime LastUpdated { get; }

        public Group(string name, User[] members)
            : base(-1)
        {
            Name = name;
            Members = members;
        }

        internal Group(_Group data, BacklogClient client)
            : base(data.id)
        {
            Name = data.name;
            Members = data.members.Select(x => client.ItemsCache.Get(x.id, () => new User(x, client))).ToArray();
            DisplayOrder = data.displayOrder;
            Creator = client.ItemsCache.Get(data.createdUser?.id, () => new User(data.createdUser, client));
            Created = data.created ?? default(DateTime);
            LastUpdater = client.ItemsCache.Get(data.updatedUser?.id, () => new User(data.updatedUser, client));
            LastUpdated = data.updated ?? default(DateTime);
            _client = client;
        }

        internal Group(int id, string name, BacklogClient client)
            : base(id)
        {
            Name = name;
        }

        public async Task<BacklogResponse<MemoryStream>> GetIconAsync()
        {
            var response = await _client.GetAsync($"/api/v2/groups/{Id}/icon");
            return _client.CreateResponse(
                response,
                HttpStatusCode.OK,
                data => new MemoryStream(data));
        }

        private BacklogClient _client;
    }
}
