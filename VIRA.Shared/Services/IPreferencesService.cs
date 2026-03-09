namespace VIRA.Shared.Services;

/// <summary>
/// Service for managing user preferences and app settings
/// </summary>
public interface IPreferencesService
{
    /// <summary>
    /// Check if this is the first time the app is launched
    /// </summary>
    bool IsFirstLaunch { get; }
    
    /// <summary>
    /// Check if the voice tutorial has been shown
    /// </summary>
    bool HasShownVoiceTutorial { get; set; }
    
    /// <summary>
    /// Check if continuous listening mode is enabled
    /// </summary>
    bool IsContinuousListeningEnabled { get; set; }
    
    /// <summary>
    /// Check if TTS (Text-to-Speech) is enabled
    /// </summary>
    bool IsTTSEnabled { get; set; }
    
    /// <summary>
    /// Mark the first launch as complete
    /// </summary>
    void MarkFirstLaunchComplete();
    
    /// <summary>
    /// Get a preference value
    /// </summary>
    T? Get<T>(string key, T? defaultValue = default);
    
    /// <summary>
    /// Set a preference value
    /// </summary>
    void Set<T>(string key, T value);
    
    /// <summary>
    /// Clear all preferences
    /// </summary>
    void Clear();
}
