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
        }

        internal Attachment(JObject data)
            : base(data.Value<int>("id"))
        {
            Name = data.Value<string>("name");
            Size = data.Value<long>("size");
        }

        public async Task<BacklogResponse<MemoryStream>> DownloadAsync()
        {
            BacklogClient client = default;
            string resource = default;
            if (_ticket != null)
            {
                client = _ticket.Project.Client;
                resource = $"/api/v2/issues/{_ticket.Id}/attachments/{Id}";
            }
            else if (_wikipage != null)
            {
                client = _wikipage.Project.Client;
                resource = $"/api/v2/wikis/{_wikipage.Id}/attachments/{Id}";
            }

            if (client == default || resource != default)
            {
                throw new InvalidOperationException("invalid client or resource");
            }

            var response = await client.GetAsync(resource).ConfigureAwait(false);

            return await _ticket.Project.Client.CreateResponseAsync(
                response,
                HttpStatusCode.OK,
                data => new MemoryStream(data))
                .ConfigureAwait(false);
        }

        private Ticket _ticket;
        private Wikipage _wikipage;
    }
}
