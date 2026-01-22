using Microsoft.UI.Xaml.Data;
using System;

namespace Sphere_Schedule_App.Views.Controls
{
    public class ProgressToDashArrayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is double percentage)
            {
                return percentage / 100.0;
            }
            if (value is int intPercentage)
            {
                return intPercentage / 100.0;
            }
            return 0.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}