using System.Drawing;

namespace NBacklog.DataTypes
{
    public enum TicketColor
    {
        Red = -1900544,     // 0xffe30000,
        Orange = -6750208,  // 0xff990000,
        Pink = -7124607,    // 0xff934981,
        Purple = -8302660,  // 0xff814fbc,

        Blue = -14190134,   // 0xff2779ca,
        Green = -16744806,  // 0xff007e9a,
        YellowGreen = -8476672, // 0xff7ea800,
        Yellow = -28160,    // 0xffff9200,

        PinkRed = -52635,   // 0xffff3265,
        Black = -10066331,  // 0xff666665,
    }

    public class TicketType : CachableBacklogItem
    {
        public Project Project { get; }
        public string Name { get; set; }
        public Color Color { get; set; }
        public int DisplayOrder { get; }

        public TicketType(int id)
            : base(id)
        { }

        public TicketType(Project project, string name, TicketColor color)
            : base(-1)
        {
            Project = project;
            Name = name;
            Color = Color.FromArgb((int)color);
        }

        internal TicketType(_TicketType data, Project project)
            : base(data.id)
        {
            Project = project;
            Name = data.name;
            Color = Utils.ColorFromWebColorStr(data.color);
            DisplayOrder = data.displayOrder;
        }

        internal QueryParameters ToApiParameters(bool toCreate)
        {
            var parameters = new QueryParameters();
            parameters.Add("name", Name, toCreate);
            parameters.Add("color", Utils.WebColorStrFromColor(Color).ToLower(), toCreate);
            return parameters;
        }
    }
}
