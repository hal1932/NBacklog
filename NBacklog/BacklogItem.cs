namespace NBacklog
{
    public abstract class BacklogItem
    {
        public int Id { get; }

        private protected BacklogItem() { }

        private protected BacklogItem(int id)
        {
            Id = id;
        }
    }
}
