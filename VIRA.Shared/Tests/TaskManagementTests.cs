using VIRA.Shared.Models;
using VIRA.Shared.Services;
using VIRA.Shared.Services.Handlers;

namespace VIRA.Shared.Tests;

/// <summary>
/// Tests for task management patterns and handlers
/// </summary>
public class TaskManagementTests
{
    private TaskManager _taskManager;
    private PatternRegistry _patternRegistry;
    private ConversationContext _context;

    public TaskManagementTests()
    {
        _taskManager = new TaskManager();
        
        // Create mock services for testing
        var httpClient = new HttpClient();
        var weatherService = new WeatherApiService(httpClient);
        var newsService = new NewsApiService(httpClient);
        
        _patternRegistry = new PatternRegistry(_taskManager, weatherService, newsService);
        _context = new ConversationContext();
    }

    /// <summary>
    /// Test adding a task with Indonesian command
    /// </summary>
    public async Task TestAddTaskIndonesian()
    {
        var input = "tambah task beli susu";
        var match = _patternRegistry.FindMatch(input);
        
        if (match == null)
        {
            throw new Exception("Pattern not matched for: " + input);
        }

        var result = await match.Pattern.Handler.HandleAsync(match.Match, _context);
        
        if (!result.Response.Contains("beli susu"))
        {
            throw new Exception("Task not added correctly");
        }

        if (_taskManager.GetActiveTaskCount() != 1)
        {
            throw new Exception("Task count incorrect");
        }

        Console.WriteLine("✅ TestAddTaskIndonesian passed");
    }

    /// <summary>
    /// Test adding a task with English command
    /// </summary>
    public async Task TestAddTaskEnglish()
    {
        var input = "add task buy milk";
        var match = _patternRegistry.FindMatch(input);
        
        if (match == null)
        {
            throw new Exception("Pattern not matched for: " + input);
        }

        var result = await match.Pattern.Handler.HandleAsync(match.Match, _context);
        
        if (!result.Response.Contains("buy milk"))
        {
            throw new Exception("Task not added correctly");
        }

        Console.WriteLine("✅ TestAddTaskEnglish passed");
    }

    /// <summary>
    /// Test listing tasks
    /// </summary>
    public async Task TestListTasks()
    {
        // Add some tasks first
        _taskManager.AddTask("Task 1");
        _taskManager.AddTask("Task 2");
        _taskManager.AddTask("Task 3");

        var input = "daftar task";
        var match = _patternRegistry.FindMatch(input);
        
        if (match == null)
        {
            throw new Exception("Pattern not matched for: " + input);
        }

        var result = await match.Pattern.Handler.HandleAsync(match.Match, _context);
        
        if (!result.Response.Contains("Task 1") || 
            !result.Response.Contains("Task 2") || 
            !result.Response.Contains("Task 3"))
        {
            throw new Exception("Tasks not listed correctly");
        }

        Console.WriteLine("✅ TestListTasks passed");
    }

    /// <summary>
    /// Test completing a task
    /// </summary>
    public async Task TestCompleteTask()
    {
        // Add a task first
        _taskManager.AddTask("Complete this task");

        var input = "selesai task Complete";
        var match = _patternRegistry.FindMatch(input);
        
        if (match == null)
        {
            throw new Exception("Pattern not matched for: " + input);
        }

        var result = await match.Pattern.Handler.HandleAsync(match.Match, _context);
        
        if (!result.Response.Contains("selesai"))
        {
            throw new Exception("Task not completed correctly");
        }

        if (_taskManager.GetActiveTaskCount() != 0)
        {
            throw new Exception("Task still active after completion");
        }

        Console.WriteLine("✅ TestCompleteTask passed");
    }

    /// <summary>
    /// Test deleting a task
    /// </summary>
    public async Task TestDeleteTask()
    {
        // Add a task first
        _taskManager.AddTask("Delete this task");

        var input = "hapus task Delete";
        var match = _patternRegistry.FindMatch(input);
        
        if (match == null)
        {
            throw new Exception("Pattern not matched for: " + input);
        }

        var result = await match.Pattern.Handler.HandleAsync(match.Match, _context);
        
        if (!result.Response.Contains("dihapus"))
        {
            throw new Exception("Task not deleted correctly");
        }

        if (_taskManager.GetTaskCount() != 0)
        {
            throw new Exception("Task still exists after deletion");
        }

        Console.WriteLine("✅ TestDeleteTask passed");
    }

    /// <summary>
    /// Run all tests
    /// </summary>
    public async Task RunAllTests()
    {
        Console.WriteLine("Running Task Management Tests...\n");

        try
        {
            await TestAddTaskIndonesian();
            _taskManager.ClearAllTasks();

            await TestAddTaskEnglish();
            _taskManager.ClearAllTasks();

            await TestListTasks();
            _taskManager.ClearAllTasks();

            await TestCompleteTask();
            _taskManager.ClearAllTasks();

            await TestDeleteTask();
            _taskManager.ClearAllTasks();

            Console.WriteLine("\n✅ All tests passed!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ Test failed: {ex.Message}");
            throw;
        }
    }
}
