namespace NBacklog.DataTypes
{
    public class ChangeLog : BacklogItem
    {
        public string Field { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public AttachmentInfo AttachmentInfo { get; set; }
        public AttributeInfo AttributeInfo { get; set; }
        public NotificationInfo Notificationinfo { get; set; }

        internal ChangeLog(_ChangeLog data)
            : base(-1)
        {
            Field = data.field;
            OldValue = data.originalValue;
            NewValue = data.newValue;
            AttachmentInfo = (data.attachmentInfo != null) ? new AttachmentInfo(data.attachmentInfo) : null;
            AttributeInfo = (data.attributeInfo != null) ? new AttributeInfo(data.attributeInfo) : null;
            Notificationinfo = (data.notificationInfo != null) ? new NotificationInfo(data.notificationInfo) : null;
        }
    }
}
