using System;

namespace NBacklog.DataTypes
{
    public class SharedFile : BacklogItem
    {
        public string Type { get; set; }
        public string Dir { get; set; }
        public string Name { get; set; }
        public int Size { get; set; }
        public User Creator { get; set; }
        public DateTime Created { get; set; }
        public User LastUpdater { get; set; }
        public DateTime LastUpdated { get; set; }

        internal SharedFile(_Sharedfile data, BacklogClient client)
            : base(data.id)
        {
            Type = data.type;
            Dir = data.dir;
            Name = data.name;
            Size = data.size;
            Creator = client.ItemsCache.Get(data.createdUser?.id, () => new User(data.createdUser));
            Created = data.created ?? default(DateTime);
            LastUpdated = data.updated ?? default(DateTime);
            LastUpdater = client.ItemsCache.Get(data.updatedUser?.id, () => new User(data.updatedUser));
        }
    }
}
