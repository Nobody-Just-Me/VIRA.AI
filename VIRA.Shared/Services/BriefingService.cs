using VIRA.Shared.Models;

namespace VIRA.Shared.Services;

/// <summary>
/// Configuration for briefing service
/// </summary>
public class BriefingConfig
{
    /// <summary>
    /// Enable/disable morning briefing
    /// </summary>
    public bool MorningBriefingEnabled { get; set; } = true;
    
    /// <summary>
    /// Time for morning briefing (24-hour format)
    /// </summary>
    public TimeSpan MorningBriefingTime { get; set; } = new TimeSpan(8, 0, 0); // 8:00 AM
    
    /// <summary>
    /// Enable/disable evening summary
    /// </summary>
    public bool EveningSummaryEnabled { get; set; } = true;
    
    /// <summary>
    /// Time for evening summary (24-hour format)
    /// </summary>
    public TimeSpan EveningSummaryTime { get; set; } = new TimeSpan(20, 0, 0); // 8:00 PM
}

/// <summary>
/// Service for generating morning briefings and evening summaries
/// </summary>
public class BriefingService
{
    private readonly TaskManager _taskManager;
    private readonly WeatherApiService _weatherService;
    private readonly NewsApiService _newsService;
    private readonly BriefingConfig _config;

    public BriefingService(
        TaskManager taskManager,
        WeatherApiService weatherService,
        NewsApiService newsService,
        BriefingConfig? config = null)
    {
        _taskManager = taskManager;
        _weatherService = weatherService;
        _newsService = newsService;
        _config = config ?? new BriefingConfig();
    }

    /// <summary>
    /// Generate morning briefing with weather, schedule, news, and tasks
    /// </summary>
    public async Task<string> GenerateMorningBriefingAsync(string? location = null)
    {
        if (!_config.MorningBriefingEnabled)
        {
            return string.Empty;
        }

        var briefing = "🌅 **Selamat Pagi! Berikut ringkasan hari ini:**\n\n";

        // Add weather forecast
        try
        {
            var weather = await _weatherService.GetWeatherAsync(location ?? "Jakarta");
            if (weather != null)
            {
                briefing += $"🌤️ **Cuaca:** {weather.Description}, {weather.Temp}\n";
                briefing += $"   Feels like: {weather.FeelsLike}, Humidity: {weather.Humidity}\n";
                briefing += "\n";
            }
        }
        catch
        {
            // Skip weather if API fails
        }

        // Add today's tasks
        var todayTasks = _taskManager.GetActiveTasks()
            .Where(t => t.DueDate.HasValue && t.DueDate.Value.Date == DateTime.Today)
            .ToList();

        if (todayTasks.Count > 0)
        {
            briefing += $"📋 **Task Hari Ini ({todayTasks.Count}):**\n";
            foreach (var task in todayTasks.Take(5))
            {
                string priorityIcon = task.Priority switch
                {
                    TaskPriority.HIGH => "🔴",
                    TaskPriority.MEDIUM => "🟡",
                    TaskPriority.LOW => "🟢",
                    _ => "⚪"
                };
                briefing += $"   {priorityIcon} {task.Title}\n";
            }
            if (todayTasks.Count > 5)
            {
                briefing += $"   ... dan {todayTasks.Count - 5} task lainnya\n";
            }
            briefing += "\n";
        }

        // Add overdue tasks warning
        var overdueTasks = _taskManager.GetActiveTasks()
            .Where(t => t.DueDate.HasValue && t.DueDate.Value < DateTime.Now)
            .ToList();

        if (overdueTasks.Count > 0)
        {
            briefing += $"⚠️ **Perhatian:** {overdueTasks.Count} task melewati deadline\n\n";
        }

        // Add top news headlines
        try
        {
            var news = await _newsService.GetTopHeadlinesAsync("id", 3);
            if (news != null && news.Count > 0)
            {
                briefing += "📰 **Berita Terkini:**\n";
                foreach (var article in news)
                {
                    briefing += $"   • {article.Title}\n";
                }
                briefing += "\n";
            }
        }
        catch
        {
            // Skip news if API fails
        }

        briefing += "Semoga hari Anda menyenangkan! 😊";

        return briefing;
    }

    /// <summary>
    /// Generate evening summary with completed/incomplete tasks and tomorrow's preview
    /// </summary>
    public async Task<string> GenerateEveningSummaryAsync()
    {
        if (!_config.EveningSummaryEnabled)
        {
            return string.Empty;
        }

        var summary = "🌙 **Ringkasan Malam Hari**\n\n";

        // Completed tasks today
        var completedToday = _taskManager.GetCompletedTasks()
            .Where(t => t.CompletedAt.HasValue && 
                       t.CompletedAt.Value.Date == DateTime.Today)
            .ToList();

        if (completedToday.Count > 0)
        {
            summary += $"✅ **Task Selesai Hari Ini:** {completedToday.Count}\n";
            foreach (var task in completedToday.Take(5))
            {
                summary += $"   • {task.Title}\n";
            }
            if (completedToday.Count > 5)
            {
                summary += $"   ... dan {completedToday.Count - 5} task lainnya\n";
            }
            summary += "\n";
        }
        else
        {
            summary += "Tidak ada task yang diselesaikan hari ini.\n\n";
        }

        // Incomplete tasks
        var incompleteTasks = _taskManager.GetActiveTasks();
        if (incompleteTasks.Count > 0)
        {
            summary += $"📋 **Task Belum Selesai:** {incompleteTasks.Count}\n";
            
            var highPriority = incompleteTasks.Where(t => t.Priority == TaskPriority.HIGH).ToList();
            if (highPriority.Count > 0)
            {
                summary += $"   🔴 Prioritas tinggi: {highPriority.Count}\n";
            }
            summary += "\n";
        }

        // Tomorrow's tasks preview
        var tomorrowTasks = _taskManager.GetActiveTasks()
            .Where(t => t.DueDate.HasValue && 
                       t.DueDate.Value.Date == DateTime.Today.AddDays(1))
            .ToList();

        if (tomorrowTasks.Count > 0)
        {
            summary += $"📅 **Preview Besok ({tomorrowTasks.Count} task):**\n";
            foreach (var task in tomorrowTasks.Take(3))
            {
                string priorityIcon = task.Priority switch
                {
                    TaskPriority.HIGH => "🔴",
                    TaskPriority.MEDIUM => "🟡",
                    TaskPriority.LOW => "🟢",
                    _ => "⚪"
                };
                summary += $"   {priorityIcon} {task.Title}\n";
            }
            if (tomorrowTasks.Count > 3)
            {
                summary += $"   ... dan {tomorrowTasks.Count - 3} task lainnya\n";
            }
            summary += "\n";
        }

        summary += "Selamat beristirahat! 😴";

        return summary;
    }

    /// <summary>
    /// Check if it's time for morning briefing
    /// </summary>
    public bool IsMorningBriefingTime()
    {
        if (!_config.MorningBriefingEnabled)
        {
            return false;
        }

        var now = DateTime.Now.TimeOfDay;
        var briefingTime = _config.MorningBriefingTime;
        
        // Check if current time is within 5 minutes of briefing time
        var diff = Math.Abs((now - briefingTime).TotalMinutes);
        return diff <= 5;
    }

    /// <summary>
    /// Check if it's time for evening summary
    /// </summary>
    public bool IsEveningSummaryTime()
    {
        if (!_config.EveningSummaryEnabled)
        {
            return false;
        }

        var now = DateTime.Now.TimeOfDay;
        var summaryTime = _config.EveningSummaryTime;
        
        // Check if current time is within 5 minutes of summary time
        var diff = Math.Abs((now - summaryTime).TotalMinutes);
        return diff <= 5;
    }

    /// <summary>
    /// Update briefing configuration
    /// </summary>
    public void UpdateConfig(BriefingConfig config)
    {
        _config.MorningBriefingEnabled = config.MorningBriefingEnabled;
        _config.MorningBriefingTime = config.MorningBriefingTime;
        _config.EveningSummaryEnabled = config.EveningSummaryEnabled;
        _config.EveningSummaryTime = config.EveningSummaryTime;
    }

    /// <summary>
    /// Get current configuration
    /// </summary>
    public BriefingConfig GetConfig()
    {
        return _config;
    }
}
