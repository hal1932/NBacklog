using System.Drawing;

namespace NBacklog.DataTypes
{
    public enum TicketColor
    {
        Red = 0xe30000,
        Orange = 0x990000,
        Pink = 0x934981,
        Purple = 0x814fbc,
        Blue = 0x2779ca,
        Green = 0x007e9a,
        YellowGreen = 0x7ea800,
        Yellow = 0xff9200,
        PinkRed = 0xff3265,
        Black = 0x666665,
    }

    public class TicketType : CachableBacklogItem
    {
        public Project Project { get; }
        public string Name { get; set; }
        public Color Color { get; set; }
        public int DisplayOrder { get; }

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

        internal QueryParameters ToApiParameters()
        {
            var parameters = new QueryParameters();
            parameters.Add("name", Name);
            parameters.Add("color", $"#{Color.R:D2}{Color.G:D2}{Color.B:D2}");
            return parameters;
        }
    }
}
