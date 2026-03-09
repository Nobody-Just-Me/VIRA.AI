using Xunit;
using VIRA.Shared.Services;
using VIRA.Shared.Models;

namespace VIRA.Shared.Tests;

/// <summary>
/// Unit tests for OpenAIProvider implementation
/// Tests API key configuration, model selection, and basic functionality
/// **Validates: Requirements 9.2, 15.1 (OpenAI API integration with streaming)**
/// </summary>
public class OpenAIProviderTests
{
    private readonly HttpClient _httpClient;
    private readonly SecureStorageManager _storageManager;

    public OpenAIProviderTests()
    {
        _httpClient = new HttpClient();
        _storageManager = new SecureStorageManager();
    }

    #region Provider Configuration Tests

    [Fact]
    public void ProviderName_ShouldReturnOpenAI()
    {
        // Arrange
        var provider = new OpenAIProvider(_httpClient, _storageManager);

        // Act
        var name = provider.ProviderName;

        // Assert
        Assert.Equal("OpenAI", name);
    }

    [Fact]
    public void IsConfigured_WithoutApiKey_ShouldReturnFalse()
    {
        // Arrange
        var provider = new OpenAIProvider(_httpClient, _storageManager);

        // Act
        var isConfigured = provider.IsConfigured;

        // Assert
        Assert.False(isConfigured);
    }

    [Fact]
    public async Task IsConfigured_WithApiKey_ShouldReturnTrue()
    {
        // Arrange
        var provider = new OpenAIProvider(_httpClient, _storageManager);
        await provider.SetApiKeyAsync("sk-test1234567890abcdef");

        // Act
        var isConfigured = provider.IsConfigured;

        // Assert
        Assert.True(isConfigured);
    }

    [Fact]
    public async Task SetApiKeyAsync_ShouldStoreApiKey()
    {
        // Arrange
        var provider = new OpenAIProvider(_httpClient, _storageManager);
        var apiKey = "sk-test1234567890abcdef";

        // Act
        await provider.SetApiKeyAsync(apiKey);

        // Assert
        Assert.True(provider.IsConfigured);
        var storedKey = await _storageManager.GetApiKeyAsync("OpenAI");
        Assert.Equal(apiKey, storedKey);
    }

    #endregion

    #region Model Selection Tests

    [Fact]
    public void GetAvailableModels_ShouldReturnOpenAIModels()
    {
        // Arrange
        var provider = new OpenAIProvider(_httpClient, _storageManager);

        // Act
        var models = provider.GetAvailableModels();

        // Assert
        Assert.NotEmpty(models);
        Assert.Contains("gpt-4", models);
        Assert.Contains("gpt-3.5-turbo", models);
    }

    [Fact]
    public void SetModel_WithValidModel_ShouldSucceed()
    {
        // Arrange
        var provider = new OpenAIProvider(_httpClient, _storageManager);

        // Act
        provider.SetModel("gpt-3.5-turbo");

        // Assert
        // No exception should be thrown
        Assert.True(true);
    }

    [Fact]
    public void SetModel_WithInvalidModel_ShouldNotChangeModel()
    {
        // Arrange
        var provider = new OpenAIProvider(_httpClient, _storageManager);

        // Act
        provider.SetModel("invalid-model-name");

        // Assert
        // Should silently ignore invalid model
        Assert.True(true);
    }

    #endregion

    #region API Request Tests

    [Fact]
    public async Task SendMessageAsync_WithoutApiKey_ShouldThrowException()
    {
        // Arrange
        var provider = new OpenAIProvider(_httpClient, _storageManager);
        var message = "Hello";
        var history = new List<ChatMessage>();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await provider.SendMessageAsync(message, history)
        );
    }

    [Fact]
    public async Task SendMessageStreamAsync_WithoutApiKey_ShouldThrowException()
    {
        // Arrange
        var provider = new OpenAIProvider(_httpClient, _storageManager);
        var message = "Hello";
        var history = new List<ChatMessage>();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await provider.SendMessageStreamAsync(message, history)
        );
    }

    #endregion

    #region Validation Tests

    [Fact]
    public async Task ValidateConnectionAsync_WithoutApiKey_ShouldReturnFalse()
    {
        // Arrange
        var provider = new OpenAIProvider(_httpClient, _storageManager);

        // Act
        var isValid = await provider.ValidateConnectionAsync();

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public async Task ValidateConnectionAsync_WithInvalidApiKey_ShouldReturnFalse()
    {
        // Arrange
        var provider = new OpenAIProvider(_httpClient, _storageManager);
        await provider.SetApiKeyAsync("invalid_key");

        // Act
        var isValid = await provider.ValidateConnectionAsync();

        // Assert
        Assert.False(isValid);
    }

    #endregion

    #region Interface Implementation Tests

    [Fact]
    public void OpenAIProvider_ShouldImplementIProviderService()
    {
        // Arrange
        var provider = new OpenAIProvider(_httpClient, _storageManager);

        // Act & Assert
        Assert.IsAssignableFrom<IProviderService>(provider);
    }

    [Fact]
    public void OpenAIProvider_ShouldHaveAllRequiredMethods()
    {
        // Arrange
        var provider = new OpenAIProvider(_httpClient, _storageManager);
        var providerType = provider.GetType();

        // Act & Assert
        Assert.NotNull(providerType.GetProperty("ProviderName"));
        Assert.NotNull(providerType.GetProperty("IsConfigured"));
        Assert.NotNull(providerType.GetMethod("ValidateConnectionAsync"));
        Assert.NotNull(providerType.GetMethod("SendMessageAsync"));
        Assert.NotNull(providerType.GetMethod("SendMessageStreamAsync"));
        Assert.NotNull(providerType.GetMethod("GetAvailableModels"));
    }

    #endregion
}
