using Xunit;
using FsCheck;
using FsCheck.Xunit;
using VIRA.Shared.Services;
using Windows.UI;

namespace VIRA.Shared.Tests;

/// <summary>
/// Tests for the ThemeService and theme resource loading
/// </summary>
public class ThemeTests
{
    [Fact]
    public void ThemeService_Colors_ShouldMatchReferenceDesign()
    {
        // Verify base colors
        Assert.Equal("#101622", ThemeService.Colors.Background);
        Assert.Equal("#8b5cf6", ThemeService.Colors.PurpleGradientStart);
        Assert.Equal("#7c3aed", ThemeService.Colors.PurpleGradientEnd);
        Assert.Equal("#6366f1", ThemeService.Colors.IndigoAccent);
    }
    
    [Fact]
    public void ThemeService_Colors_ShouldHaveSemiTransparentWhites()
    {
        // Verify semi-transparent white colors exist
        Assert.Equal("#0DFFFFFF", ThemeService.Colors.SemiTransparentWhite5);
        Assert.Equal("#14FFFFFF", ThemeService.Colors.SemiTransparentWhite8);
        Assert.Equal("#24FFFFFF", ThemeService.Colors.SemiTransparentWhite14);
    }
    
    [Fact]
    public void ThemeService_Colors_ShouldHaveStatusColors()
    {
        // Verify status colors
        Assert.Equal("#22c55e", ThemeService.Colors.SuccessGreen);
        Assert.Equal("#eab308", ThemeService.Colors.WarningYellow);
        Assert.Equal("#ef4444", ThemeService.Colors.ErrorRed);
        Assert.Equal("#2094f3", ThemeService.Colors.InfoBlue);
    }
    
    [Fact]
    public void ThemeService_Colors_ShouldHaveTextColors()
    {
        // Verify text colors
        Assert.Equal("#F1F5F9", ThemeService.Colors.TextPrimary);
        Assert.Equal("#cbd5e1", ThemeService.Colors.TextSecondary);
        Assert.Equal("#94a3b8", ThemeService.Colors.TextTertiary);
        Assert.Equal("#64748b", ThemeService.Colors.TextQuaternary);
        Assert.Equal("#FFFFFF", ThemeService.Colors.TextWhite);
    }
    
    [Fact]
    public void ThemeService_Spacing_ShouldHaveCorrectValues()
    {
        // Verify spacing values
        Assert.Equal(4, ThemeService.Spacing.XSmall);
        Assert.Equal(8, ThemeService.Spacing.Small);
        Assert.Equal(16, ThemeService.Spacing.Medium);
        Assert.Equal(24, ThemeService.Spacing.Large);
        Assert.Equal(32, ThemeService.Spacing.XLarge);
        Assert.Equal(48, ThemeService.Spacing.XXLarge);
    }
    
    [Fact]
    public void ThemeService_BorderRadius_ShouldHaveCorrectValues()
    {
        // Verify border radius values
        Assert.Equal(8, ThemeService.BorderRadius.Small);
        Assert.Equal(16, ThemeService.BorderRadius.Medium);
        Assert.Equal(24, ThemeService.BorderRadius.Large);
        Assert.Equal(32, ThemeService.BorderRadius.XLarge);
    }
    
    [Fact]
    public void ThemeService_Shadows_ShouldHaveCorrectValues()
    {
        // Verify shadow values
        Assert.Equal(24, ThemeService.Shadows.MessageShadowBlur);
        Assert.Equal(0.35, ThemeService.Shadows.MessageShadowOpacity);
        Assert.Equal(0, ThemeService.Shadows.MessageShadowOffsetX);
        Assert.Equal(8, ThemeService.Shadows.MessageShadowOffsetY);
        
        Assert.Equal(16, ThemeService.Shadows.CardShadowBlur);
        Assert.Equal(0.25, ThemeService.Shadows.CardShadowOpacity);
        Assert.Equal(0, ThemeService.Shadows.CardShadowOffsetX);
        Assert.Equal(4, ThemeService.Shadows.CardShadowOffsetY);
    }
    
    [Fact]
    public void ThemeService_Animation_ShouldHaveCorrectDurations()
    {
        // Verify animation durations
        Assert.Equal(280, ThemeService.Animation.MessageEntranceDuration);
        Assert.Equal(300, ThemeService.Animation.SidebarSlideDuration);
        Assert.Equal(200, ThemeService.Animation.ButtonTapDuration);
        Assert.Equal(50, ThemeService.Animation.WaveformUpdateInterval);
        Assert.Equal(3000, ThemeService.Animation.NotificationDismissDelay);
        Assert.Equal(500, ThemeService.Animation.VoiceButtonPulseDuration);
    }
    
    [Fact]
    public void ThemeService_HexToColor_ShouldConvertCorrectly()
    {
        // Test 6-digit hex conversion
        var color1 = ThemeService.HexToColor("#101622");
        Assert.Equal(255, color1.A);
        Assert.Equal(16, color1.R);
        Assert.Equal(22, color1.G);
        Assert.Equal(34, color1.B);
        
        // Test 8-digit hex conversion (with alpha)
        var color2 = ThemeService.HexToColor("#0DFFFFFF");
        Assert.Equal(13, color2.A);
        Assert.Equal(255, color2.R);
        Assert.Equal(255, color2.G);
        Assert.Equal(255, color2.B);
    }
    
    [Fact]
    public void ThemeService_HexToColor_ShouldHandleWithoutHashPrefix()
    {
        // Test without # prefix
        var color = ThemeService.HexToColor("8b5cf6");
        Assert.Equal(255, color.A);
        Assert.Equal(139, color.R);
        Assert.Equal(92, color.G);
        Assert.Equal(246, color.B);
    }
    
    [Fact]
    public void ThemeService_HexToColor_ShouldThrowOnInvalidFormat()
    {
        // Test invalid format
        Assert.Throws<ArgumentException>(() => ThemeService.HexToColor("#12345"));
    }
    
    #region Property-Based Tests
    
    /// <summary>
    /// Property 1: For any theme color constant, the value should match the reference design specification
    /// **Validates: Requirements 11.1**
    /// </summary>
    [Property(DisplayName = "Feature: vira-modern-ui-redesign, Property 1: Theme color values match reference design", MaxTest = 100)]
    public Property ThemeColorValues_ShouldMatchReferenceDesign()
    {
        // Define reference design color specifications
        var referenceColors = new Dictionary<string, string>
        {
            { "Background", "#101622" },
            { "PurpleGradientStart", "#8b5cf6" },
            { "PurpleGradientEnd", "#7c3aed" },
            { "IndigoAccent", "#6366f1" },
            { "SemiTransparentWhite5", "#0DFFFFFF" },
            { "SemiTransparentWhite8", "#14FFFFFF" },
            { "SemiTransparentWhite14", "#24FFFFFF" },
            { "SuccessGreen", "#22c55e" },
            { "WarningYellow", "#eab308" },
            { "ErrorRed", "#ef4444" },
            { "InfoBlue", "#2094f3" },
            { "TextPrimary", "#F1F5F9" },
            { "TextSecondary", "#cbd5e1" },
            { "TextTertiary", "#94a3b8" },
            { "TextQuaternary", "#64748b" },
            { "TextWhite", "#FFFFFF" }
        };
        
        // Property: For any color in the reference design, the ThemeService should return the exact same value
        return Prop.ForAll(
            Gen.Elements(referenceColors.Keys.ToArray()).ToArbitrary(),
            colorName =>
            {
                var expectedValue = referenceColors[colorName];
                var actualValue = GetThemeColorValue(colorName);
                
                return actualValue.Equals(expectedValue, StringComparison.OrdinalIgnoreCase);
            });
    }
    
    /// <summary>
    /// Helper method to get theme color value by property name using reflection
    /// </summary>
    private string GetThemeColorValue(string colorName)
    {
        var colorsType = typeof(ThemeService.Colors);
        var property = colorsType.GetProperty(colorName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
        
        if (property == null)
        {
            throw new ArgumentException($"Color property '{colorName}' not found in ThemeService.Colors");
        }
        
        var value = property.GetValue(null);
        return value?.ToString() ?? string.Empty;
    }
    
    #endregion
}
