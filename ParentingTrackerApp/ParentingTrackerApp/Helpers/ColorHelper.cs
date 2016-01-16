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
    }
}
