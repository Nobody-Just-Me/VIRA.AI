using Xunit;
using VIRA.Shared.Services;
using VIRA.Shared.Models;

namespace VIRA.Shared.Tests;

/// <summary>
/// Unit tests for AIProviderManager
/// Tests provider registration, selection, routing, and fallback mechanisms
/// **Validates: Requirements 9.4, 9.5, 9.6, 9.7 (Provider orchestration)**
/// </summary>
public class AIProviderManagerTests
{
    private readonly SecureStorageManager _storageManager;
    private readonly HttpClient _httpClient;

    public AIProviderManagerTests()
    {
        _storageManager = new SecureStorageManager();
        _httpClient = new HttpClient();
    }

    #region Provider Registration Tests

    [Fact]
    public void RegisterProvider_ShouldAddProviderToManager()
    {
        // Arrange
        var manager = new AIProviderManager(_storageManager);
        var provider = new GroqProvider(_httpClient, _storageManager);

        // Act
        manager.RegisterProvider(provider);
        var registeredProviders = manager.GetRegisteredProviders();

        // Assert
        Assert.Contains("Groq", registeredProviders);
    }

    [Fact]
    public void RegisterProvider_MultipleProviders_ShouldAddAll()
    {
        // Arrange
        var manager = new AIProviderManager(_storageManager);
        var groqProvider = new GroqProvider(_httpClient, _storageManager);
        var openAIProvider = new OpenAIProvider(_httpClient, _storageManager);
        var geminiProvider = new GeminiProvider(_httpClient, _storageManager);

        // Act
        manager.RegisterProvider(groqProvider);
        manager.RegisterProvider(openAIProvider);
        manager.RegisterProvider(geminiProvider);
        var registeredProviders = manager.GetRegisteredProviders();

        // Assert
        Assert.Equal(3, registeredProviders.Count);
        Assert.Contains("Groq", registeredProviders);
        Assert.Contains("OpenAI", registeredProviders);
        Assert.Contains("Gemini", registeredProviders);
    }

    #endregion

    #region Provider Selection Tests

    [Fact]
    public async Task SetActiveProviderAsync_WithValidProvider_ShouldSucceed()
    {
        // Arrange
        var manager = new AIProviderManager(_storageManager);
        var provider = new OpenAIProvider(_httpClient, _storageManager);
        manager.RegisterProvider(provider);

        // Act
        await manager.SetActiveProviderAsync("OpenAI");
        var activeProvider = manager.GetActiveProviderName();

        // Assert
        Assert.Equal("OpenAI", activeProvider);
    }

    [Fact]
    public async Task SetActiveProviderAsync_WithInvalidProvider_ShouldThrowException()
    {
        // Arrange
        var manager = new AIProviderManager(_storageManager);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await manager.SetActiveProviderAsync("NonExistentProvider")
        );
    }

    [Fact]
    public async Task SetFallbackProviderAsync_WithValidProvider_ShouldSucceed()
    {
        // Arrange
        var manager = new AIProviderManager(_storageManager);
        var groqProvider = new GroqProvider(_httpClient, _storageManager);
        var openAIProvider = new OpenAIProvider(_httpClient, _storageManager);
        manager.RegisterProvider(groqProvider);
        manager.RegisterProvider(openAIProvider);

        // Act
        await manager.SetFallbackProviderAsync("OpenAI");
        var fallbackProvider = manager.GetFallbackProviderName();

        // Assert
        Assert.Equal("OpenAI", fallbackProvider);
    }

    [Fact]
    public async Task SetFallbackProviderAsync_WithInvalidProvider_ShouldThrowException()
    {
        // Arrange
        var manager = new AIProviderManager(_storageManager);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await manager.SetFallbackProviderAsync("NonExistentProvider")
        );
    }

    #endregion

    #region Provider Retrieval Tests

    [Fact]
    public void GetProvider_WithValidName_ShouldReturnProvider()
    {
        // Arrange
        var manager = new AIProviderManager(_storageManager);
        var provider = new GroqProvider(_httpClient, _storageManager);
        manager.RegisterProvider(provider);

        // Act
        var retrievedProvider = manager.GetProvider("Groq");

        // Assert
        Assert.NotNull(retrievedProvider);
        Assert.Equal("Groq", retrievedProvider.ProviderName);
    }

    [Fact]
    public void GetProvider_WithInvalidName_ShouldReturnNull()
    {
        // Arrange
        var manager = new AIProviderManager(_storageManager);

        // Act
        var retrievedProvider = manager.GetProvider("NonExistent");

        // Assert
        Assert.Null(retrievedProvider);
    }

    [Fact]
    public void GetActiveProvider_ShouldReturnCurrentActiveProvider()
    {
        // Arrange
        var manager = new AIProviderManager(_storageManager);
        var provider = new GroqProvider(_httpClient, _storageManager);
        manager.RegisterProvider(provider);

        // Act
        var activeProvider = manager.GetActiveProvider();

        // Assert
        Assert.NotNull(activeProvider);
        Assert.Equal("Groq", activeProvider.ProviderName);
    }

    #endregion

    #region Configured Providers Tests

    [Fact]
    public async Task GetConfiguredProviders_ShouldReturnOnlyConfiguredProviders()
    {
        // Arrange
        var manager = new AIProviderManager(_storageManager);
        var groqProvider = new GroqProvider(_httpClient, _storageManager);
        var openAIProvider = new OpenAIProvider(_httpClient, _storageManager);
        
        manager.RegisterProvider(groqProvider);
        manager.RegisterProvider(openAIProvider);
        
        await groqProvider.SetApiKeyAsync("gsk_test123");

        // Act
        var configuredProviders = manager.GetConfiguredProviders();

        // Assert
        Assert.Single(configuredProviders);
        Assert.Contains("Groq", configuredProviders);
        Assert.DoesNotContain("OpenAI", configuredProviders);
    }

    #endregion

    #region API Key Validation Tests

    [Fact]
    public async Task ValidateAndSaveApiKeyAsync_WithInvalidProvider_ShouldThrowException()
    {
        // Arrange
        var manager = new AIProviderManager(_storageManager);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await manager.ValidateAndSaveApiKeyAsync("NonExistent", "test_key")
        );
    }

    #endregion

    #region Message Sending Tests

    [Fact]
    public async Task SendMessageAsync_WithoutConfiguredProvider_ShouldThrowException()
    {
        // Arrange
        var manager = new AIProviderManager(_storageManager);
        var provider = new GroqProvider(_httpClient, _storageManager);
        manager.RegisterProvider(provider);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await manager.SendMessageAsync("Hello", new List<ChatMessage>())
        );
    }

    [Fact]
    public async Task SendMessageStreamAsync_WithoutConfiguredProvider_ShouldThrowException()
    {
        // Arrange
        var manager = new AIProviderManager(_storageManager);
        var provider = new GroqProvider(_httpClient, _storageManager);
        manager.RegisterProvider(provider);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await manager.SendMessageStreamAsync("Hello", new List<ChatMessage>())
        );
    }

    #endregion

    #region Model Retrieval Tests

    [Fact]
    public void GetAvailableModels_WithActiveProvider_ShouldReturnModels()
    {
        // Arrange
        var manager = new AIProviderManager(_storageManager);
        var provider = new GroqProvider(_httpClient, _storageManager);
        manager.RegisterProvider(provider);

        // Act
        var models = manager.GetAvailableModels();

        // Assert
        Assert.NotEmpty(models);
    }

    [Fact]
    public void GetAvailableModels_WithSpecificProvider_ShouldReturnProviderModels()
    {
        // Arrange
        var manager = new AIProviderManager(_storageManager);
        var openAIProvider = new OpenAIProvider(_httpClient, _storageManager);
        manager.RegisterProvider(openAIProvider);

        // Act
        var models = manager.GetAvailableModels("OpenAI");

        // Assert
        Assert.NotEmpty(models);
        Assert.Contains("gpt-4", models);
        Assert.Contains("gpt-3.5-turbo", models);
    }

    [Fact]
    public void GetAvailableModels_WithInvalidProvider_ShouldReturnEmptyList()
    {
        // Arrange
        var manager = new AIProviderManager(_storageManager);

        // Act
        var models = manager.GetAvailableModels("NonExistent");

        // Assert
        Assert.Empty(models);
    }

    #endregion

    #region Connection Validation Tests

    [Fact]
    public async Task ValidateProviderConnectionAsync_WithInvalidProvider_ShouldReturnFalse()
    {
        // Arrange
        var manager = new AIProviderManager(_storageManager);

        // Act
        var isValid = await manager.ValidateProviderConnectionAsync("NonExistent");

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public async Task ValidateProviderConnectionAsync_WithUnconfiguredProvider_ShouldReturnFalse()
    {
        // Arrange
        var manager = new AIProviderManager(_storageManager);
        var provider = new GroqProvider(_httpClient, _storageManager);
        manager.RegisterProvider(provider);

        // Act
        var isValid = await manager.ValidateProviderConnectionAsync("Groq");

        // Assert
        Assert.False(isValid);
    }

    #endregion
}
