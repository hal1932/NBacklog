namespace NBacklog.DataTypes
{
    public class Category : CachableBacklogItem
    {
        public string Name { get; set; }
        public int DisplayOrder { get; }

        public Category(int id)
            : base(id)
        { }

        public Category(string name)
            : base(-1)
        {
            Name = name;
        }

        internal Category(_Category data)
            : base(data.id)
        {
            Name = data.name;
            DisplayOrder = data.displayOrder;
        }

        internal QueryParameters ToApiParameters()
        {
            var parameters = new QueryParameters();
            parameters.Add("name", Name);
            return parameters;
        }
    }
}
