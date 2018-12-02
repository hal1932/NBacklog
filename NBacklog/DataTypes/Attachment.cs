using Newtonsoft.Json.Linq;
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

        internal Attachment(_Attachment data, Ticket ticket)
            : base(data.id)
        {
            Name = data.name;
            Size = data.size;
            _ticket = ticket;
        }

        internal Attachment(JObject data, Ticket ticket)
            : base(data.Value<int>("id"))
        {
            Name = data.Value<string>("name");
            Size = data.Value<long>("size");
            _ticket = ticket;
        }

        public async Task<BacklogResponse<MemoryStream>> DownloadAsync()
        {
            var response = await _ticket.Project.Client
                .GetAsync($"/api/v2/issues/{_ticket.Id}/attachments/{Id}")
                .ConfigureAwait(false);

            return await _ticket.Project.Client.CreateResponseAsync(
                response,
                HttpStatusCode.OK,
                data => new MemoryStream(data))
                .ConfigureAwait(false);
        }

        private Ticket _ticket;
    }
}
