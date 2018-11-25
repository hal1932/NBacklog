using System.ComponentModel;

namespace NBacklog.Query
{
    public class StarQuery : Query<StarQuery>
    {
        public const int MaxCount = 100;

        public StarQuery MinId(int id)
        {
            return ReplaceParameter("minId", id);
        }

        public StarQuery MaxId(int id)
        {
            return ReplaceParameter("maxId", id);
        }

        public StarQuery IdRange(int min, int max)
        {
            return MinId(min).MaxId(max);
        }

        public StarQuery Count(int value)
        {
            return ReplaceParameter("count", value);
        }

        public StarQuery OrderBy(OrderType value)
        {
            var attribute = typeof(OrderType).GetMember(value.ToString())[0]
                .GetCustomAttributes(typeof(DescriptionAttribute), false)[0] as DescriptionAttribute;
            return ReplaceParameter("order", attribute.Description);
        }
    }
}
