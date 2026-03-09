namespace VIRA.Mobile.SharedModels;

/// <summary>
/// Represents theme preference options for the application
/// </summary>
public enum ThemePreference
{
    LIGHT,
    DARK,
    SYSTEM
}

/// <summary>
/// Helper class for ThemePreference constants
/// </summary>
public static class ThemePreferenceConstants
{
    public const string PREF_KEY = "theme_preference";
}
