namespace NBacklog.DataTypes
{
    public class Category : CachableBacklogItem
    {
        public string Name { get; set; }
        public int DisplayOrder { get; set; }

        internal Category(_Category data)
            : base(data.id)
        {
            Name = data.name;
            DisplayOrder = data.displayOrder;
        }
    }
}
