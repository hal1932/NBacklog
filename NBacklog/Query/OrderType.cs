using System.ComponentModel;

namespace NBacklog.Query
{
    public enum OrderType
    {
        [Description("desc")] Desc,
        [Description("asc")] Asc,
    }
}
