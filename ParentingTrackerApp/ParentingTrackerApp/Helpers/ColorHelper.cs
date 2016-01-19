using Windows.UI;

namespace ParentingTrackerApp.Helpers
{
    public static class ColorHelper
    {
        public static double GetY(this Color color)
        {
            var r = color.R / 255.0;
            var g = color.G / 255.0;
            var b = color.B / 255.0;
            var y = 0.299 * r + 0.587 * g + 0.114 * b;
            return y;
        }

        public static string ToHtmlColor(this Color color)
        {
            var s = string.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B);
            return s;
        }

        public static Color GetConstrastingBlackOrWhite(this Color input)
        {
            var y = input.GetY();
            var output = (y < 0.5) ? Colors.White : Colors.Black;
            return output;
        }
    }
}
