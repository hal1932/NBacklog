namespace NBacklog.Query
{
    public class CommentQuery : Query<CommentQuery>
    {
        public const int MaxCount = 100;

        public CommentQuery MinId(int id)
        {
            return ReplaceParameter("minId", id);
        }

        public CommentQuery MaxId(int id)
        {
            return ReplaceParameter("maxId", id);
        }

        public CommentQuery IdRange(int min, int max)
        {
            return MinId(min).MaxId(max);
        }

        public CommentQuery Count(int value)
        {
            return ReplaceParameter("count", value);
        }

        public CommentQuery OrderBy(OrderType value)
        {
            return ReplaceParameter("order", GetEnumDesc(value));
        }
    }
}
