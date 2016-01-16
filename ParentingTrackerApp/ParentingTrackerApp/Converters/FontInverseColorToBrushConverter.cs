using ParentingTrackerApp.Helpers;
using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace ParentingTrackerApp.Converters
{
    public class FontInverseColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var input = (Color)value;
            var y = input.GetY();
            var output = (y < 0.5) ? Colors.White : Colors.Black;
            return new SolidColorBrush(output);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
