using ParentingTrackerApp.Helpers;
using System;
using Windows.UI.Xaml.Data;

namespace ParentingTrackerApp.Converters
{
    public class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var paramStr = parameter as string;
            if (string.IsNullOrWhiteSpace(paramStr))
            {
                return value.ToString();
            }
            else if (paramStr.ToLower() == "reldatetime")
            {
                var val = (DateTime)value;
                return val.ToRelativeDateTimeString();
            }
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }

    }
}
