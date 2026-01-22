using Microsoft.UI.Xaml.Data;
using System;

namespace Sphere_Schedule_App.Converters
{
    public class NumberFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is double doubleValue)
            {
                return $"{doubleValue:0}%";
            }

            if (value is int intValue)
            {
                return $"{intValue}%";
            }

            if (value is decimal decimalValue)
            {
                return $"{decimalValue:0}%";
            }

            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}