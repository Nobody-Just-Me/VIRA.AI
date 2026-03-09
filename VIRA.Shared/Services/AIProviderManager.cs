using VIRA.Shared.Models;

namespace VIRA.Shared.Services;

/// <summary>
/// Manages multiple AI providers and handles provider selection, routing, and fallback.
/// Orchestrates communication between the application and various AI services.
/// </summary>
public class AIProviderManager
{
    private readonly Dictionary<string, IProviderService> _providers;
    private readonly ISecureStorageManager _storageManager;
    private string _activeProvider = "Groq";
    private string _fallbackProvider = string.Empty;

    public AIProviderManager(ISecureStorageManager storageManager)
    {
        _storageManager = storageManager;
        _providers = new Dictionary<string, IProviderService>();
        
        // Load saved provider preferences
        LoadProviderPreferencesAsync().Wait();
    }

    /// <summary>
    /// Registers a provider with the manager
    /// </summary>
    public void RegisterProvider(IProviderService provider)
    {
        if (!_providers.ContainsKey(provider.ProviderName))
        {
            _providers[provider.ProviderName] = provider;
        }
    }

    /// <summary>
    /// Sets the active provider for AI requests
    /// </summary>
    public async Task SetActiveProviderAsync(string providerName)
    {
        if (!_providers.ContainsKey(providerName))
        {
            throw new ArgumentException($"Provider '{providerName}' is not registered.");
        }

        _activeProvider = providerName;
        await SaveProviderPreferencesAsync();
    }
    
    /// <summary>
    /// Sets the active provider for AI requests (synchronous)
    /// </summary>
    public void SetActiveProvider(string providerName)
    {
        if (!_providers.ContainsKey(providerName))
        {
            throw new ArgumentException($"Provider '{providerName}' is not registered.");
        }

        _activeProvider = providerName;
        // Note: Preferences will be saved on next async operation
    }

    /// <summary>
    /// Sets the fallback provider for when the active provider fails
    /// </summary>
    public async Task SetFallbackProviderAsync(string providerName)
    {
        if (!string.IsNullOrEmpty(providerName) && !_providers.ContainsKey(providerName))
        {
            throw new ArgumentException($"Provider '{providerName}' is not registered.");
        }

        _fallbackProvider = providerName;
        await SaveProviderPreferencesAsync();
    }

    /// <summary>
    /// Gets the currently active provider name
    /// </summary>
    public string GetActiveProviderName()
    {
        return _activeProvider;
    }

    /// <summary>
    /// Gets the fallback provider name
    /// </summary>
    public string GetFallbackProviderName()
    {
        return _fallbackProvider;
    }

    /// <summary>
    /// Gets a list of all registered provider names
    /// </summary>
    public List<string> GetRegisteredProviders()
    {
        return _providers.Keys.ToList();
    }

    /// <summary>
    /// Gets a list of all configured (with API keys) provider names
    /// </summary>
    public List<string> GetConfiguredProviders()
    {
        return _providers
            .Where(p => p.Value.IsConfigured)
            .Select(p => p.Key)
            .ToList();
    }

    /// <summary>
    /// Gets a specific provider by name
    /// </summary>
    public IProviderService? GetProvider(string providerName)
    {
        return _providers.TryGetValue(providerName, out var provider) ? provider : null;
    }

    /// <summary>
    /// Gets the active provider instance
    /// </summary>
    public IProviderService? GetActiveProvider()
    {
        return GetProvider(_activeProvider);
    }

    /// <summary>
    /// Validates API key before saving
    /// </summary>
    public async Task<bool> ValidateAndSaveApiKeyAsync(string providerName, string apiKey)
    {
        if (!_providers.ContainsKey(providerName))
        {
            throw new ArgumentException($"Provider '{providerName}' is not registered.");
        }

        var provider = _providers[providerName];
        
        // Temporarily set the API key for validation
        var providerType = provider.GetType();
        var setApiKeyMethod = providerType.GetMethod("SetApiKeyAsync");
        
        if (setApiKeyMethod != null)
        {
            await (Task)setApiKeyMethod.Invoke(provider, new object[] { apiKey })!;
            
            // Validate the connection
            var isValid = await provider.ValidateConnectionAsync();
            
            if (!isValid)
            {
                // Clear the invalid key
                await (Task)setApiKeyMethod.Invoke(provider, new object[] { string.Empty })!;
            }
            
            return isValid;
        }

        return false;
    }

    /// <summary>
    /// Sends a message using the active provider with automatic fallback
    /// </summary>
    public async Task<ChatMessage> SendMessageAsync(string message, List<ChatMessage> history)
    {
        try
        {
            var provider = GetActiveProvider();
            if (provider == null)
            {
                throw new InvalidOperationException("No active provider is configured.");
            }

            if (!provider.IsConfigured)
            {
                throw new InvalidOperationException($"{provider.ProviderName} is not configured. Please set your API key in Settings.");
            }

            return await provider.SendMessageAsync(message, history);
        }
        catch (Exception ex)
        {
            // Attempt fallback to secondary provider
            if (!string.IsNullOrEmpty(_fallbackProvider) && _fallbackProvider != _activeProvider)
            {
                try
                {
                    var fallbackProvider = GetProvider(_fallbackProvider);
                    if (fallbackProvider != null && fallbackProvider.IsConfigured)
                    {
                        return await fallbackProvider.SendMessageAsync(message, history);
                    }
                }
                catch
                {
                    // Fallback also failed, throw original exception
                }
            }

            throw new Exception($"Failed to send message: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Sends a message using the active provider with streaming and automatic fallback
    /// </summary>
    public async Task<Stream> SendMessageStreamAsync(string message, List<ChatMessage> history)
    {
        try
        {
            var provider = GetActiveProvider();
            if (provider == null)
            {
                throw new InvalidOperationException("No active provider is configured.");
            }

            if (!provider.IsConfigured)
            {
                throw new InvalidOperationException($"{provider.ProviderName} is not configured. Please set your API key in Settings.");
            }

            return await provider.SendMessageStreamAsync(message, history);
        }
        catch (Exception ex)
        {
            // Attempt fallback to secondary provider
            if (!string.IsNullOrEmpty(_fallbackProvider) && _fallbackProvider != _activeProvider)
            {
                try
                {
                    var fallbackProvider = GetProvider(_fallbackProvider);
                    if (fallbackProvider != null && fallbackProvider.IsConfigured)
                    {
                        return await fallbackProvider.SendMessageStreamAsync(message, history);
                    }
                }
                catch
                {
                    // Fallback also failed, throw original exception
                }
            }

            throw new Exception($"Failed to stream message: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Gets available models for the active provider
    /// </summary>
    public List<string> GetAvailableModels()
    {
        var provider = GetActiveProvider();
        return provider?.GetAvailableModels() ?? new List<string>();
    }

    /// <summary>
    /// Gets available models for a specific provider
    /// </summary>
    public List<string> GetAvailableModels(string providerName)
    {
        var provider = GetProvider(providerName);
        return provider?.GetAvailableModels() ?? new List<string>();
    }

    /// <summary>
    /// Validates connection for a specific provider
    /// </summary>
    public async Task<bool> ValidateProviderConnectionAsync(string providerName)
    {
        var provider = GetProvider(providerName);
        if (provider == null)
        {
            return false;
        }

        return await provider.ValidateConnectionAsync();
    }

    /// <summary>
    /// Loads provider preferences from storage
    /// </summary>
    private async Task LoadProviderPreferencesAsync()
    {
        try
        {
            var config = await _storageManager.GetAIConfigAsync();
            if (config != null && !string.IsNullOrEmpty(config.Provider))
            {
                _activeProvider = config.Provider;
            }
        }
        catch
        {
            // Use defaults if loading fails
        }
    }

    /// <summary>
    /// Saves provider preferences to storage
    /// </summary>
    private async Task SaveProviderPreferencesAsync()
    {
        try
        {
            var config = await _storageManager.GetAIConfigAsync() ?? new AIConfig();
            config.Provider = _activeProvider;
            await _storageManager.SaveAIConfigAsync(config);
        }
        catch
        {
            // Silently fail if saving fails
        }
    }
}
