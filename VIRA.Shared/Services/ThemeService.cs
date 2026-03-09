using Microsoft.UI;
using Windows.UI;

namespace VIRA.Shared.Services;

/// <summary>
/// Provides centralized theme constants for the VIRA Modern UI redesign.
/// All values match the reference design from "AI Assistant Mobile UI" folder.
/// </summary>
public static class ThemeService
{
    /// <summary>
    /// Color constants from the reference design
    /// </summary>
    public static class Colors
    {
        // Base colors
        public const string Background = "#101622";
        public const string PurpleGradientStart = "#8b5cf6";
        public const string PurpleGradientEnd = "#7c3aed";
        public const string IndigoAccent = "#6366f1";
        
        // Semi-transparent whites for glassmorphism
        public const string SemiTransparentWhite5 = "#0DFFFFFF";  // 5% opacity
        public const string SemiTransparentWhite8 = "#14FFFFFF";  // 8% opacity
        public const string SemiTransparentWhite14 = "#24FFFFFF"; // 14% opacity
        
        // Message bubbles
        public const string UserMessageGradient = "#8b5cf6";
        public const string AIMessageBackground = "#0DFFFFFF"; // 5% opacity
        
        // Status colors
        public const string SuccessGreen = "#22c55e";
        public const string WarningYellow = "#eab308";
        public const string ErrorRed = "#ef4444";
        public const string InfoBlue = "#2094f3";
        
        // Text colors
        public const string TextPrimary = "#F1F5F9";
        public const string TextSecondary = "#cbd5e1";
        public const string TextTertiary = "#94a3b8";
        public const string TextQuaternary = "#64748b";
        public const string TextWhite = "#FFFFFF";
        
        // Ambient glow colors
        public const string AmbientGlowPurple = "#8b5cf6";
        public const string AmbientGlowIndigo = "#6366f1";
    }
    
    /// <summary>
    /// Spacing constants for consistent layout
    /// </summary>
    public static class Spacing
    {
        public const double XSmall = 4;
        public const double Small = 8;
        public const double Medium = 16;
        public const double Large = 24;
        public const double XLarge = 32;
        public const double XXLarge = 48;
    }
    
    /// <summary>
    /// Border radius constants for rounded corners
    /// </summary>
    public static class BorderRadius
    {
        public const double Small = 8;
        public const double Medium = 16;
        public const double Large = 24;
        public const double XLarge = 32;
    }
    
    /// <summary>
    /// Shadow constants for depth effects
    /// </summary>
    public static class Shadows
    {
        public const double MessageShadowBlur = 24;
        public const double MessageShadowOpacity = 0.35;
        public const double MessageShadowOffsetX = 0;
        public const double MessageShadowOffsetY = 8;
        
        public const double CardShadowBlur = 16;
        public const double CardShadowOpacity = 0.25;
        public const double CardShadowOffsetX = 0;
        public const double CardShadowOffsetY = 4;
    }
    
    /// <summary>
    /// Animation timing constants
    /// </summary>
    public static class Animation
    {
        public const int MessageEntranceDuration = 280;
        public const int SidebarSlideDuration = 300;
        public const int ButtonTapDuration = 200;
        public const int WaveformUpdateInterval = 50;
        public const int NotificationDismissDelay = 3000;
        public const int VoiceButtonPulseDuration = 500;
    }
    
    /// <summary>
    /// Helper method to convert hex color string to Windows.UI.Color
    /// </summary>
    public static Color HexToColor(string hex)
    {
        hex = hex.Replace("#", "");
        
        byte a = 255;
        byte r, g, b;
        
        if (hex.Length == 8)
        {
            a = Convert.ToByte(hex.Substring(0, 2), 16);
            r = Convert.ToByte(hex.Substring(2, 2), 16);
            g = Convert.ToByte(hex.Substring(4, 2), 16);
            b = Convert.ToByte(hex.Substring(6, 2), 16);
        }
        else if (hex.Length == 6)
        {
            r = Convert.ToByte(hex.Substring(0, 2), 16);
            g = Convert.ToByte(hex.Substring(2, 2), 16);
            b = Convert.ToByte(hex.Substring(4, 2), 16);
        }
        else
        {
            throw new ArgumentException("Invalid hex color format");
        }
        
        return Color.FromArgb(a, r, g, b);
    }
}
