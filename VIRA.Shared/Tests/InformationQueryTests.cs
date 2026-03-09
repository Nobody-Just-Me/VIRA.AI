using VIRA.Shared.Models;
using VIRA.Shared.Services;

namespace VIRA.Shared.Tests;

/// <summary>
/// Tests for information query patterns and handlers
/// </summary>
public class InformationQueryTests
{
    private TaskManager _taskManager;
    private PatternRegistry _patternRegistry;
    private ConversationContext _context;
    private WeatherApiService _weatherService;
    private NewsApiService _newsService;

    public InformationQueryTests()
    {
        _taskManager = new TaskManager();
        
        // Create services for testing
        var httpClient = new HttpClient();
        _weatherService = new WeatherApiService(httpClient);
        _newsService = new NewsApiService(httpClient);
        
        _patternRegistry = new PatternRegistry(_taskManager, _weatherService, _newsService);
        _context = new ConversationContext();
        
        // Set up device state for battery test
        _context.DeviceState["battery_level"] = 85;
        _context.DeviceState["is_charging"] = false;
    }

    /// <summary>
    /// Test weather query pattern matching (Indonesian)
    /// </summary>
    public async Task TestWeatherPatternIndonesian()
    {
        var input = "cuaca hari ini";
        var match = _patternRegistry.FindMatch(input);
        
        if (match == null)
        {
            throw new Exception("Weather pattern not matched for: " + input);
        }

        if (match.Pattern.Id != "weather")
        {
            throw new Exception($"Wrong pattern matched: {match.Pattern.Id}");
        }

        var result = await match.Pattern.Handler.HandleAsync(match.Match, _context);
        
        if (result.Confidence != 1.0f)
        {
            throw new Exception("Confidence should be 1.0");
        }

        Console.WriteLine("✅ TestWeatherPatternIndonesian passed");
    }

    /// <summary>
    /// Test weather query pattern matching (English)
    /// </summary>
    public async Task TestWeatherPatternEnglish()
    {
        var input = "what's the weather";
        var match = _patternRegistry.FindMatch(input);
        
        if (match == null)
        {
            throw new Exception("Weather pattern not matched for: " + input);
        }

        if (match.Pattern.Id != "weather")
        {
            throw new Exception($"Wrong pattern matched: {match.Pattern.Id}");
        }

        Console.WriteLine("✅ TestWeatherPatternEnglish passed");
    }

    /// <summary>
    /// Test news query pattern matching
    /// </summary>
    public async Task TestNewsPattern()
    {
        var inputs = new[] { "berita terkini", "news today", "headline hari ini", "kabar terbaru" };
        
        foreach (var input in inputs)
        {
            var match = _patternRegistry.FindMatch(input);
            
            if (match == null)
            {
                throw new Exception($"News pattern not matched for: {input}");
            }

            if (match.Pattern.Id != "news")
            {
                throw new Exception($"Wrong pattern matched for '{input}': {match.Pattern.Id}");
            }
        }

        Console.WriteLine("✅ TestNewsPattern passed");
    }

    /// <summary>
    /// Test schedule query pattern matching
    /// </summary>
    public async Task TestSchedulePattern()
    {
        var inputs = new[] { "jadwal hari ini", "schedule today", "agenda saya", "appointment" };
        
        foreach (var input in inputs)
        {
            var match = _patternRegistry.FindMatch(input);
            
            if (match == null)
            {
                throw new Exception($"Schedule pattern not matched for: {input}");
            }

            if (match.Pattern.Id != "schedule")
            {
                throw new Exception($"Wrong pattern matched for '{input}': {match.Pattern.Id}");
            }
        }

        Console.WriteLine("✅ TestSchedulePattern passed");
    }

    /// <summary>
    /// Test time query pattern matching
    /// </summary>
    public async Task TestTimePattern()
    {
        var inputs = new[] { "jam berapa", "what time", "waktu sekarang", "pukul berapa" };
        
        foreach (var input in inputs)
        {
            var match = _patternRegistry.FindMatch(input);
            
            if (match == null)
            {
                throw new Exception($"Time pattern not matched for: {input}");
            }

            if (match.Pattern.Id != "time")
            {
                throw new Exception($"Wrong pattern matched for '{input}': {match.Pattern.Id}");
            }

            var result = await match.Pattern.Handler.HandleAsync(match.Match, _context);
            
            if (!result.Response.Contains("Waktu Saat Ini"))
            {
                throw new Exception("Time response should contain 'Waktu Saat Ini'");
            }
        }

        Console.WriteLine("✅ TestTimePattern passed");
    }

    /// <summary>
    /// Test battery query pattern matching
    /// </summary>
    public async Task TestBatteryPattern()
    {
        var inputs = new[] { "baterai", "battery level", "daya baterai", "power" };
        
        foreach (var input in inputs)
        {
            var match = _patternRegistry.FindMatch(input);
            
            if (match == null)
            {
                throw new Exception($"Battery pattern not matched for: {input}");
            }

            if (match.Pattern.Id != "battery")
            {
                throw new Exception($"Wrong pattern matched for '{input}': {match.Pattern.Id}");
            }

            var result = await match.Pattern.Handler.HandleAsync(match.Match, _context);
            
            if (!result.Response.Contains("85%"))
            {
                throw new Exception("Battery response should contain '85%'");
            }
        }

        Console.WriteLine("✅ TestBatteryPattern passed");
    }

    /// <summary>
    /// Test schedule handler with tasks
    /// </summary>
    public async Task TestScheduleWithTasks()
    {
        // Add tasks with due dates
        var task1 = _taskManager.AddTask("Meeting with team");
        task1.DueDate = DateTime.Today.AddHours(10);
        task1.Priority = TaskPriority.HIGH;

        var task2 = _taskManager.AddTask("Lunch break");
        task2.DueDate = DateTime.Today.AddHours(12);
        task2.Priority = TaskPriority.MEDIUM;

        var input = "jadwal hari ini";
        var match = _patternRegistry.FindMatch(input);
        
        if (match == null)
        {
            throw new Exception("Schedule pattern not matched");
        }

        var result = await match.Pattern.Handler.HandleAsync(match.Match, _context);
        
        if (!result.Response.Contains("Meeting with team"))
        {
            throw new Exception("Schedule should contain 'Meeting with team'");
        }

        if (!result.Response.Contains("Lunch break"))
        {
            throw new Exception("Schedule should contain 'Lunch break'");
        }

        // Clean up
        _taskManager.ClearAllTasks();

        Console.WriteLine("✅ TestScheduleWithTasks passed");
    }

    /// <summary>
    /// Test schedule handler with no tasks
    /// </summary>
    public async Task TestScheduleEmpty()
    {
        var input = "jadwal hari ini";
        var match = _patternRegistry.FindMatch(input);
        
        if (match == null)
        {
            throw new Exception("Schedule pattern not matched");
        }

        var result = await match.Pattern.Handler.HandleAsync(match.Match, _context);
        
        if (!result.Response.Contains("tidak memiliki jadwal"))
        {
            throw new Exception("Empty schedule should indicate no tasks");
        }

        Console.WriteLine("✅ TestScheduleEmpty passed");
    }

    /// <summary>
    /// Run all tests
    /// </summary>
    public async Task RunAllTests()
    {
        Console.WriteLine("Running Information Query Tests...\n");

        try
        {
            await TestWeatherPatternIndonesian();
            await TestWeatherPatternEnglish();
            await TestNewsPattern();
            await TestSchedulePattern();
            await TestTimePattern();
            await TestBatteryPattern();
            await TestScheduleWithTasks();
            await TestScheduleEmpty();

            Console.WriteLine("\n✅ All information query tests passed!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ Test failed: {ex.Message}");
            throw;
        }
    }
}
