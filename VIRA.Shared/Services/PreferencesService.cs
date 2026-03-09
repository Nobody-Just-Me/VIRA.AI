using System.Text.Json;

namespace VIRA.Shared.Services;

/// <summary>
/// Implementation of preferences service using local storage
/// </summary>
public class PreferencesService : IPreferencesService
{
    private const string FirstLaunchKey = "first_launch";
    private const string VoiceTutorialKey = "voice_tutorial_shown";
    private const string ContinuousListeningKey = "continuous_listening";
    private const string TTSEnabledKey = "tts_enabled";
    private const string ActiveProviderKey = "active_provider";
    private const string VoiceOutputKey = "voice_output_enabled";
    private const string WebBrowsingKey = "web_browsing_enabled";
    private const string HapticsKey = "haptics_enabled";
    private const string NotificationsKey = "notifications_enabled";
    private const string MemoryModeKey = "memory_mode_enabled";
    private const string PrivacyModeKey = "privacy_mode_enabled";
    
    private readonly Dictionary<string, object> _cache = new();
    private readonly string _storageFilePath;

    public PreferencesService()
    {
        // Store preferences in app data folder
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var viraFolder = Path.Combine(appDataPath, "VIRA");
        
        if (!Directory.Exists(viraFolder))
        {
            Directory.CreateDirectory(viraFolder);
        }
        
        _storageFilePath = Path.Combine(viraFolder, "preferences.json");
        LoadPreferences();
    }

    public bool IsFirstLaunch => Get(FirstLaunchKey, true);

    public bool HasShownVoiceTutorial
    {
        get => Get(VoiceTutorialKey, false);
        set => Set(VoiceTutorialKey, value);
    }

    public bool IsContinuousListeningEnabled
    {
        get => Get(ContinuousListeningKey, false);
        set => Set(ContinuousListeningKey, value);
    }

    public bool IsTTSEnabled
    {
        get => Get(TTSEnabledKey, true);
        set => Set(TTSEnabledKey, value);
    }

    public void MarkFirstLaunchComplete()
    {
        Set(FirstLaunchKey, false);
    }

    public T? Get<T>(string key, T? defaultValue = default)
    {
        if (_cache.TryGetValue(key, out var value))
        {
            if (value is T typedValue)
            {
                return typedValue;
            }
            
            // Try to convert if types don't match
            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }
        
        return defaultValue;
    }

    public void Set<T>(string key, T value)
    {
        if (value == null)
        {
            _cache.Remove(key);
        }
        else
        {
            _cache[key] = value;
        }
        
        SavePreferences();
    }

    public void Clear()
    {
        _cache.Clear();
        SavePreferences();
    }
    
    // Provider configuration methods
    public string GetActiveProvider() => Get("active_provider", "groq") ?? "groq";
    public void SaveActiveProvider(string provider) => Set("active_provider", provider);
    
    public string GetSelectedModel(string provider) => Get($"{provider}_model", string.Empty) ?? string.Empty;
    public void SaveSelectedModel(string provider, string model) => Set($"{provider}_model", model);
    
    public async Task SaveApiKeyAsync(string provider, string apiKey)
    {
        // In production, this should use secure storage
        Set($"{provider}_api_key", apiKey);
        await Task.CompletedTask;
    }
    
    // Preference getters and setters
    public bool GetVoiceOutputEnabled() => Get(VoiceOutputKey, true);
    public void SaveVoiceOutputEnabled(bool enabled) => Set(VoiceOutputKey, enabled);
    
    public bool GetWebBrowsingEnabled() => Get(WebBrowsingKey, true);
    public void SaveWebBrowsingEnabled(bool enabled) => Set(WebBrowsingKey, enabled);
    
    public bool GetHapticsEnabled() => Get(HapticsKey, true);
    public void SaveHapticsEnabled(bool enabled) => Set(HapticsKey, enabled);
    
    public bool GetNotificationsEnabled() => Get(NotificationsKey, true);
    public void SaveNotificationsEnabled(bool enabled) => Set(NotificationsKey, enabled);
    
    public bool GetMemoryModeEnabled() => Get(MemoryModeKey, true);
    public void SaveMemoryModeEnabled(bool enabled) => Set(MemoryModeKey, enabled);
    
    public bool GetPrivacyModeEnabled() => Get(PrivacyModeKey, false);
    public void SavePrivacyModeEnabled(bool enabled) => Set(PrivacyModeKey, enabled);

    private void LoadPreferences()
    {
        try
        {
            if (File.Exists(_storageFilePath))
            {
                var json = File.ReadAllText(_storageFilePath);
                var loaded = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
                
                if (loaded != null)
                {
                    foreach (var kvp in loaded)
                    {
                        // Store as appropriate type
                        _cache[kvp.Key] = kvp.Value.ValueKind switch
                        {
                            JsonValueKind.True => true,
                            JsonValueKind.False => false,
                            JsonValueKind.Number => kvp.Value.GetDouble(),
                            JsonValueKind.String => kvp.Value.GetString() ?? string.Empty,
                            _ => kvp.Value.ToString()
                        };
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading preferences: {ex.Message}");
        }
    }

    private void SavePreferences()
    {
        try
        {
            var json = JsonSerializer.Serialize(_cache, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            
            File.WriteAllText(_storageFilePath, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving preferences: {ex.Message}");
        }
    }
}
