using Microsoft.UI.Xaml.Data;
using System;

namespace Sphere_Schedule_App.Converters
{
    public class DateTimeFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime dateTimeValue)
            {
                return FormatDateTime(dateTimeValue, parameter);
            }

            // Fix: Use pattern matching for nullable DateTime correctly
            if (value is DateTime nullableDateTime)
            {
                return FormatDateTime(nullableDateTime, parameter);
            }

            return string.Empty;
        }

        private string FormatDateTime(DateTime dateTime, object parameter)
        {
            var format = parameter as string;
            if (!string.IsNullOrEmpty(format))
            {
                if (format.StartsWith("Due: "))
                {
                    var actualFormat = format.Substring(5);
                    return $"Due: {dateTime.ToString(actualFormat)}";
                }
                return dateTime.ToString(format);
            }
            return dateTime.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}