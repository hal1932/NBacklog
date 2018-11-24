using NBacklog.DataTypes;
using System.Collections.Generic;
using System.ComponentModel;

namespace NBacklog.Query
{
    public class SharedFileQuery : Query<SharedFileQuery>
    {
        public const int MaxCount = 1000;

        internal string[] TypeNames { get; private set; } = new[] { "file" };

        public SharedFileQuery Offset(int value)
        {
            return ReplaceParameter("offset", value);
        }

        public SharedFileQuery Count(int value)
        {
            return ReplaceParameter("count", value);
        }

        public SharedFileQuery OrderBy(OrderType value)
        {
            var attribute = typeof(OrderType).GetMember(value.ToString())[0]
                .GetCustomAttributes(typeof(DescriptionAttribute), false)[0] as DescriptionAttribute;
            return ReplaceParameter("order", attribute.Description);
        }

        public SharedFileQuery FileType(SharedFileType type)
        {
            var typeNames = new List<string>();

            if (type.HasFlag(SharedFileType.File)) typeNames.Add("file");
            if (type.HasFlag(SharedFileType.Directory)) typeNames.Add("directory");

            TypeNames = typeNames.ToArray();
            return this;
        }
    }
}
