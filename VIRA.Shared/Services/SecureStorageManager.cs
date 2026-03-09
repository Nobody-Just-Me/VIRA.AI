namespace VIRA.Shared.Services;

/// <summary>
/// AI Provider configuration
/// </summary>
public class AIConfig
{
    public string Provider { get; set; } = "Gemini";
    public string Model { get; set; } = "gemini-1.5-flash";
    public float Temperature { get; set; } = 0.7f;
    public int MaxTokens { get; set; } = 1000;
    public string ApiKey { get; set; } = string.Empty;
}

/// <summary>
/// Validation result for API keys
/// </summary>
public enum ValidationResult
{
    Valid,
    Invalid,
    NetworkError,
    Unknown
}

/// <summary>
/// Interface for secure storage operations
/// Platform-specific implementation required
/// </summary>
public interface ISecureStorageManager
{
    /// <summary>
    /// Save API key securely
    /// </summary>
    Task SaveApiKeyAsync(string provider, string apiKey);
    
    /// <summary>
    /// Get API key securely
    /// </summary>
    Task<string?> GetApiKeyAsync(string provider);
    
    /// <summary>
    /// Delete API key
    /// </summary>
    Task DeleteApiKeyAsync(string provider);
    
    /// <summary>
    /// Save AI configuration
    /// </summary>
    Task SaveAIConfigAsync(AIConfig config);
    
    /// <summary>
    /// Get AI configuration
    /// </summary>
    Task<AIConfig?> GetAIConfigAsync();
    
    /// <summary>
    /// Clear all AI configuration
    /// </summary>
    Task ClearAIConfigAsync();
}

/// <summary>
/// Default implementation using in-memory storage
/// Platform-specific implementations should use EncryptedSharedPreferences (Android)
/// or Keychain (iOS) or Data Protection API (Windows)
/// </summary>
public class SecureStorageManager : ISecureStorageManager
{
    private readonly Dictionary<string, string> _storage = new();
    private AIConfig? _currentConfig;

    /// <summary>
    /// Save API key securely
    /// </summary>
    public Task SaveApiKeyAsync(string provider, string apiKey)
    {
        var key = $"api_key_{provider.ToLower()}";
        _storage[key] = apiKey;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Get API key securely
    /// </summary>
    public Task<string?> GetApiKeyAsync(string provider)
    {
        var key = $"api_key_{provider.ToLower()}";
        _storage.TryGetValue(key, out var apiKey);
        return Task.FromResult(apiKey);
    }

    /// <summary>
    /// Delete API key
    /// </summary>
    public Task DeleteApiKeyAsync(string provider)
    {
        var key = $"api_key_{provider.ToLower()}";
        _storage.Remove(key);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Save AI configuration
    /// </summary>
    public Task SaveAIConfigAsync(AIConfig config)
    {
        _currentConfig = config;
        
        // Save individual settings
        _storage["ai_provider"] = config.Provider;
        _storage["ai_model"] = config.Model;
        _storage["ai_temperature"] = config.Temperature.ToString();
        _storage["ai_max_tokens"] = config.MaxTokens.ToString();
        
        // Save API key separately
        return SaveApiKeyAsync(config.Provider, config.ApiKey);
    }

    /// <summary>
    /// Get AI configuration
    /// </summary>
    public async Task<AIConfig?> GetAIConfigAsync()
    {
        if (_currentConfig != null)
        {
            return _currentConfig;
        }

        if (!_storage.TryGetValue("ai_provider", out var provider))
        {
            return null;
        }

        var config = new AIConfig
        {
            Provider = provider,
            Model = _storage.TryGetValue("ai_model", out var model) ? model : "gemini-1.5-flash",
            Temperature = _storage.TryGetValue("ai_temperature", out var temp) && float.TryParse(temp, out var t) ? t : 0.7f,
            MaxTokens = _storage.TryGetValue("ai_max_tokens", out var tokens) && int.TryParse(tokens, out var mt) ? mt : 1000,
            ApiKey = await GetApiKeyAsync(provider) ?? string.Empty
        };

        _currentConfig = config;
        return config;
    }

    /// <summary>
    /// Clear all AI configuration
    /// </summary>
    public async Task ClearAIConfigAsync()
    {
        if (_currentConfig != null)
        {
            await DeleteApiKeyAsync(_currentConfig.Provider);
        }

        _storage.Remove("ai_provider");
        _storage.Remove("ai_model");
        _storage.Remove("ai_temperature");
        _storage.Remove("ai_max_tokens");
        
        _currentConfig = null;
    }
}

/// <summary>
/// Service for validating API keys
/// </summary>
public class APIKeyValidator
{
    private readonly HttpClient _httpClient;

    public APIKeyValidator(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Validate Gemini API key
    /// </summary>
    public async Task<ValidationResult> ValidateGeminiKeyAsync(string apiKey)
    {
        try
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models?key={apiKey}";
            var response = await _httpClient.GetAsync(url);
            
            return response.IsSuccessStatusCode ? ValidationResult.Valid : ValidationResult.Invalid;
        }
        catch (HttpRequestException)
        {
            return ValidationResult.NetworkError;
        }
        catch
        {
            return ValidationResult.Unknown;
        }
    }

    /// <summary>
    /// Validate Groq API key
    /// </summary>
    public async Task<ValidationResult> ValidateGroqKeyAsync(string apiKey)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.groq.com/openai/v1/models");
            request.Headers.Add("Authorization", $"Bearer {apiKey}");
            
            var response = await _httpClient.SendAsync(request);
            
            return response.IsSuccessStatusCode ? ValidationResult.Valid : ValidationResult.Invalid;
        }
        catch (HttpRequestException)
        {
            return ValidationResult.NetworkError;
        }
        catch
        {
            return ValidationResult.Unknown;
        }
    }

    /// <summary>
    /// Validate OpenAI API key
    /// </summary>
    public async Task<ValidationResult> ValidateOpenAIKeyAsync(string apiKey)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.openai.com/v1/models");
            request.Headers.Add("Authorization", $"Bearer {apiKey}");
            
            var response = await _httpClient.SendAsync(request);
            
            return response.IsSuccessStatusCode ? ValidationResult.Valid : ValidationResult.Invalid;
        }
        catch (HttpRequestException)
        {
            return ValidationResult.NetworkError;
        }
        catch
        {
            return ValidationResult.Unknown;
        }
    }

    /// <summary>
    /// Validate API key for any provider
    /// </summary>
    public async Task<ValidationResult> ValidateApiKeyAsync(string provider, string apiKey)
    {
        return provider.ToLower() switch
        {
            "gemini" => await ValidateGeminiKeyAsync(apiKey),
            "groq" => await ValidateGroqKeyAsync(apiKey),
            "openai" => await ValidateOpenAIKeyAsync(apiKey),
            _ => ValidationResult.Unknown
        };
    }

    /// <summary>
    /// Get validation result message
    /// </summary>
    public string GetValidationMessage(ValidationResult result)
    {
        return result switch
        {
            ValidationResult.Valid => "✅ API key valid",
            ValidationResult.Invalid => "❌ API key tidak valid",
            ValidationResult.NetworkError => "⚠️ Tidak dapat terhubung ke server. Periksa koneksi internet Anda.",
            ValidationResult.Unknown => "❓ Tidak dapat memvalidasi API key",
            _ => "Unknown result"
        };
    }
}
