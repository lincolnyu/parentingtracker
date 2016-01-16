using System;
using Windows.UI.Xaml.Data;
using ParentingTrackerApp.Helpers;

namespace ParentingTrackerApp.Converters
{
    public class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var paramStr = parameter as string;
            if (string.IsNullOrWhiteSpace(paramStr))
            {
                return ((DateTime)value).ToNotTooLongString();
            }
            else if (paramStr.ToLower() == "reldatetime")
            {   // TODO this is not used
                var val = (DateTime)value;
                return val.ToRelativeDateTimeString();
            }
            return ((DateTime)value).ToNotTooLongString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
