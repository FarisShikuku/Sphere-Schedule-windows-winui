using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;

namespace Sphere_Schedule_App.Converters
{
    public class StringToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string colorHex)
            {
                try
                {
                    return new SolidColorBrush(ParseColor(colorHex));
                }
                catch
                {
                    return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 33, 150, 243)); // Default blue
                }
            }
            return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 33, 150, 243));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        private Windows.UI.Color ParseColor(string colorHex)
        {
            colorHex = colorHex.Replace("#", "");

            if (colorHex.Length == 6)
            {
                return Windows.UI.Color.FromArgb(255,
                    byte.Parse(colorHex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                    byte.Parse(colorHex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                    byte.Parse(colorHex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));
            }
            else if (colorHex.Length == 8)
            {
                return Windows.UI.Color.FromArgb(
                    byte.Parse(colorHex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                    byte.Parse(colorHex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                    byte.Parse(colorHex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber),
                    byte.Parse(colorHex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber));
            }

            throw new ArgumentException("Invalid color format");
        }
    }
}