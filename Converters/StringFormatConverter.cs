using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using Windows.UI; // Add this line

namespace Sphere_Schedule_App.Converters
{
    public class StringFormatConverter : IValueConverter
    {
        private static readonly Dictionary<string, SolidColorBrush> StatusBrushes = new()
        {
            { "Scheduled", new SolidColorBrush(Color.FromArgb(255, 0, 120, 212)) },        // #0078D4
            { "InProgress", new SolidColorBrush(Color.FromArgb(255, 16, 124, 16)) },       // #107C10
            { "Completed", new SolidColorBrush(Color.FromArgb(255, 80, 80, 80)) },         // #505050
            { "Cancelled", new SolidColorBrush(Color.FromArgb(255, 209, 52, 56)) },        // #D13438
            { "Critical", new SolidColorBrush(Color.FromArgb(255, 227, 0, 140)) },         // #E3008C
            { "In Progress", new SolidColorBrush(Color.FromArgb(255, 16, 124, 16)) }       // #107C10
        };

        private static readonly Dictionary<string, SolidColorBrush> PriorityBrushes = new()
        {
            { "Low", new SolidColorBrush(Color.FromArgb(255, 16, 124, 16)) },              // #107C10
            { "Normal", new SolidColorBrush(Color.FromArgb(255, 0, 120, 212)) },           // #0078D4
            { "High", new SolidColorBrush(Color.FromArgb(255, 255, 140, 0)) },             // #FF8C00
            { "Critical", new SolidColorBrush(Color.FromArgb(255, 209, 52, 56)) }          // #D13438
        };

        private static readonly Dictionary<string, SolidColorBrush> PlatformBrushes = new()
        {
            { "Google Meet", new SolidColorBrush(Color.FromArgb(255, 52, 168, 83)) },      // #34A853
            { "Microsoft Teams", new SolidColorBrush(Color.FromArgb(255, 98, 100, 167)) }, // #6264A7
            { "Zoom", new SolidColorBrush(Color.FromArgb(255, 45, 140, 255)) },            // #2D8CFF
            { "Webex", new SolidColorBrush(Color.FromArgb(255, 250, 71, 0)) },             // #FA4700
            { "Skype", new SolidColorBrush(Color.FromArgb(255, 0, 175, 240)) },            // #00AFF0
            { "Custom", new SolidColorBrush(Color.FromArgb(255, 107, 91, 149)) }           // #6B5B95
        };

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return null;

            string valueStr = value.ToString();

            if (parameter is string format)
            {
                // Handle brush conversions
                if (format.EndsWith("Brush"))
                {
                    // Platform brush: {0}Brush
                    if (format == "{0}Brush")
                    {
                        if (PlatformBrushes.TryGetValue(valueStr, out SolidColorBrush brush))
                            return brush;
                    }
                    // Status brush: Status{0}Brush
                    else if (format == "Status{0}Brush")
                    {
                        if (StatusBrushes.TryGetValue(valueStr, out SolidColorBrush brush))
                            return brush;
                    }
                    // Priority brush: Priority{0}Brush
                    else if (format == "Priority{0}Brush")
                    {
                        if (PriorityBrushes.TryGetValue(valueStr, out SolidColorBrush brush))
                            return brush;
                    }
                    // Direct brush name from resources
                    else if (format == "StatusBrush" && StatusBrushes.TryGetValue(valueStr, out SolidColorBrush statusBrush))
                    {
                        return statusBrush;
                    }
                    else if (format == "PriorityBrush" && PriorityBrushes.TryGetValue(valueStr, out SolidColorBrush priorityBrush))
                    {
                        return priorityBrush;
                    }
                }

                // Handle icon selection for boolean values
                if (format.Contains("&#x"))
                {
                    try
                    {
                        bool boolValue = (bool)value;
                        string[] parts = format.Split(':');
                        if (parts.Length == 2)
                            return boolValue ? parts[0] : parts[1];
                    }
                    catch
                    {
                        return format;
                    }
                }

                // Handle template selection
                if (format.Contains("MeetingItemTemplate"))
                {
                    try
                    {
                        bool isCompact = (bool)value;
                        string[] parts = format.Split(':');
                        if (parts.Length == 2)
                            return isCompact ? parts[0] : parts[1];
                    }
                    catch
                    {
                        return format;
                    }
                }

                // Handle view mode text
                if (format.Contains("{0:Compact:Detailed}"))
                {
                    try
                    {
                        bool isCompact = (bool)value;
                        return isCompact ? "Compact" : "Detailed";
                    }
                    catch
                    {
                        return format;
                    }
                }

                // Handle count formatting
                if (format.Contains("{0} meetings"))
                {
                    try
                    {
                        int count = System.Convert.ToInt32(value);
                        return $"{count} meeting{(count == 1 ? "" : "s")}";
                    }
                    catch
                    {
                        return format;
                    }
                }

                // Handle participants formatting
                if (format.Contains("{0} participants"))
                {
                    try
                    {
                        int count = System.Convert.ToInt32(value);
                        return $"{count} participant{(count == 1 ? "" : "s")}";
                    }
                    catch
                    {
                        return format;
                    }
                }

                // Original string formatting
                try
                {
                    return string.Format(format, value);
                }
                catch
                {
                    return valueStr;
                }
            }

            return valueStr;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}