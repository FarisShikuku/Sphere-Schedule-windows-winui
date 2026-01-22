using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;

namespace Sphere_Schedule_App.Converters
{
    public class PriorityToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string priority)
            {
                return priority.ToLower() switch
                {
                    "critical" => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 244, 67, 54)),   // Red
                    "high" => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 87, 34)),      // Deep Orange
                    "medium" => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 152, 0)),     // Orange
                    "low" => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 76, 175, 80)),        // Green
                    _ => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 158, 158, 158))          // Grey
                };
            }
            return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 158, 158, 158));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}