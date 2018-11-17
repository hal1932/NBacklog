using System;
using System.Linq;

namespace NBacklog.DataTypes
{
    public class Group : CachableBacklogItem
    {
        public string Name { get; set; }
        public User[] Members { get; set; }
        public int DisplayOrder { get; set; }
        public User Creator { get; set; }
        public DateTime Created { get; set; }
        public User LastUpdater { get; set; }
        public DateTime LastUpdated { get; set; }

        internal Group(_Group data, BacklogClient client)
            : base(data.id)
        {
            Name = data.name;
            Members = data.members.Select(x => client.ItemsCache.Get(x.id, () => new User(x, client))).ToArray();
            DisplayOrder = data.displayOrder;
            Creator = client.ItemsCache.Get(data.createdUser.id, () => new User(data.createdUser, client));
            Created = data.created;
            LastUpdater = client.ItemsCache.Get(data.updatedUser.id, () => new User(data.updatedUser, client));
            LastUpdated = data.updated;
        }
    }
}
