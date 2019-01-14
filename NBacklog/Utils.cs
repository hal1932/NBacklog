using System.Drawing;
using System.Globalization;

namespace NBacklog
{
    internal static class Utils
    {
        public static Color ColorFromWebColorStr(string value)
        {
            if (value.StartsWith("#"))
            {
                value = value.Substring(1);
            }
            return Color.FromArgb(
                int.Parse(value.Substring(0, 2), NumberStyles.HexNumber),
                int.Parse(value.Substring(2, 2), NumberStyles.HexNumber),
                int.Parse(value.Substring(4, 2), NumberStyles.HexNumber));
        }

        public static string WebColorStrFromColor(Color value)
        {
            return $"#{value.R:X2}{value.G:X2}{value.B:X2}";
        }
    }
}
