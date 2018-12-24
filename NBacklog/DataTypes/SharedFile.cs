using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace NBacklog.DataTypes
{
    [Flags]
    public enum SharedFileType
    {
        Directory,
        File,
    }

    public class SharedFileSummary : BacklogItem
    {
        public SharedFileType Type { get; }
        public string TypeName { get; }
        public string Dir { get; }
        public string Name { get; }
        public long Size { get; }
        public UserSummary Creator { get; }
        public DateTime Created { get; }
        public UserSummary LastUpdater { get; }
        public DateTime LastUpdated { get; }

        internal SharedFileSummary(_SharedFile data)
            : base(data.id)
        {
            TypeName = data.type;
            switch (TypeName)
            {
                case "file": Type = SharedFileType.File; break;
                case "directory": Type = SharedFileType.Directory; break;
                default:
                    throw new ArgumentException($"unsupported file type: {Type}");
            }

            Dir = data.dir;
            Name = data.name;
            Size = data.size ?? default;
            Creator = (data.createdUser != null) ? new UserSummary(data.createdUser) : null;
            Created = data.created ?? default;
            LastUpdated = data.updated ?? default;
            LastUpdater = (data.updatedUser != null) ? new UserSummary(data.updatedUser) : null;
        }
    }

    public class SharedFile : CachableBacklogItem
    {
        public Project Project { get; }
        public SharedFileType Type { get; }
        public string TypeName { get; }
        public string Dir { get; }
        public string Name { get; }
        public long Size { get; }
        public User Creator { get; }
        public DateTime Created { get; }
        public User LastUpdater { get; }
        public DateTime LastUpdated { get; }

        internal SharedFile(_SharedFile data, Wikipage wikipage)
            : this(data, wikipage.Project)
        {
            _wikipage = wikipage;
        }

        internal SharedFile(_SharedFile data, Project project)
            : base(data.id)
        {
            Project = project;

            TypeName = data.type;
            switch (TypeName)
            {
                case "file": Type = SharedFileType.File; break;
                case "directory": Type = SharedFileType.Directory; break;
                default:
                    throw new ArgumentException($"unsupported file type: {Type}");
            }

            Dir = data.dir;
            Name = data.name;
            Size = data.size ?? default;
            Creator = project.Client.ItemsCache.Update(data.createdUser?.id, () => new User(data.createdUser, project.Client));
            Created = data.created ?? default;
            LastUpdated = data.updated ?? default;
            LastUpdater = project.Client.ItemsCache.Update(data.updatedUser?.id, () => new User(data.updatedUser, project.Client));
        }

        internal SharedFile(int id, string name, long size, Project project)
            : base(id)
        {
            Name = name;
            Size = size;
        }

        public async Task<BacklogResponse<MemoryStream>> DownloadAsync()
        {
            string resource;
            if (_wikipage != null)
            {
                resource = $"/api/v2/wikis/{_wikipage.Id}/sharedFiles/{Id}";
            }
            else
            {
                resource = $"/api/v2/projects/{Project.Id}/files/{Id}";
            }

            var response = await Project.Client.GetAsync(resource).ConfigureAwait(false);
            return await Project.Client.CreateResponseAsync(
                response,
                HttpStatusCode.OK,
                data => new MemoryStream(data)).ConfigureAwait(false);
        }

        private Wikipage _wikipage;
    }
}
