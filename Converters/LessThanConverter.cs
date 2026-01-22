
using Microsoft.UI.Xaml.Data;
using System;

namespace Sphere_Schedule_App.Converters
{
    public class LessThanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is double actualValue && parameter != null)
            {
                if (double.TryParse(parameter.ToString(), out double threshold))
                {
                    return actualValue < threshold;
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
