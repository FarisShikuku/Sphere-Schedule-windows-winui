using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;

namespace Sphere_Schedule_App.Converters
{
    public class CategoryToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string category)
            {
                return category.ToLower() switch
                {
                    "work" => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 33, 150, 243)),     // Blue
                    "personal" => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 76, 175, 80)),  // Green
                    "health" => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 244, 67, 54)),    // Red
                    "education" => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 156, 39, 176)), // Purple
                    "shopping" => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 152, 0)),   // Orange
                    "finance" => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 121, 85, 72)),    // Brown
                    "entertainment" => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 233, 30, 99)), // Pink
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