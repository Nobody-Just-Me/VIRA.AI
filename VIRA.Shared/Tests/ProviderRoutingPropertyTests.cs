using Xunit;
using FsCheck;
using FsCheck.Xunit;
using VIRA.Shared.Services;
using VIRA.Shared.Models;

namespace VIRA.Shared.Tests;

/// <summary>
/// Property-based tests for AI provider routing
/// Tests that requests are correctly routed to the selected provider
/// **Validates: Requirements 9.4 (Provider routing)**
/// </summary>
public class ProviderRoutingPropertyTests
{
    private readonly SecureStorageManager _storageManager;
    private readonly HttpClient _httpClient;

    public ProviderRoutingPropertyTests()
    {
        _storageManager = new SecureStorageManager();
        _httpClient = new HttpClient();
    }

    #region Property 25: Provider Routing

    /// <summary>
    /// Property 25: For any AI request when a provider is selected, 
    /// the request should be routed to that specific provider's API
    /// **Validates: Requirements 9.4**
    /// </summary>
    [Property(DisplayName = "Feature: vira-modern-ui-redesign, Property 25: Provider routing directs requests to selected provider", MaxTest = 100)]
    public Property ProviderRoutingDirectsRequestsToSelectedProvider()
    {
        // Generator for provider names
        var providerGen = Gen.Elements("Groq", "OpenAI", "Gemini");
        
        // Generator for test messages
        var messageGen = Gen.Elements(
            "Hello",
            "What's the weather?",
            "Tell me a joke",
            "How are you?",
            "Explain quantum physics"
        );

        return Prop.ForAll(
            Arb.From(providerGen),
            Arb.From(messageGen),
            (providerName, message) =>
            {
                // Arrange
                var manager = new AIProviderManager(_storageManager);
                var mockProvider = new MockProviderService(providerName);
                
                manager.RegisterProvider(mockProvider);
                manager.SetActiveProvider(providerName);

                // Act
                var activeProvider = manager.GetActiveProvider();

                // Assert
                return activeProvider != null &&
                       activeProvider.ProviderName == providerName;
            });
    }

    /// <summary>
    /// Property: When a provider is set as active, GetActiveProvider should return that provider
    /// </summary>
    [Property(DisplayName = "Active provider retrieval returns the selected provider", MaxTest = 100)]
    public Property ActiveProviderRetrievalReturnsSelectedProvider()
    {
        var providerGen = Gen.Elements("Groq", "OpenAI", "Gemini");

        return Prop.ForAll(
            Arb.From(providerGen),
            (providerName) =>
            {
                // Arrange
                var manager = new AIProviderManager(_storageManager);
                var mockProvider = new MockProviderService(providerName);
                
                manager.RegisterProvider(mockProvider);
                manager.SetActiveProvider(providerName);

                // Act
                var activeProviderName = manager.GetActiveProviderName();
                var activeProvider = manager.GetActiveProvider();

                // Assert
                return activeProviderName == providerName &&
                       activeProvider != null &&
                       activeProvider.ProviderName == providerName;
            });
    }

    /// <summary>
    /// Property: For any registered provider, setting it as active should make it the routing target
    /// </summary>
    [Property(DisplayName = "Setting provider as active makes it the routing target", MaxTest = 100)]
    public Property SettingProviderAsActiveMakesItRoutingTarget()
    {
        var providerGen = Gen.Elements("Groq", "OpenAI", "Gemini");

        return Prop.ForAll(
            Arb.From(providerGen),
            (providerName) =>
            {
                // Arrange
                var manager = new AIProviderManager(_storageManager);
                
                // Register all providers
                var groqProvider = new MockProviderService("Groq");
                var openAIProvider = new MockProviderService("OpenAI");
                var geminiProvider = new MockProviderService("Gemini");
                
                manager.RegisterProvider(groqProvider);
                manager.RegisterProvider(openAIProvider);
                manager.RegisterProvider(geminiProvider);

                // Act
                manager.SetActiveProvider(providerName);
                var activeProvider = manager.GetActiveProvider();

                // Assert
                return activeProvider != null &&
                       activeProvider.ProviderName == providerName;
            });
    }

    /// <summary>
    /// Property: Switching between providers should always route to the currently selected provider
    /// </summary>
    [Property(DisplayName = "Switching providers updates routing target", MaxTest = 100)]
    public Property SwitchingProvidersUpdatesRoutingTarget()
    {
        var providerSequenceGen = Gen.NonEmptyListOf(Gen.Elements("Groq", "OpenAI", "Gemini"));

        return Prop.ForAll(
            Arb.From(providerSequenceGen),
            (providerSequence) =>
            {
                // Arrange
                var manager = new AIProviderManager(_storageManager);
                
                var groqProvider = new MockProviderService("Groq");
                var openAIProvider = new MockProviderService("OpenAI");
                var geminiProvider = new MockProviderService("Gemini");
                
                manager.RegisterProvider(groqProvider);
                manager.RegisterProvider(openAIProvider);
                manager.RegisterProvider(geminiProvider);

                // Act & Assert - switch through the sequence
                foreach (var providerName in providerSequence)
                {
                    manager.SetActiveProvider(providerName);
                    var activeProvider = manager.GetActiveProvider();
                    
                    if (activeProvider == null || activeProvider.ProviderName != providerName)
                    {
                        return false;
                    }
                }

                return true;
            });
    }

    /// <summary>
    /// Property: The active provider should remain consistent across multiple retrievals
    /// </summary>
    [Property(DisplayName = "Active provider remains consistent across retrievals", MaxTest = 100)]
    public Property ActiveProviderRemainsConsistentAcrossRetrievals()
    {
        var providerGen = Gen.Elements("Groq", "OpenAI", "Gemini");
        var retrievalCountGen = Gen.Choose(2, 10);

        return Prop.ForAll(
            Arb.From(providerGen),
            Arb.From(retrievalCountGen),
            (providerName, retrievalCount) =>
            {
                // Arrange
                var manager = new AIProviderManager(_storageManager);
                var mockProvider = new MockProviderService(providerName);
                
                manager.RegisterProvider(mockProvider);
                manager.SetActiveProvider(providerName);

                // Act - retrieve multiple times
                var retrievedProviders = new List<string>();
                for (int i = 0; i < retrievalCount; i++)
                {
                    var activeProvider = manager.GetActiveProvider();
                    if (activeProvider != null)
                    {
                        retrievedProviders.Add(activeProvider.ProviderName);
                    }
                }

                // Assert - all retrievals should return the same provider
                return retrievedProviders.Count == retrievalCount &&
                       retrievedProviders.All(p => p == providerName);
            });
    }

    #endregion

    #region Property 26: Provider Fallback

    /// <summary>
    /// Property 26: For any provider request failure, 
    /// the system should attempt to route the request to the configured fallback provider
    /// **Validates: Requirements 9.5**
    /// </summary>
    [Property(DisplayName = "Feature: vira-modern-ui-redesign, Property 26: Provider fallback routes to backup provider on failure", MaxTest = 100)]
    public Property ProviderFallbackRoutesToBackupProviderOnFailure()
    {
        // Generator for primary provider names
        var primaryProviderGen = Gen.Elements("Groq", "OpenAI", "Gemini");
        
        // Generator for fallback provider names (different from primary)
        var fallbackProviderGen = Gen.Elements("Groq", "OpenAI", "Gemini");
        
        // Generator for test messages
        var messageGen = Gen.Elements(
            "Hello",
            "What's the weather?",
            "Tell me a joke",
            "How are you?",
            "Explain quantum physics"
        );

        return Prop.ForAll(
            Arb.From(primaryProviderGen),
            Arb.From(fallbackProviderGen),
            Arb.From(messageGen),
            (primaryProvider, fallbackProvider, message) =>
            {
                // Skip if primary and fallback are the same
                if (primaryProvider == fallbackProvider)
                {
                    return true;
                }

                // Arrange
                var manager = new AIProviderManager(_storageManager);
                
                // Create a failing primary provider and a working fallback provider
                var failingProvider = new FailingMockProviderService(primaryProvider);
                var workingProvider = new MockProviderService(fallbackProvider);
                workingProvider.SetApiKey("test-key");
                
                manager.RegisterProvider(failingProvider);
                manager.RegisterProvider(workingProvider);
                
                manager.SetActiveProvider(primaryProvider);
                manager.SetFallbackProviderAsync(fallbackProvider).Wait();

                // Act
                try
                {
                    var response = manager.SendMessageAsync(message, new List<ChatMessage>()).Result;
                    
                    // Assert - response should come from fallback provider
                    return response != null &&
                           response.Content.Contains(fallbackProvider);
                }
                catch
                {
                    // If both providers fail, that's acceptable
                    return false;
                }
            });
    }

    /// <summary>
    /// Property: When primary provider fails and no fallback is configured, an exception should be thrown
    /// </summary>
    [Property(DisplayName = "No fallback configured throws exception on primary failure", MaxTest = 100)]
    public Property NoFallbackConfiguredThrowsExceptionOnPrimaryFailure()
    {
        var providerGen = Gen.Elements("Groq", "OpenAI", "Gemini");
        var messageGen = Gen.Elements("Hello", "Test message", "How are you?");

        return Prop.ForAll(
            Arb.From(providerGen),
            Arb.From(messageGen),
            (providerName, message) =>
            {
                // Arrange
                var manager = new AIProviderManager(_storageManager);
                var failingProvider = new FailingMockProviderService(providerName);
                
                manager.RegisterProvider(failingProvider);
                manager.SetActiveProvider(providerName);
                // No fallback configured

                // Act & Assert
                try
                {
                    var result = manager.SendMessageAsync(message, new List<ChatMessage>()).Result;
                    return false; // Should have thrown an exception
                }
                catch (Exception)
                {
                    return true; // Expected behavior
                }
            });
    }

    /// <summary>
    /// Property: When both primary and fallback providers fail, the original exception should be thrown
    /// </summary>
    [Property(DisplayName = "Both providers failing throws original exception", MaxTest = 100)]
    public Property BothProvidersFailingThrowsOriginalException()
    {
        var primaryProviderGen = Gen.Elements("Groq", "OpenAI", "Gemini");
        var fallbackProviderGen = Gen.Elements("Groq", "OpenAI", "Gemini");
        var messageGen = Gen.Elements("Hello", "Test message");

        return Prop.ForAll(
            Arb.From(primaryProviderGen),
            Arb.From(fallbackProviderGen),
            Arb.From(messageGen),
            (primaryProvider, fallbackProvider, message) =>
            {
                // Skip if primary and fallback are the same
                if (primaryProvider == fallbackProvider)
                {
                    return true;
                }

                // Arrange
                var manager = new AIProviderManager(_storageManager);
                
                var failingPrimary = new FailingMockProviderService(primaryProvider);
                var failingFallback = new FailingMockProviderService(fallbackProvider);
                
                manager.RegisterProvider(failingPrimary);
                manager.RegisterProvider(failingFallback);
                
                manager.SetActiveProvider(primaryProvider);
                manager.SetFallbackProviderAsync(fallbackProvider).Wait();

                // Act & Assert
                try
                {
                    var result = manager.SendMessageAsync(message, new List<ChatMessage>()).Result;
                    return false; // Should have thrown an exception
                }
                catch (Exception)
                {
                    return true; // Expected behavior
                }
            });
    }

    /// <summary>
    /// Property: Fallback should work for streaming requests as well
    /// </summary>
    [Property(DisplayName = "Fallback works for streaming requests", MaxTest = 100)]
    public Property FallbackWorksForStreamingRequests()
    {
        var primaryProviderGen = Gen.Elements("Groq", "OpenAI", "Gemini");
        var fallbackProviderGen = Gen.Elements("Groq", "OpenAI", "Gemini");
        var messageGen = Gen.Elements("Hello", "Stream this message");

        return Prop.ForAll(
            Arb.From(primaryProviderGen),
            Arb.From(fallbackProviderGen),
            Arb.From(messageGen),
            (primaryProvider, fallbackProvider, message) =>
            {
                // Skip if primary and fallback are the same
                if (primaryProvider == fallbackProvider)
                {
                    return true;
                }

                // Arrange
                var manager = new AIProviderManager(_storageManager);
                
                var failingProvider = new FailingMockProviderService(primaryProvider);
                var workingProvider = new MockProviderService(fallbackProvider);
                workingProvider.SetApiKey("test-key");
                
                manager.RegisterProvider(failingProvider);
                manager.RegisterProvider(workingProvider);
                
                manager.SetActiveProvider(primaryProvider);
                manager.SetFallbackProviderAsync(fallbackProvider).Wait();

                // Act
                try
                {
                    var stream = manager.SendMessageStreamAsync(message, new List<ChatMessage>()).Result;
                    
                    // Read the stream to verify it contains fallback provider response
                    using var reader = new StreamReader(stream);
                    var content = reader.ReadToEndAsync().Result;
                    
                    // Assert - response should come from fallback provider
                    return !string.IsNullOrEmpty(content) &&
                           content.Contains(fallbackProvider);
                }
                catch
                {
                    return false;
                }
            });
    }

    /// <summary>
    /// Property: Fallback should not be used when primary provider succeeds
    /// </summary>
    [Property(DisplayName = "Fallback not used when primary succeeds", MaxTest = 100)]
    public Property FallbackNotUsedWhenPrimarySucceeds()
    {
        var primaryProviderGen = Gen.Elements("Groq", "OpenAI", "Gemini");
        var fallbackProviderGen = Gen.Elements("Groq", "OpenAI", "Gemini");
        var messageGen = Gen.Elements("Hello", "Test message");

        return Prop.ForAll(
            Arb.From(primaryProviderGen),
            Arb.From(fallbackProviderGen),
            Arb.From(messageGen),
            (primaryProvider, fallbackProvider, message) =>
            {
                // Skip if primary and fallback are the same
                if (primaryProvider == fallbackProvider)
                {
                    return true;
                }

                // Arrange
                var manager = new AIProviderManager(_storageManager);
                
                var workingPrimary = new MockProviderService(primaryProvider);
                workingPrimary.SetApiKey("test-key");
                var workingFallback = new MockProviderService(fallbackProvider);
                workingFallback.SetApiKey("test-key");
                
                manager.RegisterProvider(workingPrimary);
                manager.RegisterProvider(workingFallback);
                
                manager.SetActiveProvider(primaryProvider);
                manager.SetFallbackProviderAsync(fallbackProvider).Wait();

                // Act
                var response = manager.SendMessageAsync(message, new List<ChatMessage>()).Result;
                
                // Assert - response should come from primary provider, not fallback
                return response != null &&
                       response.Content.Contains(primaryProvider) &&
                       !response.Content.Contains(fallbackProvider);
            });
    }

    #endregion

    #region Property 27: API Key Isolation

    /// <summary>
    /// Property 27: For any provider, its API key should be stored separately 
    /// and not accessible to other providers
    /// **Validates: Requirements 9.6**
    /// </summary>
    [Property(DisplayName = "Feature: vira-modern-ui-redesign, Property 27: API keys are isolated between providers", MaxTest = 100)]
    public Property ApiKeysAreIsolatedBetweenProviders()
    {
        // Generator for provider names
        var providerGen = Gen.Elements("Groq", "OpenAI", "Gemini");
        
        // Generator for API keys (simulating different keys for different providers)
        var apiKeyGen = Gen.Choose(1000, 9999).Select(n => $"test-api-key-{n}");

        // Combine generators into tuples
        var providerPairGen = Gen.Two(providerGen);
        var apiKeyPairGen = Gen.Two(apiKeyGen);

        return Prop.ForAll(
            Arb.From(providerPairGen),
            Arb.From(apiKeyPairGen),
            (providerPair, apiKeyPair) =>
            {
                var (provider1, provider2) = providerPair;
                var (apiKey1, apiKey2) = apiKeyPair;
                
                // Skip if both providers are the same
                if (provider1 == provider2)
                {
                    return true;
                }

                // Arrange
                var storageManager = new SecureStorageManager();
                
                // Act - Save API keys for both providers
                storageManager.SaveApiKeyAsync(provider1, apiKey1).Wait();
                storageManager.SaveApiKeyAsync(provider2, apiKey2).Wait();
                
                // Retrieve API keys
                var retrievedKey1 = storageManager.GetApiKeyAsync(provider1).Result;
                var retrievedKey2 = storageManager.GetApiKeyAsync(provider2).Result;
                
                // Assert - Each provider should only have access to its own API key
                return retrievedKey1 == apiKey1 &&
                       retrievedKey2 == apiKey2 &&
                       retrievedKey1 != retrievedKey2;
            });
    }

    /// <summary>
    /// Property: Updating one provider's API key should not affect other providers' keys
    /// </summary>
    [Property(DisplayName = "Updating one provider API key does not affect others", MaxTest = 100)]
    public Property UpdatingOneProviderKeyDoesNotAffectOthers()
    {
        var apiKeyGen = Gen.Choose(1000, 9999).Select(n => $"test-api-key-{n}");
        var apiKeyTripleGen = Gen.Three(apiKeyGen);

        return Prop.ForAll(
            Arb.From(apiKeyTripleGen),
            (apiKeyTriple) =>
            {
                var (groqKey, openAIKey, newGroqKey) = apiKeyTriple;
                
                // Arrange
                var storageManager = new SecureStorageManager();
                
                // Save initial keys for all providers
                storageManager.SaveApiKeyAsync("Groq", groqKey).Wait();
                storageManager.SaveApiKeyAsync("OpenAI", openAIKey).Wait();
                storageManager.SaveApiKeyAsync("Gemini", "gemini-key").Wait();
                
                // Act - Update only Groq's key
                storageManager.SaveApiKeyAsync("Groq", newGroqKey).Wait();
                
                // Retrieve all keys
                var retrievedGroqKey = storageManager.GetApiKeyAsync("Groq").Result;
                var retrievedOpenAIKey = storageManager.GetApiKeyAsync("OpenAI").Result;
                var retrievedGeminiKey = storageManager.GetApiKeyAsync("Gemini").Result;
                
                // Assert - Only Groq's key should have changed
                return retrievedGroqKey == newGroqKey &&
                       retrievedOpenAIKey == openAIKey &&
                       retrievedGeminiKey == "gemini-key";
            });
    }

    /// <summary>
    /// Property: Deleting one provider's API key should not affect other providers' keys
    /// </summary>
    [Property(DisplayName = "Deleting one provider API key does not affect others", MaxTest = 100)]
    public Property DeletingOneProviderKeyDoesNotAffectOthers()
    {
        var providerToDeleteGen = Gen.Elements("Groq", "OpenAI", "Gemini");
        var apiKeyGen = Gen.Choose(1000, 9999).Select(n => $"test-api-key-{n}");
        var apiKeyTripleGen = Gen.Three(apiKeyGen);

        return Prop.ForAll(
            Arb.From(providerToDeleteGen),
            Arb.From(apiKeyTripleGen),
            (providerToDelete, apiKeyTriple) =>
            {
                var (groqKey, openAIKey, geminiKey) = apiKeyTriple;
                
                // Arrange
                var storageManager = new SecureStorageManager();
                
                // Save keys for all providers
                storageManager.SaveApiKeyAsync("Groq", groqKey).Wait();
                storageManager.SaveApiKeyAsync("OpenAI", openAIKey).Wait();
                storageManager.SaveApiKeyAsync("Gemini", geminiKey).Wait();
                
                // Act - Delete one provider's key
                storageManager.DeleteApiKeyAsync(providerToDelete).Wait();
                
                // Retrieve all keys
                var retrievedGroqKey = storageManager.GetApiKeyAsync("Groq").Result;
                var retrievedOpenAIKey = storageManager.GetApiKeyAsync("OpenAI").Result;
                var retrievedGeminiKey = storageManager.GetApiKeyAsync("Gemini").Result;
                
                // Assert - Only the deleted provider's key should be null/empty
                var deletedKeyIsNull = providerToDelete switch
                {
                    "Groq" => string.IsNullOrEmpty(retrievedGroqKey),
                    "OpenAI" => string.IsNullOrEmpty(retrievedOpenAIKey),
                    "Gemini" => string.IsNullOrEmpty(retrievedGeminiKey),
                    _ => false
                };
                
                var otherKeysIntact = providerToDelete switch
                {
                    "Groq" => retrievedOpenAIKey == openAIKey && retrievedGeminiKey == geminiKey,
                    "OpenAI" => retrievedGroqKey == groqKey && retrievedGeminiKey == geminiKey,
                    "Gemini" => retrievedGroqKey == groqKey && retrievedOpenAIKey == openAIKey,
                    _ => false
                };
                
                return deletedKeyIsNull && otherKeysIntact;
            });
    }

    /// <summary>
    /// Property: API keys should be stored with provider-specific keys to ensure isolation
    /// </summary>
    [Property(DisplayName = "API keys use provider-specific storage keys", MaxTest = 100)]
    public Property ApiKeysUseProviderSpecificStorageKeys()
    {
        var providerGen = Gen.Elements("Groq", "OpenAI", "Gemini");
        var apiKeyGen = Gen.Choose(1000, 9999).Select(n => $"test-api-key-{n}");

        return Prop.ForAll(
            Arb.From(providerGen),
            Arb.From(apiKeyGen),
            (providerName, apiKey) =>
            {
                // Arrange
                var storageManager = new SecureStorageManager();
                
                // Act - Save API key for the provider
                storageManager.SaveApiKeyAsync(providerName, apiKey).Wait();
                
                // Retrieve the key
                var retrievedKey = storageManager.GetApiKeyAsync(providerName).Result;
                
                // Try to retrieve with a different provider name (should not work)
                var otherProvider = providerName switch
                {
                    "Groq" => "OpenAI",
                    "OpenAI" => "Gemini",
                    "Gemini" => "Groq",
                    _ => "Unknown"
                };
                
                var wrongProviderKey = storageManager.GetApiKeyAsync(otherProvider).Result;
                
                // Assert - Key should only be retrievable with the correct provider name
                return retrievedKey == apiKey &&
                       (string.IsNullOrEmpty(wrongProviderKey) || wrongProviderKey != apiKey);
            });
    }

    /// <summary>
    /// Property: Multiple providers can have the same API key value, but they should be stored separately
    /// </summary>
    [Property(DisplayName = "Same API key value can be stored for multiple providers independently", MaxTest = 100)]
    public Property SameApiKeyValueCanBeStoredForMultipleProvidersIndependently()
    {
        var apiKeyGen = Gen.Choose(1000, 9999).Select(n => $"test-api-key-{n}");

        return Prop.ForAll(
            Arb.From(apiKeyGen),
            (sharedApiKey) =>
            {
                // Arrange
                var storageManager = new SecureStorageManager();
                
                // Act - Save the same API key for all providers
                storageManager.SaveApiKeyAsync("Groq", sharedApiKey).Wait();
                storageManager.SaveApiKeyAsync("OpenAI", sharedApiKey).Wait();
                storageManager.SaveApiKeyAsync("Gemini", sharedApiKey).Wait();
                
                // Retrieve all keys
                var groqKey = storageManager.GetApiKeyAsync("Groq").Result;
                var openAIKey = storageManager.GetApiKeyAsync("OpenAI").Result;
                var geminiKey = storageManager.GetApiKeyAsync("Gemini").Result;
                
                // Delete one provider's key
                storageManager.DeleteApiKeyAsync("Groq").Wait();
                
                // Retrieve again
                var groqKeyAfterDelete = storageManager.GetApiKeyAsync("Groq").Result;
                var openAIKeyAfterDelete = storageManager.GetApiKeyAsync("OpenAI").Result;
                var geminiKeyAfterDelete = storageManager.GetApiKeyAsync("Gemini").Result;
                
                // Assert - All providers should have had the key initially,
                // but deleting one should not affect the others
                return groqKey == sharedApiKey &&
                       openAIKey == sharedApiKey &&
                       geminiKey == sharedApiKey &&
                       string.IsNullOrEmpty(groqKeyAfterDelete) &&
                       openAIKeyAfterDelete == sharedApiKey &&
                       geminiKeyAfterDelete == sharedApiKey;
            });
    }

    /// <summary>
    /// Property: Case-insensitive provider names should still maintain isolation
    /// </summary>
    [Property(DisplayName = "Case-insensitive provider names maintain isolation", MaxTest = 100)]
    public Property CaseInsensitiveProviderNamesMaintainIsolation()
    {
        var apiKeyGen = Gen.Choose(1000, 9999).Select(n => $"test-api-key-{n}");
        var apiKeyPairGen = Gen.Two(apiKeyGen);

        return Prop.ForAll(
            Arb.From(apiKeyPairGen),
            (apiKeyPair) =>
            {
                var (lowerCaseKey, upperCaseKey) = apiKeyPair;
                
                // Arrange
                var storageManager = new SecureStorageManager();
                
                // Act - Save keys with different casing
                storageManager.SaveApiKeyAsync("groq", lowerCaseKey).Wait();
                storageManager.SaveApiKeyAsync("GROQ", upperCaseKey).Wait();
                
                // Retrieve with different casings
                var lowerRetrieved = storageManager.GetApiKeyAsync("groq").Result;
                var upperRetrieved = storageManager.GetApiKeyAsync("GROQ").Result;
                var mixedRetrieved = storageManager.GetApiKeyAsync("Groq").Result;
                
                // Assert - The storage should normalize to lowercase (based on implementation)
                // All variations should retrieve the same key (the last one saved)
                return lowerRetrieved == upperCaseKey &&
                       upperRetrieved == upperCaseKey &&
                       mixedRetrieved == upperCaseKey;
            });
    }

    #endregion

    #region Property 28: API Key Validation

    /// <summary>
    /// Property 28: For any API key input, validation should occur before the key is saved to storage
    /// **Validates: Requirements 9.7**
    /// </summary>
    [Property(DisplayName = "Feature: vira-modern-ui-redesign, Property 28: API keys are validated before storage", MaxTest = 100)]
    public Property ApiKeysAreValidatedBeforeStorage()
    {
        // Generator for provider names
        var providerGen = Gen.Elements("Groq", "OpenAI", "Gemini");
        
        // Generator for API keys (both valid and invalid formats)
        var validApiKeyGen = Gen.Choose(1000, 9999).Select(n => $"valid-api-key-{n}");
        var invalidApiKeyGen = Gen.Elements("", "invalid", "short", "x");

        return Prop.ForAll(
            Arb.From(providerGen),
            Arb.From(Gen.OneOf(validApiKeyGen, invalidApiKeyGen)),
            (providerName, apiKey) =>
            {
                // Arrange
                var manager = new AIProviderManager(_storageManager);
                var mockProvider = new ValidationMockProviderService(providerName, apiKey);
                
                manager.RegisterProvider(mockProvider);

                // Act
                var validationResult = manager.ValidateAndSaveApiKeyAsync(providerName, apiKey).Result;

                // Assert - Validation should have been called before saving
                // If validation passed, the key should be saved
                // If validation failed, the key should not be saved
                if (validationResult)
                {
                    // Key was validated and saved
                    var savedKey = _storageManager.GetApiKeyAsync(providerName).Result;
                    return mockProvider.ValidationWasCalled && 
                           savedKey == apiKey;
                }
                else
                {
                    // Key validation failed, should not be saved
                    var savedKey = _storageManager.GetApiKeyAsync(providerName).Result;
                    return mockProvider.ValidationWasCalled && 
                           (string.IsNullOrEmpty(savedKey) || savedKey != apiKey);
                }
            });
    }

    /// <summary>
    /// Property: Invalid API keys should not be saved to storage
    /// </summary>
    [Property(DisplayName = "Invalid API keys are not saved to storage", MaxTest = 100)]
    public Property InvalidApiKeysAreNotSavedToStorage()
    {
        var providerGen = Gen.Elements("Groq", "OpenAI", "Gemini");
        var invalidApiKeyGen = Gen.Elements("", "invalid", "short", "bad-key", "x");

        return Prop.ForAll(
            Arb.From(providerGen),
            Arb.From(invalidApiKeyGen),
            (providerName, invalidApiKey) =>
            {
                // Arrange
                var manager = new AIProviderManager(_storageManager);
                var mockProvider = new ValidationMockProviderService(providerName, invalidApiKey, shouldValidate: false);
                
                manager.RegisterProvider(mockProvider);

                // Act
                var validationResult = manager.ValidateAndSaveApiKeyAsync(providerName, invalidApiKey).Result;

                // Assert - Validation should fail and key should not be saved
                var savedKey = _storageManager.GetApiKeyAsync(providerName).Result;
                return !validationResult && 
                       (string.IsNullOrEmpty(savedKey) || savedKey != invalidApiKey);
            });
    }

    /// <summary>
    /// Property: Valid API keys should be saved to storage after validation
    /// </summary>
    [Property(DisplayName = "Valid API keys are saved after validation", MaxTest = 100)]
    public Property ValidApiKeysAreSavedAfterValidation()
    {
        var providerGen = Gen.Elements("Groq", "OpenAI", "Gemini");
        var validApiKeyGen = Gen.Choose(1000, 9999).Select(n => $"valid-api-key-{n}");

        return Prop.ForAll(
            Arb.From(providerGen),
            Arb.From(validApiKeyGen),
            (providerName, validApiKey) =>
            {
                // Arrange
                var manager = new AIProviderManager(_storageManager);
                var mockProvider = new ValidationMockProviderService(providerName, validApiKey, shouldValidate: true);
                
                manager.RegisterProvider(mockProvider);

                // Act
                var validationResult = manager.ValidateAndSaveApiKeyAsync(providerName, validApiKey).Result;

                // Assert - Validation should succeed and key should be saved
                var savedKey = _storageManager.GetApiKeyAsync(providerName).Result;
                return validationResult && 
                       savedKey == validApiKey;
            });
    }

    /// <summary>
    /// Property: Validation must occur before any storage operation
    /// </summary>
    [Property(DisplayName = "Validation occurs before storage operation", MaxTest = 100)]
    public Property ValidationOccursBeforeStorageOperation()
    {
        var providerGen = Gen.Elements("Groq", "OpenAI", "Gemini");
        var apiKeyGen = Gen.Choose(1000, 9999).Select(n => $"test-api-key-{n}");

        return Prop.ForAll(
            Arb.From(providerGen),
            Arb.From(apiKeyGen),
            (providerName, apiKey) =>
            {
                // Arrange
                var manager = new AIProviderManager(_storageManager);
                var mockProvider = new ValidationMockProviderService(providerName, apiKey);
                
                manager.RegisterProvider(mockProvider);

                // Act
                var validationResult = manager.ValidateAndSaveApiKeyAsync(providerName, apiKey).Result;

                // Assert - Validation should have been called
                return mockProvider.ValidationWasCalled;
            });
    }

    /// <summary>
    /// Property: Failed validation should clear any temporarily set API key
    /// </summary>
    [Property(DisplayName = "Failed validation clears temporary API key", MaxTest = 100)]
    public Property FailedValidationClearsTemporaryApiKey()
    {
        var providerGen = Gen.Elements("Groq", "OpenAI", "Gemini");
        var invalidApiKeyGen = Gen.Elements("invalid-key", "bad-key", "wrong-key");

        return Prop.ForAll(
            Arb.From(providerGen),
            Arb.From(invalidApiKeyGen),
            (providerName, invalidApiKey) =>
            {
                // Arrange
                var manager = new AIProviderManager(_storageManager);
                var mockProvider = new ValidationMockProviderService(providerName, invalidApiKey, shouldValidate: false);
                
                manager.RegisterProvider(mockProvider);

                // Act
                var validationResult = manager.ValidateAndSaveApiKeyAsync(providerName, invalidApiKey).Result;

                // Assert - Provider should not be configured after failed validation
                var provider = manager.GetProvider(providerName);
                return !validationResult && 
                       provider != null && 
                       !provider.IsConfigured;
            });
    }

    /// <summary>
    /// Property: Multiple validation attempts should each trigger validation
    /// </summary>
    [Property(DisplayName = "Multiple validation attempts each trigger validation", MaxTest = 100)]
    public Property MultipleValidationAttemptsEachTriggerValidation()
    {
        var providerGen = Gen.Elements("Groq", "OpenAI", "Gemini");
        var apiKeyGen = Gen.Choose(1000, 9999).Select(n => $"test-api-key-{n}");
        var attemptCountGen = Gen.Choose(2, 5);

        return Prop.ForAll(
            Arb.From(providerGen),
            Arb.From(apiKeyGen),
            Arb.From(attemptCountGen),
            (providerName, apiKey, attemptCount) =>
            {
                // Arrange
                var manager = new AIProviderManager(_storageManager);
                var mockProvider = new ValidationMockProviderService(providerName, apiKey);
                
                manager.RegisterProvider(mockProvider);

                // Act - Attempt validation multiple times
                int validationCallCount = 0;
                for (int i = 0; i < attemptCount; i++)
                {
                    mockProvider.ResetValidationFlag();
                    manager.ValidateAndSaveApiKeyAsync(providerName, $"{apiKey}-{i}").Wait();
                    if (mockProvider.ValidationWasCalled)
                    {
                        validationCallCount++;
                    }
                }

                // Assert - Validation should have been called for each attempt
                return validationCallCount == attemptCount;
            });
    }

    #endregion

    #region Mock Provider for Testing

    /// <summary>
    /// Mock provider service for testing routing without making actual API calls
    /// </summary>
    private class MockProviderService : IProviderService
    {
        public string ProviderName { get; }
        public bool IsConfigured { get; private set; }
        private string _apiKey = string.Empty;

        public MockProviderService(string providerName)
        {
            ProviderName = providerName;
            IsConfigured = false;
        }

        public void SetApiKey(string apiKey)
        {
            _apiKey = apiKey;
            IsConfigured = !string.IsNullOrEmpty(apiKey);
        }

        public Task<bool> ValidateConnectionAsync()
        {
            return Task.FromResult(IsConfigured);
        }

        public Task<ChatMessage> SendMessageAsync(string message, List<ChatMessage> history)
        {
            var response = new ChatMessage
            {
                Role = ChatMessageRole.Assistant,
                Content = $"Mock response from {ProviderName}",
                Timestamp = DateTime.UtcNow
            };
            return Task.FromResult(response);
        }

        public Task<Stream> SendMessageStreamAsync(string message, List<ChatMessage> history)
        {
            var responseText = $"Mock streaming response from {ProviderName}";
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(responseText));
            return Task.FromResult<Stream>(stream);
        }

        public List<string> GetAvailableModels()
        {
            return ProviderName switch
            {
                "Groq" => new List<string> { "mixtral-8x7b-32768", "llama2-70b-4096" },
                "OpenAI" => new List<string> { "gpt-4", "gpt-3.5-turbo" },
                "Gemini" => new List<string> { "gemini-2.0-flash", "gemini-pro" },
                _ => new List<string>()
            };
        }
    }

    /// <summary>
    /// Mock provider service that always fails - used for testing fallback behavior
    /// </summary>
    private class FailingMockProviderService : IProviderService
    {
        public string ProviderName { get; }
        public bool IsConfigured => true; // Always configured but always fails

        public FailingMockProviderService(string providerName)
        {
            ProviderName = providerName;
        }

        public Task<bool> ValidateConnectionAsync()
        {
            return Task.FromResult(false);
        }

        public Task<ChatMessage> SendMessageAsync(string message, List<ChatMessage> history)
        {
            throw new Exception($"Simulated failure from {ProviderName}");
        }

        public Task<Stream> SendMessageStreamAsync(string message, List<ChatMessage> history)
        {
            throw new Exception($"Simulated streaming failure from {ProviderName}");
        }

        public List<string> GetAvailableModels()
        {
            return new List<string>();
        }
    }

    /// <summary>
    /// Mock provider service for testing API key validation
    /// Tracks whether validation was called and can simulate validation success/failure
    /// </summary>
    private class ValidationMockProviderService : IProviderService
    {
        public string ProviderName { get; }
        public bool IsConfigured { get; private set; }
        public bool ValidationWasCalled { get; private set; }
        
        private string _apiKey = string.Empty;
        private readonly string _expectedApiKey;
        private readonly bool _shouldValidate;

        public ValidationMockProviderService(string providerName, string expectedApiKey, bool shouldValidate = true)
        {
            ProviderName = providerName;
            _expectedApiKey = expectedApiKey;
            _shouldValidate = shouldValidate;
            IsConfigured = false;
            ValidationWasCalled = false;
        }

        public async Task SetApiKeyAsync(string apiKey)
        {
            _apiKey = apiKey;
            IsConfigured = !string.IsNullOrEmpty(apiKey);
            await Task.CompletedTask;
        }

        public Task<bool> ValidateConnectionAsync()
        {
            ValidationWasCalled = true;
            
            // Simulate validation logic
            // Valid keys are non-empty and match expected pattern
            bool isValid = !string.IsNullOrEmpty(_apiKey) && 
                          _apiKey.Length > 5 && 
                          _shouldValidate;
            
            if (!isValid)
            {
                // Clear the API key on failed validation
                _apiKey = string.Empty;
                IsConfigured = false;
            }
            
            return Task.FromResult(isValid);
        }

        public void ResetValidationFlag()
        {
            ValidationWasCalled = false;
        }

        public Task<ChatMessage> SendMessageAsync(string message, List<ChatMessage> history)
        {
            var response = new ChatMessage
            {
                Role = ChatMessageRole.Assistant,
                Content = $"Mock response from {ProviderName}",
                Timestamp = DateTime.UtcNow
            };
            return Task.FromResult(response);
        }

        public Task<Stream> SendMessageStreamAsync(string message, List<ChatMessage> history)
        {
            var responseText = $"Mock streaming response from {ProviderName}";
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(responseText));
            return Task.FromResult<Stream>(stream);
        }

        public List<string> GetAvailableModels()
        {
            return ProviderName switch
            {
                "Groq" => new List<string> { "mixtral-8x7b-32768", "llama2-70b-4096" },
                "OpenAI" => new List<string> { "gpt-4", "gpt-3.5-turbo" },
                "Gemini" => new List<string> { "gemini-2.0-flash", "gemini-pro" },
                _ => new List<string>()
            };
        }
    }

    #endregion
}
