namespace NBacklog
{
    public abstract class BacklogItem
    {
        public int Id { get; }

        protected BacklogItem() { }

        protected BacklogItem(int id)
        {
            Id = id;
        }
    }
}
