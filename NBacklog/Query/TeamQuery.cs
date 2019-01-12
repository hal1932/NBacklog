namespace NBacklog.Query
{
    public class TeamQuery : Query<TeamQuery>
    {
        public const int MaxCount = 100;

        public TeamQuery Offset(int value)
        {
            return ReplaceParameter("offset", value);
        }

        public TeamQuery Count(int value)
        {
            return ReplaceParameter("count", value);
        }

        public TeamQuery OrderBy(OrderType value)
        {
            return ReplaceParameter("order", GetEnumDesc(value));
        }
    }
}
