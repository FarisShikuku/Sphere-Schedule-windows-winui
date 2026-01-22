using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;

namespace Sphere_Schedule_App.Converters
{
    public class ChangeToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string change)
            {
                if (change.StartsWith("+"))
                {
                    return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 76, 175, 80)); // Green for positive
                }
                else if (change.StartsWith("-"))
                {
                    return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 244, 67, 54)); // Red for negative
                }
            }
            return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 152, 0)); // Orange for neutral
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}