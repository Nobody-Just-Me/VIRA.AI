using Android.Content;
using System;
using System.Collections.Generic;
using VIRA.Mobile.SharedModels;

namespace VIRA.Mobile.SharedServices;

/// <summary>
/// Manages AI provider configuration including provider selection, models, and API keys
/// </summary>
public class ProviderConfigManager
{
    private const string TAG = "ProviderConfigManager";
    private const string PREFS_NAME = "vira_settings";
    private readonly ISharedPreferences _prefs;

    public enum AIProvider
    {
        GEMINI,
        GROQ,
        OPENAI
    }

    public ProviderConfigManager(Context context)
    {
        _prefs = context.GetSharedPreferences(PREFS_NAME, FileCreationMode.Private) 
            ?? throw new InvalidOperationException("Failed to get SharedPreferences");
    }

    /// <summary>
    /// Saves provider configuration to SharedPreferences
    /// </summary>
    public void SaveProviderConfig(ProviderConfiguration config)
    {
        try
        {
            var editor = _prefs.Edit();
            if (editor == null)
            {
                throw new InvalidOperationException("Failed to get SharedPreferences editor");
            }

            editor.PutString(ProviderConfiguration.PREF_KEY_PROVIDER, config.Provider);
            editor.PutString(ProviderConfiguration.PREF_KEY_MODEL, config.Model);
            
            // Store API key with provider-specific key
            var apiKeyKey = ProviderConfiguration.GetApiKeyPrefKey(config.Provider);
            editor.PutString(apiKeyKey, config.ApiKey);
            
            editor.Apply();
            
            Android.Util.Log.Info(TAG, $"Saved provider config: {config.Provider}, model: {config.Model}");
        }
        catch (Exception ex)
        {
            Android.Util.Log.Error(TAG, $"Failed to save provider config: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Loads provider configuration from SharedPreferences
    /// </summary>
    public ProviderConfiguration? LoadProviderConfig()
    {
        try
        {
            var provider = _prefs.GetString(ProviderConfiguration.PREF_KEY_PROVIDER, "gemini") ?? "gemini";
            var model = _prefs.GetString(ProviderConfiguration.PREF_KEY_MODEL, string.Empty) ?? string.Empty;
            
            // Load API key for the current provider
            var apiKeyKey = ProviderConfiguration.GetApiKeyPrefKey(provider);
            var apiKey = _prefs.GetString(apiKeyKey, string.Empty) ?? string.Empty;

            var config = new ProviderConfiguration(provider, model, apiKey);
            
            Android.Util.Log.Info(TAG, $"Loaded provider config: {provider}, model: {model}");
            return config;
        }
        catch (Exception ex)
        {
            Android.Util.Log.Error(TAG, $"Failed to load provider config: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Gets available models for a specific provider
    /// </summary>
    public List<string> GetAvailableModels(AIProvider provider)
    {
        return provider switch
        {
            AIProvider.GEMINI => new List<string> 
            { 
                "gemini-2.0-flash-exp",
                "gemini-1.5-pro", 
                "gemini-1.5-flash"
            },
            AIProvider.GROQ => new List<string> 
            { 
                "llama-3.3-70b-versatile",
                "mixtral-8x7b-32768"
            },
            AIProvider.OPENAI => new List<string> 
            { 
                "gpt-4o-mini",
                "gpt-4o",
                "gpt-4-turbo"
            },
            _ => new List<string>()
        };
    }

    /// <summary>
    /// Gets available models for a provider string
    /// </summary>
    public List<string> GetAvailableModels(string provider)
    {
        var providerEnum = provider.ToLower() switch
        {
            "gemini" => AIProvider.GEMINI,
            "groq" => AIProvider.GROQ,
            "openai" => AIProvider.OPENAI,
            _ => AIProvider.GEMINI
        };
        
        return GetAvailableModels(providerEnum);
    }

    /// <summary>
    /// Validates API key format (basic validation)
    /// </summary>
    public bool ValidateApiKey(string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return false;
        }

        // Basic validation: API keys should be at least 20 characters
        if (apiKey.Length < 20)
        {
            return false;
        }

        // Check for common prefixes
        var validPrefixes = new[] { "sk-", "gsk_", "AIza" };
        var hasValidPrefix = false;
        foreach (var prefix in validPrefixes)
        {
            if (apiKey.StartsWith(prefix))
            {
                hasValidPrefix = true;
                break;
            }
        }

        return hasValidPrefix;
    }
}
