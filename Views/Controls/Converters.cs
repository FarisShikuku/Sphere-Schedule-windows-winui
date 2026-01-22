using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;

namespace Sphere_Schedule_App.Views.Controls
{
    public class AppointmentTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string type)
            {
                return type.ToLower() switch
                {
                    "health" => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 244, 67, 54)),     // Red
                    "business" => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 33, 150, 243)),  // Blue
                    "personal" => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 76, 175, 80)),   // Green
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