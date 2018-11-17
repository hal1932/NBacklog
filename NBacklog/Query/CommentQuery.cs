using System.ComponentModel;

namespace NBacklog.Query
{
    public class CommentQuery : Query<CommentQuery>
    {
        public CommentQuery MinId(int id)
        {
            return AddParameter("minId", id);
        }

        public CommentQuery MaxId(int id)
        {
            return AddParameter("maxId", id);
        }

        public CommentQuery IdRange(int min, int max)
        {
            return MinId(min).MaxId(max);
        }

        public CommentQuery Count(int value)
        {
            return AddParameter("count", value);
        }

        public CommentQuery OrderBy(OrderType value)
        {
            var attribute = typeof(OrderType).GetMember(value.ToString())[0]
                .GetCustomAttributes(typeof(DescriptionAttribute), false)[0] as DescriptionAttribute;
            return AddParameter("order", attribute.Description);
        }
    }
}
