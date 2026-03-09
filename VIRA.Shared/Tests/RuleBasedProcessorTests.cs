using VIRA.Shared.Models;
using VIRA.Shared.Services;

namespace VIRA.Shared.Tests;

/// <summary>
/// Unit tests for RuleBasedProcessor
/// Validates pattern matching logic, confidence scores, and error handling
/// </summary>
public class RuleBasedProcessorTests
{
    private readonly TaskManager _taskManager;
    private readonly WeatherApiService _weatherService;
    private readonly NewsApiService _newsService;
    private readonly PatternRegistry _patternRegistry;
    private readonly RuleBasedProcessor _processor;

    public RuleBasedProcessorTests()
    {
        var httpClient = new HttpClient();
        _taskManager = new TaskManager();
        _weatherService = new WeatherApiService(httpClient);
        _newsService = new NewsApiService(httpClient);
        _patternRegistry = new PatternRegistry(_taskManager, _weatherService, _newsService);
        _processor = new RuleBasedProcessor(_patternRegistry);
    }

    /// <summary>
    /// Test that empty message returns ErrorResult
    /// </summary>
    public async Task TestProcessMessageAsync_WithEmptyMessage_ReturnsErrorResult()
    {
        // Arrange
        var context = new ConversationContext();

        // Act
        var result = await _processor.ProcessMessageAsync("", context);

        // Assert
        if (result is not ErrorResult errorResult)
        {
            throw new Exception($"Expected ErrorResult but got {result.GetType().Name}");
        }
        
        if (errorResult.ErrorMessage != "Empty message")
        {
            throw new Exception($"Expected error message 'Empty message' but got '{errorResult.ErrorMessage}'");
        }
        
        Console.WriteLine("✓ TestProcessMessageAsync_WithEmptyMessage_ReturnsErrorResult passed");
    }

    /// <summary>
    /// Test that unknown command returns low confidence
    /// </summary>
    public async Task TestProcessMessageAsync_WithUnknownCommand_ReturnsLowConfidence()
    {
        // Arrange
        var context = new ConversationContext();
        var message = "xyz123 unknown command that should not match";

        // Act
        var result = await _processor.ProcessMessageAsync(message, context);

        // Assert
        if (result is not RuleBasedResult ruleResult)
        {
            throw new Exception($"Expected RuleBasedResult but got {result.GetType().Name}");
        }
        
        if (ruleResult.Confidence != 0.0f)
        {
            throw new Exception($"Expected confidence 0.0 but got {ruleResult.Confidence}");
        }
        
        if (!ruleResult.Response.ToLower().Contains("tidak mengerti"))
        {
            throw new Exception($"Expected response to contain 'tidak mengerti' but got '{ruleResult.Response}'");
        }
        
        Console.WriteLine("✓ TestProcessMessageAsync_WithUnknownCommand_ReturnsLowConfidence passed");
    }

    /// <summary>
    /// Test that add task command returns high confidence
    /// </summary>
    public async Task TestProcessMessageAsync_WithAddTaskCommand_ReturnsHighConfidence()
    {
        // Arrange
        var context = new ConversationContext();
        var message = "tambah task beli susu";

        // Act
        var result = await _processor.ProcessMessageAsync(message, context);

        // Assert
        if (result is not RuleBasedResult ruleResult)
        {
            throw new Exception($"Expected RuleBasedResult but got {result.GetType().Name}");
        }
        
        if (ruleResult.Confidence < 0.8f)
        {
            throw new Exception($"Expected confidence >= 0.8 but got {ruleResult.Confidence}");
        }
        
        if (!ruleResult.Response.ToLower().Contains("beli susu"))
        {
            throw new Exception($"Expected response to contain 'beli susu' but got '{ruleResult.Response}'");
        }
        
        Console.WriteLine("✓ TestProcessMessageAsync_WithAddTaskCommand_ReturnsHighConfidence passed");
    }

    /// <summary>
    /// Test that weather query returns RuleBasedResult
    /// </summary>
    public async Task TestProcessMessageAsync_WithWeatherQuery_ReturnsRuleBasedResult()
    {
        // Arrange
        var context = new ConversationContext();
        var message = "cuaca hari ini";

        // Act
        var result = await _processor.ProcessMessageAsync(message, context);

        // Assert
        if (result is not RuleBasedResult ruleResult)
        {
            throw new Exception($"Expected RuleBasedResult but got {result.GetType().Name}");
        }
        
        if (string.IsNullOrEmpty(ruleResult.Response))
        {
            throw new Exception("Expected non-empty response");
        }
        
        if (ruleResult.Confidence <= 0.0f)
        {
            throw new Exception($"Expected confidence > 0.0 but got {ruleResult.Confidence}");
        }
        
        Console.WriteLine("✓ TestProcessMessageAsync_WithWeatherQuery_ReturnsRuleBasedResult passed");
    }

    /// <summary>
    /// Test that greeting returns RuleBasedResult
    /// </summary>
    public async Task TestProcessMessageAsync_WithGreeting_ReturnsRuleBasedResult()
    {
        // Arrange
        var context = new ConversationContext();
        var message = "halo VIRA";

        // Act
        var result = await _processor.ProcessMessageAsync(message, context);

        // Assert
        if (result is not RuleBasedResult ruleResult)
        {
            throw new Exception($"Expected RuleBasedResult but got {result.GetType().Name}");
        }
        
        if (string.IsNullOrEmpty(ruleResult.Response))
        {
            throw new Exception("Expected non-empty response");
        }
        
        if (ruleResult.Confidence <= 0.0f)
        {
            throw new Exception($"Expected confidence > 0.0 but got {ruleResult.Confidence}");
        }
        
        Console.WriteLine("✓ TestProcessMessageAsync_WithGreeting_ReturnsRuleBasedResult passed");
    }

    /// <summary>
    /// Test confidence threshold with high confidence
    /// </summary>
    public void TestMeetsConfidenceThreshold_WithHighConfidence_ReturnsTrue()
    {
        // Arrange
        float confidence = 0.9f;

        // Act
        bool result = RuleBasedProcessor.MeetsConfidenceThreshold(confidence);

        // Assert
        if (!result)
        {
            throw new Exception($"Expected true but got false for confidence {confidence}");
        }
        
        Console.WriteLine("✓ TestMeetsConfidenceThreshold_WithHighConfidence_ReturnsTrue passed");
    }

    /// <summary>
    /// Test confidence threshold with low confidence
    /// </summary>
    public void TestMeetsConfidenceThreshold_WithLowConfidence_ReturnsFalse()
    {
        // Arrange
        float confidence = 0.5f;

        // Act
        bool result = RuleBasedProcessor.MeetsConfidenceThreshold(confidence);

        // Assert
        if (result)
        {
            throw new Exception($"Expected false but got true for confidence {confidence}");
        }
        
        Console.WriteLine("✓ TestMeetsConfidenceThreshold_WithLowConfidence_ReturnsFalse passed");
    }

    /// <summary>
    /// Test confidence threshold with exact threshold value
    /// </summary>
    public void TestMeetsConfidenceThreshold_WithExactThreshold_ReturnsTrue()
    {
        // Arrange
        float confidence = 0.8f;

        // Act
        bool result = RuleBasedProcessor.MeetsConfidenceThreshold(confidence);

        // Assert
        if (!result)
        {
            throw new Exception($"Expected true but got false for confidence {confidence}");
        }
        
        Console.WriteLine("✓ TestMeetsConfidenceThreshold_WithExactThreshold_ReturnsTrue passed");
    }

    /// <summary>
    /// Test get confidence threshold returns correct value
    /// </summary>
    public void TestGetConfidenceThreshold_ReturnsCorrectValue()
    {
        // Act
        float threshold = RuleBasedProcessor.GetConfidenceThreshold();

        // Assert
        if (threshold != 0.8f)
        {
            throw new Exception($"Expected threshold 0.8 but got {threshold}");
        }
        
        Console.WriteLine("✓ TestGetConfidenceThreshold_ReturnsCorrectValue passed");
    }

    /// <summary>
    /// Run all tests
    /// </summary>
    public async Task RunAllTests()
    {
        Console.WriteLine("\n=== Running RuleBasedProcessor Tests ===\n");
        
        try
        {
            await TestProcessMessageAsync_WithEmptyMessage_ReturnsErrorResult();
            await TestProcessMessageAsync_WithUnknownCommand_ReturnsLowConfidence();
            await TestProcessMessageAsync_WithAddTaskCommand_ReturnsHighConfidence();
            await TestProcessMessageAsync_WithWeatherQuery_ReturnsRuleBasedResult();
            await TestProcessMessageAsync_WithGreeting_ReturnsRuleBasedResult();
            TestMeetsConfidenceThreshold_WithHighConfidence_ReturnsTrue();
            TestMeetsConfidenceThreshold_WithLowConfidence_ReturnsFalse();
            TestMeetsConfidenceThreshold_WithExactThreshold_ReturnsTrue();
            TestGetConfidenceThreshold_ReturnsCorrectValue();
            
            Console.WriteLine("\n✅ All RuleBasedProcessor tests passed!\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ Test failed: {ex.Message}\n");
            throw;
        }
    }
}

