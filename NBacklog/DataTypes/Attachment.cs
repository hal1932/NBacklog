namespace NBacklog.DataTypes
{
    public class Attachment : BacklogItem
    {
        public string Name { get; set; }
        public int Size { get; set; }

        internal Attachment(_Attachment data)
            : base(data.id)
        {
            Name = data.name;
            Size = data.size;
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
