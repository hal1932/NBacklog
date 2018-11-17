namespace NBacklog
{
    public abstract class BacklogItem
    {
        public int Id { get; }

        protected BacklogItem(int id)
        {
            Id = id;
        }
    }
}
