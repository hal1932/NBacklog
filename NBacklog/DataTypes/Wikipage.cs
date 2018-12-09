using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NBacklog.DataTypes
{
    public class Wikipage : BacklogItem
    {
        public Project Project { get; }
        public int ProjectId { get; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string[] Tags { get; set; }
        public Attachment[] Attachments { get; }
        public SharedFile[] SharedFiles { get; }
        public Star[] Stars { get; }
        public User Creator { get; }
        public DateTime Created { get; }
        public User LastUpdater { get; }
        public DateTime LastUpdated { get; }

        public Wikipage(string name, string content, string[] tags = null)
            : base(-1)
        {
            Name = name;
            Content = content;
            Tags = tags ?? Array.Empty<string>();
        }

        internal Wikipage(_Wikipage data, Project project)
            : base(data.id)
        {
            var client = project.Client;

            ProjectId = data.projectId;
            Name = data.name;
            Content = data.content;
            Tags = data.tags.Select(x => x.name).ToArray();
            Attachments = data.attachments.Select(x => new Attachment(x, this)).ToArray();
            SharedFiles = data.sharedFiles.Select(x => client.ItemsCache.Update(x.id, () => new SharedFile(x, this))).ToArray();
            Stars = data.stars.Select(x => new Star(x, client)).ToArray();
            Creator = client.ItemsCache.Update(data.createdUser?.id, () => new User(data.createdUser, client));
            Created = data.created ?? default;
            LastUpdater = client.ItemsCache.Update(data.updatedUser?.id, () => new User(data.updatedUser, client));
            LastUpdated = data.updated ?? default;

            Project = project;
        }

        public async Task<BacklogResponse<string>> GetContentAsync()
        {
            var client = Project.Client;

            var response = await client.GetAsync($"/api/v2/wikis/{Id}").ConfigureAwait(false);
            var result = await client.CreateResponseAsync<Wikipage, _Wikipage>(
                response,
                HttpStatusCode.OK,
                data => new Wikipage(data, Project)).ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                return new BacklogResponse<string>(result.StatusCode, result.Errors);
            }

            Content = result.Content.Content;
            return new BacklogResponse<string>(result.StatusCode, Content);
        }
    }
}
