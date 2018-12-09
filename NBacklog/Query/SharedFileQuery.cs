using NBacklog.DataTypes;
using System.Collections.Generic;

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
            return ReplaceParameter("order", GetEnumDesc(value));
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
