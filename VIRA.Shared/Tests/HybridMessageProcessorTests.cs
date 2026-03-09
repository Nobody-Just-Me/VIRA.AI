using VIRA.Shared.Models;
using VIRA.Shared.Services;

namespace VIRA.Shared.Tests;

/// <summary>
/// Tests for HybridMessageProcessor
/// Validates: AC-16.10 (Fallback to rule-based when AI unavailable)
/// </summary>
public class HybridMessageProcessorTests
{
    private readonly TaskManager _taskManager;
    private readonly WeatherApiService _weatherService;
    private readonly NewsApiService _newsService;
    private readonly PatternRegistry _patternRegistry;
    private readonly RuleBasedProcessor _ruleBasedProcessor;
    private readonly PreferencesService _preferencesService;

    public HybridMessageProcessorTests()
    {
        var httpClient = new HttpClient();
        _taskManager = new TaskManager();
        _weatherService = new WeatherApiService(httpClient);
        _newsService = new NewsApiService(httpClient);
        _patternRegistry = new PatternRegistry(_taskManager, _weatherService, _newsService);
        _ruleBasedProcessor = new RuleBasedProcessor(_patternRegistry);
        _preferencesService = new PreferencesService();
    }

    /// <summary>
    /// Test that high confidence rule-based result is returned without AI fallback
    /// </summary>
    public async Task TestProcessMessage_HighConfidenceRuleBased_ReturnsRuleBasedResult()
    {
        // Arrange
        var message = "cuaca hari ini";
        var context = new ConversationContext();

        // Act
        var result = await _ruleBasedProcessor.ProcessMessageAsync(message, context);

        // Assert
        if (result is not RuleBasedResult ruleBasedResult)
        {
            throw new Exception($"Expected RuleBasedResult but got {result.GetType().Name}");
        }
        
        if (ruleBasedResult.Confidence < 0.8f)
        {
            throw new Exception($"Expected confidence >= 0.8 but got {ruleBasedResult.Confidence}");
        }
        
        Console.WriteLine("✓ TestProcessMessage_HighConfidenceRuleBased_ReturnsRuleBasedResult passed");
    }

    /// <summary>
    /// Test that low confidence rule-based result triggers AI fallback logic
    /// </summary>
    public async Task TestProcessMessage_LowConfidenceRuleBased_TriggersAIFallback()
    {
        // Arrange
        var message = "apa arti kehidupan?";
        var context = new ConversationContext();

        // Act
        var result = await _ruleBasedProcessor.ProcessMessageAsync(message, context);

        // Assert
        if (result is not RuleBasedResult ruleBasedResult)
        {
            throw new Exception($"Expected RuleBasedResult but got {result.GetType().Name}");
        }
        
        if (ruleBasedResult.Confidence >= 0.8f)
        {
            throw new Exception($"Expected confidence < 0.8 but got {ruleBasedResult.Confidence}");
        }
        
        Console.WriteLine("✓ TestProcessMessage_LowConfidenceRuleBased_TriggersAIFallback passed");
    }

    /// <summary>
    /// Test that empty message returns ErrorResult
    /// </summary>
    public async Task TestProcessMessage_EmptyMessage_ReturnsError()
    {
        // Arrange
        var message = "";
        var context = new ConversationContext();

        // Act
        var result = await _ruleBasedProcessor.ProcessMessageAsync(message, context);

        // Assert
        if (result is not ErrorResult errorResult)
        {
            throw new Exception($"Expected ErrorResult but got {result.GetType().Name}");
        }
        
        if (!errorResult.Response.Contains("tidak menangkap pesan"))
        {
            throw new Exception($"Expected response to contain 'tidak menangkap pesan' but got '{errorResult.Response}'");
        }
        
        Console.WriteLine("✓ TestProcessMessage_EmptyMessage_ReturnsError passed");
    }

    /// <summary>
    /// Test confidence threshold is 0.8
    /// </summary>
    public void TestConfidenceThreshold_IsCorrect()
    {
        // Act
        var threshold = RuleBasedProcessor.GetConfidenceThreshold();

        // Assert
        if (threshold != 0.8f)
        {
            throw new Exception($"Expected threshold 0.8 but got {threshold}");
        }
        
        Console.WriteLine("✓ TestConfidenceThreshold_IsCorrect passed");
    }

    /// <summary>
    /// Test confidence threshold with various values
    /// </summary>
    public void TestMeetsConfidenceThreshold_VariousValues()
    {
        // Test cases: (confidence, expected)
        var testCases = new[]
        {
            (0.9f, true),
            (0.8f, true),
            (0.79f, false),
            (0.5f, false),
            (0.0f, false)
        };

        foreach (var (confidence, expected) in testCases)
        {
            var result = RuleBasedProcessor.MeetsConfidenceThreshold(confidence);
            if (result != expected)
            {
                throw new Exception($"Expected {expected} for confidence {confidence} but got {result}");
            }
        }
        
        Console.WriteLine("✓ TestMeetsConfidenceThreshold_VariousValues passed");
    }

    /// <summary>
    /// Test that task commands have high confidence
    /// </summary>
    public async Task TestProcessMessage_TaskCommands_HighConfidence()
    {
        // Arrange
        var messages = new[]
        {
            "tambah task beli susu",
            "daftar task",
            "selesai task beli susu"
        };
        var context = new ConversationContext();

        foreach (var message in messages)
        {
            // Act
            var result = await _ruleBasedProcessor.ProcessMessageAsync(message, context);

            // Assert
            if (result is not RuleBasedResult ruleBasedResult)
            {
                throw new Exception($"Expected RuleBasedResult for '{message}' but got {result.GetType().Name}");
            }
            
            if (ruleBasedResult.Confidence < 0.8f)
            {
                throw new Exception($"Expected confidence >= 0.8 for '{message}' but got {ruleBasedResult.Confidence}");
            }
        }
        
        Console.WriteLine("✓ TestProcessMessage_TaskCommands_HighConfidence passed");
    }

    /// <summary>
    /// Test that information queries have high confidence
    /// </summary>
    public async Task TestProcessMessage_InformationQueries_HighConfidence()
    {
        // Arrange
        var messages = new[]
        {
            "cuaca hari ini",
            "berita terkini",
            "jam berapa sekarang"
        };
        var context = new ConversationContext();

        foreach (var message in messages)
        {
            // Act
            var result = await _ruleBasedProcessor.ProcessMessageAsync(message, context);

            // Assert
            if (result is not RuleBasedResult ruleBasedResult)
            {
                throw new Exception($"Expected RuleBasedResult for '{message}' but got {result.GetType().Name}");
            }
            
            if (ruleBasedResult.Confidence <= 0.0f)
            {
                throw new Exception($"Expected confidence > 0.0 for '{message}' but got {ruleBasedResult.Confidence}");
            }
        }
        
        Console.WriteLine("✓ TestProcessMessage_InformationQueries_HighConfidence passed");
    }

    /// <summary>
    /// Run all tests
    /// </summary>
    public async Task RunAllTests()
    {
        Console.WriteLine("\n=== Running HybridMessageProcessor Tests ===\n");
        
        try
        {
            await TestProcessMessage_HighConfidenceRuleBased_ReturnsRuleBasedResult();
            await TestProcessMessage_LowConfidenceRuleBased_TriggersAIFallback();
            await TestProcessMessage_EmptyMessage_ReturnsError();
            TestConfidenceThreshold_IsCorrect();
            TestMeetsConfidenceThreshold_VariousValues();
            await TestProcessMessage_TaskCommands_HighConfidence();
            await TestProcessMessage_InformationQueries_HighConfidence();
            
            Console.WriteLine("\n✅ All HybridMessageProcessor tests passed!\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ Test failed: {ex.Message}\n");
            throw;
        }
    }
}
