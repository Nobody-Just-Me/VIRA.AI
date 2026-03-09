using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.UI;

namespace VIRA.Shared.Converters;

/// <summary>
/// Converts a hex color string (e.g., "#8b5cf6") to a Color object
/// </summary>
public class StringToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is string colorString && !string.IsNullOrEmpty(colorString))
        {
            try
            {
                // Remove # if present
                colorString = colorString.TrimStart('#');
                
                // Parse hex color
                if (colorString.Length == 6)
                {
                    byte r = System.Convert.ToByte(colorString.Substring(0, 2), 16);
                    byte g = System.Convert.ToByte(colorString.Substring(2, 2), 16);
                    byte b = System.Convert.ToByte(colorString.Substring(4, 2), 16);
                    return Color.FromArgb(255, r, g, b);
                }
                else if (colorString.Length == 8)
                {
                    byte a = System.Convert.ToByte(colorString.Substring(0, 2), 16);
                    byte r = System.Convert.ToByte(colorString.Substring(2, 2), 16);
                    byte g = System.Convert.ToByte(colorString.Substring(4, 2), 16);
                    byte b = System.Convert.ToByte(colorString.Substring(6, 2), 16);
                    return Color.FromArgb(a, r, g, b);
                }
            }
            catch
            {
                // Return default color on error
                return Color.FromArgb(255, 139, 92, 246); // Purple
            }
        }
        
        // Default color
        return Color.FromArgb(255, 139, 92, 246); // Purple
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
