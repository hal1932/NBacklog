using Newtonsoft.Json.Linq;

namespace NBacklog.DataTypes
{
    public class Attachment : BacklogItem
    {
        public string Name { get; set; }
        public long Size { get; set; }

        internal Attachment(_Attachment data)
            : base(data.id)
        {
            Name = data.name;
            Size = data.size;
        }

        internal Attachment(JObject data)
            : base(data.Value<int>("id"))
        {
            Name = data.Value<string>("name");
            Size = data.Value<long>("size");
        }
    }

    public class AttachmentInfo : BacklogItem
    {
        public string Name { get; set; }

        internal AttachmentInfo(_AttachmentInfo data)
            : base(data.id)
        {
            Name = data.name;
        }
    }
}
