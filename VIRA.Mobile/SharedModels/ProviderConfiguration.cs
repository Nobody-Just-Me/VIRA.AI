using System;

namespace VIRA.Mobile.SharedModels;

/// <summary>
/// Represents AI provider configuration including provider type, model, and API key
/// </summary>
public class ProviderConfiguration
{
    public string Provider { get; set; } // "gemini", "groq", "openai"
    public string Model { get; set; }
    public string ApiKey { get; set; }
    public long LastUpdated { get; set; }

    // SharedPreferences keys
    public const string PREF_KEY_PROVIDER = "ai_provider";
    public const string PREF_KEY_MODEL = "ai_model";
    public const string PREF_KEY_API_KEY_PREFIX = "api_key_";

    public ProviderConfiguration()
    {
        Provider = "gemini";
        Model = string.Empty;
        ApiKey = string.Empty;
        LastUpdated = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    public ProviderConfiguration(string provider, string model, string apiKey)
    {
        Provider = provider;
        Model = model;
        ApiKey = apiKey;
        LastUpdated = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// Gets the SharedPreferences key for the API key based on provider
    /// </summary>
    public static string GetApiKeyPrefKey(string provider)
    {
        return $"{PREF_KEY_API_KEY_PREFIX}{provider}";
    }
}
