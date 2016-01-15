using ParentingTrackerApp.Helpers;
using System;
using Windows.UI.Xaml.Data;

namespace ParentingTrackerApp.Converters
{
    public class DateTimeToDateStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((DateTime)value).ToShortDate();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
