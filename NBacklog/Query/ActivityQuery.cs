using NBacklog.DataTypes;
using System.ComponentModel;

namespace NBacklog.Query
{
    public class ActivityQuery : Query<ActivityQuery>
    {
        public const int MaxCount = 100;

        public ActivityQuery Type(ActivityType type)
        {
            return AddParameter("activityTypeId[]", type);
        }

        public ActivityQuery MinId(int id)
        {
            return ReplaceParameter("minId", id);
        }

        public ActivityQuery MaxId(int id)
        {
            return ReplaceParameter("maxId", id);
        }

        public ActivityQuery IdRange(int min, int max)
        {
            return MinId(min).MaxId(max);
        }

        public ActivityQuery Count(int value)
        {
            return ReplaceParameter("count", value);
        }

        public ActivityQuery OrderBy(OrderType value)
        {
            var attribute = typeof(OrderType).GetMember(value.ToString())[0]
                .GetCustomAttributes(typeof(DescriptionAttribute), false)[0] as DescriptionAttribute;
            return ReplaceParameter("order", attribute.Description);
        }
    }
}
