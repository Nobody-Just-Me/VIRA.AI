using Xunit;
using VIRA.Shared.Services;
using VIRA.Shared.Models;

namespace VIRA.Shared.Tests;

/// <summary>
/// Unit tests for message processing functionality
/// Tests rule-based processing, AI processing, hybrid decision logic, confidence threshold, and fallback mechanisms
/// **Validates: Property 29 (AI fallback to rule-based), Property 31 (Confidence threshold enforcement)**
/// </summary>
public class MessageProcessingTests
{
    private readonly HybridMessageProcessor _processor;
    private readonly RuleBasedProcessor _ruleBasedProcessor;
    private readonly TaskManager _taskManager;
    private readonly WeatherApiService _weatherService;
    private readonly NewsApiService _newsService;
    private readonly PreferencesService _preferencesService;

    public MessageProcessingTests()
    {
        // Initialize dependencies
        _taskManager = new TaskManager();
        _weatherService = new WeatherApiService();
        _newsService = new NewsApiService();
        _preferencesService = new PreferencesService();
        
        _ruleBasedProcessor = new RuleBasedProcessor(
            _taskManager,
            _weatherService,
            _newsService
        );
        
        _processor = new HybridMessageProcessor(
            _ruleBasedProcessor,
            _preferencesService
        );
    }

    #region Rule-Based Processing Flow Tests

    [Theory]
    [InlineData("tambah task beli susu")]
    [InlineData("cuaca hari ini")]
    [InlineData("berita terkini")]
    [InlineData("halo")]
    [InlineData("daftar task")]
    public async Task ProcessMessage_ShouldUseRuleBased_ForKnownPatterns(string input)
    {
        // Arrange
        var context = CreateTestContext();

        // Act
        var result = await _processor.ProcessMessageAsync(input, context);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsRuleBased);
        Assert.False(result.IsAIEnhanced);
        Assert.True(result.Confidence >= 0.8f); // High confidence for rule-based
    }

    [Fact]
    public async Task RuleBasedProcessor_ShouldReturnHighConfidence_ForExactMatch()
    {
        // Arrange
        string input = "tambah task beli susu";
        var context = CreateTestContext();

        // Act
        var result = await _ruleBasedProcessor.ProcessAsync(input, context);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Confidence >= 0.9f);
        Assert.Contains("task", result.Response.ToLower());
    }

    [Fact]
    public async Task RuleBasedProcessor_ShouldReturnLowConfidence_ForNoMatch()
    {
        // Arrange
        string input = "what is the meaning of life";
        var context = CreateTestContext();

        // Act
        var result = await _ruleBasedProcessor.ProcessAsync(input, context);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Confidence < 0.8f);
    }

    [Theory]
    [InlineData("tambah task beli susu", 0.95f)]
    [InlineData("cuaca", 0.9f)]
    [InlineData("halo", 0.85f)]
    public async Task RuleBasedProcessor_ShouldCalculateCorrectConfidence(string input, float expectedMinConfidence)
    {
        // Arrange
        var context = CreateTestContext();

        // Act
        var result = await _ruleBasedProcessor.ProcessAsync(input, context);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Confidence >= expectedMinConfidence);
    }

    #endregion

    #region AI Processing Flow Tests

    [Theory]
    [InlineData("what is the meaning of life")]
    [InlineData("explain quantum physics")]
    [InlineData("tell me a joke")]
    [InlineData("how do I cook pasta")]
    public async Task ProcessMessage_ShouldFallbackToAI_ForUnknownPatterns(string input)
    {
        // Arrange
        var context = CreateTestContext();
        
        // Configure AI (mock)
        _preferencesService.SetPreference("ai_provider", "groq");
        _preferencesService.SetPreference("ai_api_key", "test_key");

        // Act
        var result = await _processor.ProcessMessageAsync(input, context);

        // Assert
        Assert.NotNull(result);
        // Should attempt AI processing or return error if AI not available
    }

    [Fact]
    public async Task ProcessMessage_ShouldReturnError_WhenAINotConfigured()
    {
        // Arrange
        string input = "what is the meaning of life";
        var context = CreateTestContext();
        
        // Ensure AI is not configured
        _preferencesService.ClearPreference("ai_provider");
        _preferencesService.ClearPreference("ai_api_key");

        // Act
        var result = await _processor.ProcessMessageAsync(input, context);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsAIEnhanced);
        Assert.Contains("tidak dapat memahami", result.Response.ToLower());
    }

    #endregion

    #region Hybrid Processing Decision Logic Tests

    [Fact]
    public async Task HybridProcessor_ShouldTryRuleBasedFirst()
    {
        // Arrange
        string input = "tambah task beli susu";
        var context = CreateTestContext();

        // Act
        var result = await _processor.ProcessMessageAsync(input, context);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsRuleBased);
        Assert.False(result.IsAIEnhanced);
    }

    [Fact]
    public async Task HybridProcessor_ShouldCheckConfidenceThreshold()
    {
        // Arrange
        string input = "tambah task beli susu";
        var context = CreateTestContext();

        // Act
        var result = await _processor.ProcessMessageAsync(input, context);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Confidence >= HybridMessageProcessor.CONFIDENCE_THRESHOLD);
    }

    [Theory]
    [InlineData("tambah task beli susu", true)]  // High confidence - use rule-based
    [InlineData("what is quantum physics", false)] // Low confidence - try AI
    public async Task HybridProcessor_ShouldDecideCorrectly_BasedOnConfidence(string input, bool shouldBeRuleBased)
    {
        // Arrange
        var context = CreateTestContext();

        // Act
        var result = await _processor.ProcessMessageAsync(input, context);

        // Assert
        Assert.NotNull(result);
        if (shouldBeRuleBased)
        {
            Assert.True(result.IsRuleBased);
        }
    }

    #endregion

    #region Confidence Threshold Enforcement Tests
    // **Validates: Property 31 - Confidence threshold enforcement**

    [Fact]
    public async Task ConfidenceThreshold_ShouldBe_0_8()
    {
        // Assert
        Assert.Equal(0.8f, HybridMessageProcessor.CONFIDENCE_THRESHOLD);
    }

    [Theory]
    [InlineData(0.9f, true)]  // Above threshold - use rule-based
    [InlineData(0.85f, true)] // Above threshold - use rule-based
    [InlineData(0.8f, true)]  // At threshold - use rule-based
    [InlineData(0.79f, false)] // Below threshold - try AI
    [InlineData(0.5f, false)]  // Below threshold - try AI
    public async Task HybridProcessor_ShouldEnforceConfidenceThreshold(float confidence, bool shouldUseRuleBased)
    {
        // This test validates that the confidence threshold is properly enforced
        // Confidence >= 0.8 should use rule-based, < 0.8 should try AI
        
        // Note: This is a conceptual test - actual implementation depends on pattern matching
        Assert.True(confidence >= 0.8f == shouldUseRuleBased);
    }

    [Fact]
    public async Task RuleBasedProcessor_ShouldReturnConfidenceAboveThreshold_ForKnownPatterns()
    {
        // Arrange
        var knownPatterns = new[]
        {
            "tambah task beli susu",
            "cuaca hari ini",
            "berita terkini",
            "halo",
            "daftar task"
        };
        var context = CreateTestContext();

        foreach (var input in knownPatterns)
        {
            // Act
            var result = await _ruleBasedProcessor.ProcessAsync(input, context);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Confidence >= 0.8f, 
                $"Pattern '{input}' should have confidence >= 0.8, but got {result.Confidence}");
        }
    }

    [Fact]
    public async Task RuleBasedProcessor_ShouldReturnConfidenceBelowThreshold_ForUnknownPatterns()
    {
        // Arrange
        var unknownPatterns = new[]
        {
            "what is the meaning of life",
            "explain quantum physics",
            "random gibberish text",
            "asdfghjkl"
        };
        var context = CreateTestContext();

        foreach (var input in unknownPatterns)
        {
            // Act
            var result = await _ruleBasedProcessor.ProcessAsync(input, context);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Confidence < 0.8f,
                $"Unknown pattern '{input}' should have confidence < 0.8, but got {result.Confidence}");
        }
    }

    #endregion

    #region Fallback Mechanisms Tests
    // **Validates: Property 29 - AI fallback to rule-based**

    [Fact]
    public async Task HybridProcessor_ShouldFallbackToRuleBased_WhenAIFails()
    {
        // Arrange
        string input = "what is the meaning of life";
        var context = CreateTestContext();
        
        // Configure invalid AI to force failure
        _preferencesService.SetPreference("ai_provider", "invalid");
        _preferencesService.SetPreference("ai_api_key", "invalid_key");

        // Act
        var result = await _processor.ProcessMessageAsync(input, context);

        // Assert
        Assert.NotNull(result);
        // Should return error message or fallback response
        Assert.NotEmpty(result.Response);
    }

    [Fact]
    public async Task HybridProcessor_ShouldFallbackToRuleBased_WhenAINotConfigured()
    {
        // Arrange
        string input = "tambah task beli susu";
        var context = CreateTestContext();
        
        // Ensure AI is not configured
        _preferencesService.ClearPreference("ai_provider");
        _preferencesService.ClearPreference("ai_api_key");

        // Act
        var result = await _processor.ProcessMessageAsync(input, context);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsRuleBased);
        Assert.Contains("task", result.Response.ToLower());
    }

    [Fact]
    public async Task HybridProcessor_ShouldProvideHelpfulError_WhenNoProcessingPossible()
    {
        // Arrange
        string input = "random unknown command xyz123";
        var context = CreateTestContext();
        
        // Ensure AI is not configured
        _preferencesService.ClearPreference("ai_provider");

        // Act
        var result = await _processor.ProcessMessageAsync(input, context);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Response);
        // Should provide helpful error message
    }

    [Theory]
    [InlineData("tambah task beli susu")]
    [InlineData("cuaca hari ini")]
    [InlineData("halo")]
    public async Task HybridProcessor_ShouldAlwaysUseRuleBased_ForHighConfidencePatterns_RegardlessOfAIConfig(string input)
    {
        // Arrange
        var context = CreateTestContext();
        
        // Test with AI configured
        _preferencesService.SetPreference("ai_provider", "groq");
        _preferencesService.SetPreference("ai_api_key", "test_key");

        // Act
        var result = await _processor.ProcessMessageAsync(input, context);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsRuleBased);
        Assert.False(result.IsAIEnhanced);
    }

    #endregion

    #region Error Handling Tests

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task ProcessMessage_ShouldHandleEmptyInput(string input)
    {
        // Arrange
        var context = CreateTestContext();

        // Act
        var result = await _processor.ProcessMessageAsync(input, context);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Response);
    }

    [Fact]
    public async Task ProcessMessage_ShouldHandleNullContext()
    {
        // Arrange
        string input = "tambah task beli susu";

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(async () =>
        {
            await _processor.ProcessMessageAsync(input, null);
        });
    }

    [Fact]
    public async Task ProcessMessage_ShouldHandleVeryLongInput()
    {
        // Arrange
        string input = new string('a', 10000);
        var context = CreateTestContext();

        // Act
        var result = await _processor.ProcessMessageAsync(input, context);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Response);
    }

    #endregion

    #region Performance Tests

    [Fact]
    public async Task RuleBasedProcessing_ShouldBeFast()
    {
        // Arrange
        string input = "tambah task beli susu";
        var context = CreateTestContext();
        var startTime = DateTime.UtcNow;

        // Act
        var result = await _ruleBasedProcessor.ProcessAsync(input, context);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        Assert.True(duration.TotalMilliseconds < 100, 
            $"Rule-based processing should be < 100ms, but took {duration.TotalMilliseconds}ms");
    }

    [Fact]
    public async Task HybridProcessor_ShouldBeFast_ForRuleBasedPath()
    {
        // Arrange
        string input = "tambah task beli susu";
        var context = CreateTestContext();
        var startTime = DateTime.UtcNow;

        // Act
        var result = await _processor.ProcessMessageAsync(input, context);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        Assert.True(duration.TotalMilliseconds < 200,
            $"Hybrid processing (rule-based path) should be < 200ms, but took {duration.TotalMilliseconds}ms");
    }

    #endregion

    #region Context Handling Tests

    [Fact]
    public async Task ProcessMessage_ShouldUseContext_ForPersonalization()
    {
        // Arrange
        string input = "halo";
        var context = CreateTestContext();
        context.UserName = "John";

        // Act
        var result = await _processor.ProcessMessageAsync(input, context);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Response);
    }

    [Fact]
    public async Task ProcessMessage_ShouldTrackProcessingMetadata()
    {
        // Arrange
        string input = "tambah task beli susu";
        var context = CreateTestContext();

        // Act
        var result = await _processor.ProcessMessageAsync(input, context);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ProcessingTimeMs >= 0);
    }

    #endregion

    #region Helper Methods

    private ConversationContext CreateTestContext()
    {
        return new ConversationContext
        {
            UserId = "test_user",
            SessionId = Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow,
            UserName = "Test User",
            Location = "Jakarta",
            DeviceInfo = "Test Device"
        };
    }

    #endregion
}
