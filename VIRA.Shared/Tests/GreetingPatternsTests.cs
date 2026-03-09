using VIRA.Shared.Models;
using VIRA.Shared.Services;
using VIRA.Shared.Services.Handlers;

namespace VIRA.Shared.Tests;

/// <summary>
/// Tests for greeting and conversation patterns (Task 2.5)
/// Validates: AC-4.1 - Time-based greetings and conversation patterns
/// </summary>
public class GreetingPatternsTests
{
    private readonly PatternRegistry _registry;
    private readonly ConversationContext _context;

    public GreetingPatternsTests()
    {
        var taskManager = new TaskManager();
        var httpClient = new HttpClient();
        var weatherService = new WeatherApiService(httpClient);
        var newsService = new NewsApiService(httpClient);
        
        _registry = new PatternRegistry(taskManager, weatherService, newsService);
        _context = new ConversationContext();
    }

    /// <summary>
    /// Test greeting patterns match Indonesian and English greetings
    /// </summary>
    public async Task TestGreetingPatterns()
    {
        var inputs = new[] { "halo", "hello", "hi", "hey", "selamat pagi" };

        foreach (var input in inputs)
        {
            var match = _registry.FindMatch(input);
            
            if (match == null || match.Pattern.Id != "greeting")
            {
                throw new Exception($"Pattern match failed for '{input}'");
            }

            var result = await match.Pattern.Handler.HandleAsync(match.Match, _context);
            
            if (result == null || !result.Speak || result.Confidence < 0.8f)
            {
                throw new Exception($"Handler failed for '{input}'");
            }

            Console.WriteLine($"✓ Greeting pattern '{input}' passed: {result.Response}");
        }
    }

    /// <summary>
    /// Test status query patterns
    /// </summary>
    public async Task TestStatusQueryPatterns()
    {
        var inputs = new[] { "apa kabar", "how are you", "gimana" };

        foreach (var input in inputs)
        {
            var match = _registry.FindMatch(input);
            
            if (match == null || match.Pattern.Id != "status_query")
            {
                throw new Exception($"Pattern match failed for '{input}'");
            }

            var result = await match.Pattern.Handler.HandleAsync(match.Match, _context);
            
            if (result == null || !result.Speak || result.Confidence < 0.8f)
            {
                throw new Exception($"Handler failed for '{input}'");
            }

            if (!result.Response.ToLower().Contains("baik"))
            {
                throw new Exception($"Expected response to contain 'baik' for '{input}'");
            }

            Console.WriteLine($"✓ Status query pattern '{input}' passed: {result.Response}");
        }
    }

    /// <summary>
    /// Test thank you patterns
    /// </summary>
    public async Task TestThankYouPatterns()
    {
        var inputs = new[] { "terima kasih", "thanks", "thank you", "makasih" };

        foreach (var input in inputs)
        {
            var match = _registry.FindMatch(input);
            
            if (match == null || match.Pattern.Id != "thank_you")
            {
                throw new Exception($"Pattern match failed for '{input}'");
            }

            var result = await match.Pattern.Handler.HandleAsync(match.Match, _context);
            
            if (result == null || !result.Speak || result.Confidence < 0.8f)
            {
                throw new Exception($"Handler failed for '{input}'");
            }

            if (!result.Response.ToLower().Contains("sama"))
            {
                throw new Exception($"Expected response to contain 'sama' for '{input}'");
            }

            Console.WriteLine($"✓ Thank you pattern '{input}' passed: {result.Response}");
        }
    }

    /// <summary>
    /// Test goodbye patterns
    /// </summary>
    public async Task TestGoodbyePatterns()
    {
        var inputs = new[] { "bye", "goodbye", "sampai jumpa", "dadah" };

        foreach (var input in inputs)
        {
            var match = _registry.FindMatch(input);
            
            if (match == null || match.Pattern.Id != "goodbye")
            {
                throw new Exception($"Pattern match failed for '{input}'");
            }

            var result = await match.Pattern.Handler.HandleAsync(match.Match, _context);
            
            if (result == null || !result.Speak || result.Confidence < 0.8f)
            {
                throw new Exception($"Handler failed for '{input}'");
            }

            Console.WriteLine($"✓ Goodbye pattern '{input}' passed: {result.Response}");
        }
    }

    /// <summary>
    /// Test that all greeting patterns are bilingual (Indonesian and English)
    /// </summary>
    public void TestBilingualSupport()
    {
        // Test Indonesian patterns
        var indonesianPatterns = new[] { "halo", "apa kabar", "terima kasih", "sampai jumpa" };
        foreach (var pattern in indonesianPatterns)
        {
            if (_registry.FindMatch(pattern) == null)
            {
                throw new Exception($"Indonesian pattern '{pattern}' not found");
            }
        }

        // Test English patterns
        var englishPatterns = new[] { "hello", "how are you", "thanks", "goodbye" };
        foreach (var pattern in englishPatterns)
        {
            if (_registry.FindMatch(pattern) == null)
            {
                throw new Exception($"English pattern '{pattern}' not found");
            }
        }

        Console.WriteLine("✓ Bilingual support test passed");
    }
}
