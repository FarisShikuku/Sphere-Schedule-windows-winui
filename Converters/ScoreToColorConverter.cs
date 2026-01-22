using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;

namespace Sphere_Schedule_App.Converters
{
    public class ScoreToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int score)
            {
                if (score >= 80)
                    return Windows.UI.Color.FromArgb(255, 76, 175, 80);    // Green for excellent
                else if (score >= 60)
                    return Windows.UI.Color.FromArgb(255, 255, 152, 0);    // Orange for good
                else if (score >= 40)
                    return Windows.UI.Color.FromArgb(255, 255, 193, 7);    // Yellow for fair
                else
                    return Windows.UI.Color.FromArgb(255, 244, 67, 54);    // Red for poor
            }
            return Windows.UI.Color.FromArgb(255, 158, 158, 158);          // Grey default
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}