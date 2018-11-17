namespace NBacklog.DataTypes
{
    public class AttributeInfo : BacklogItem
    {
        public string TypeId { get; set; }

        internal AttributeInfo(_AttributeInfo data)
            : base(data.id)
        {
            TypeId = data.typeId;
        }
    }
}
