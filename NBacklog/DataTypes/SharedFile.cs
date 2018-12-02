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
            Size = data.size ?? default(long);
            Creator = project.Client.ItemsCache.Get(data.createdUser?.id, () => new User(data.createdUser, project.Client));
            Created = data.created ?? default(DateTime);
            LastUpdated = data.updated ?? default(DateTime);
            LastUpdater = project.Client.ItemsCache.Get(data.updatedUser?.id, () => new User(data.updatedUser, project.Client));
        }

        internal SharedFile(int id, string name, long size, Project project)
            : base(id)
        {
            Name = name;
            Size = size;
        }

        public async Task<BacklogResponse<MemoryStream>> DownloadAsync()
        {
            var response = await Project.Client.GetAsync($"/api/v2/projects/{Project.Id}/files/{Id}").ConfigureAwait(false);
            return await Project.Client.CreateResponseAsync(
                response,
                HttpStatusCode.OK,
                data => new MemoryStream(data)).ConfigureAwait(false);
        }
    }
}
