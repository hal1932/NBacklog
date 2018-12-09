namespace NBacklog.Query
{
    public class GroupQuery : Query<GroupQuery>
    {
        public const int MaxCount = 100;

        public GroupQuery Offset(int value)
        {
            return ReplaceParameter("offset", value);
        }

        public GroupQuery Count(int value)
        {
            return ReplaceParameter("count", value);
        }

        public GroupQuery OrderBy(OrderType value)
        {
            return ReplaceParameter("order", GetEnumDesc(value));
        }
    }
}
