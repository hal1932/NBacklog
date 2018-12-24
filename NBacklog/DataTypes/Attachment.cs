using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace NBacklog.DataTypes
{
    public class AttachmentInfo : BacklogItem
    {
        public string Name { get; set; }

        internal AttachmentInfo(_AttachmentInfo data)
            : base(data.id)
        {
            Name = data.name;
        }
    }

    public class Attachment : BacklogItem
    {
        public string Name { get; set; }
        public long Size { get; set; }
        public User Creator { get; }
        public DateTime Created { get; }

        internal Attachment(_Attachment data, Ticket ticket)
            : this(data, ticket.Project.Client)
        {
            _ticket = ticket;
        }

        internal Attachment(_Attachment data, Wikipage wikipage)
            : this(data, wikipage.Project.Client)
        {
            _wikipage = wikipage;
        }

        internal Attachment(_Attachment data, PullRequest pullRequest)
            : this(data, pullRequest.Repository.Project.Client)
        {
            _pullRequest = pullRequest;
        }

        internal Attachment(JObject data, Ticket ticket)
            : this(data)
        {
            _ticket = ticket;
        }

        internal Attachment(_Attachment data, BacklogClient client)
            : base(data.id)
        {
            Name = data.name;
            Size = data.size;
            Creator = client.ItemsCache.Update(data.createdUser?.id, () => new User(data.createdUser, client));
            Created = data.created ?? default;
            _client = client;
        }

        internal Attachment(JObject data)
            : base(data.Value<int>("id"))
        {
            Name = data.Value<string>("name");
            Size = data.Value<long>("size");
        }

        public async Task<BacklogResponse<MemoryStream>> DownloadAsync()
        {
            string resource = default;
            if (_ticket != null)
            {
                resource = $"/api/v2/issues/{_ticket.Id}/attachments/{Id}";
            }
            else if (_wikipage != null)
            {
                resource = $"/api/v2/wikis/{_wikipage.Id}/attachments/{Id}";
            }
            else if (_pullRequest != null)
            {
                var repo = _pullRequest.Repository;
                var proj = repo.Project;
                resource = $"/api/v2/projects/{proj.Id}/git/repositories/{repo.Id}/pullRequests/{_pullRequest.Id}/attachments/{Id}";
            }

            if (resource != default)
            {
                throw new InvalidOperationException("invalid client or resource");
            }

            var response = await _client.GetAsync(resource).ConfigureAwait(false);

            return await _client.CreateResponseAsync(
                response,
                HttpStatusCode.OK,
                data => new MemoryStream(data))
                .ConfigureAwait(false);
        }

        private BacklogClient _client;
        private Ticket _ticket;
        private Wikipage _wikipage;
        private PullRequest _pullRequest;
    }
}
