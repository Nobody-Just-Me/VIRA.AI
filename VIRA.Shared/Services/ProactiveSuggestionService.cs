using VIRA.Shared.Models;

namespace VIRA.Shared.Services;

/// <summary>
/// Configuration for proactive suggestions
/// </summary>
public class ProactiveSuggestionConfig
{
    /// <summary>
    /// Enable/disable proactive suggestions
    /// </summary>
    public bool Enabled { get; set; } = true;
    
    /// <summary>
    /// Minimum interval between suggestions (in minutes)
    /// </summary>
    public int MinIntervalMinutes { get; set; } = 60;
}

/// <summary>
/// Suggestion types
/// </summary>
public enum SuggestionType
{
    TaskReminder,
    BreakReminder,
    ProductivityTip,
    WeatherUpdate,
    NewsUpdate
}

/// <summary>
/// Represents a proactive suggestion
/// </summary>
public class ProactiveSuggestion
{
    public SuggestionType Type { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public Dictionary<string, object>? Data { get; set; }
}

/// <summary>
/// Service for generating proactive suggestions based on user activity and context
/// </summary>
public class ProactiveSuggestionService
{
    private readonly TaskManager _taskManager;
    private readonly TaskAnalyticsService _analyticsService;
    private readonly ProactiveSuggestionConfig _config;
    private DateTime _lastSuggestionTime = DateTime.MinValue;

    public ProactiveSuggestionService(
        TaskManager taskManager,
        TaskAnalyticsService analyticsService,
        ProactiveSuggestionConfig? config = null)
    {
        _taskManager = taskManager;
        _analyticsService = analyticsService;
        _config = config ?? new ProactiveSuggestionConfig();
    }

    /// <summary>
    /// Check if enough time has passed since last suggestion
    /// </summary>
    private bool CanSuggest()
    {
        if (!_config.Enabled)
        {
            return false;
        }

        var timeSinceLastSuggestion = DateTime.Now - _lastSuggestionTime;
        return timeSinceLastSuggestion.TotalMinutes >= _config.MinIntervalMinutes;
    }

    /// <summary>
    /// Generate a proactive suggestion based on current context
    /// </summary>
    public ProactiveSuggestion? GenerateSuggestion()
    {
        if (!CanSuggest())
        {
            return null;
        }

        var now = DateTime.Now;
        var hour = now.Hour;

        // Check for upcoming tasks (within next hour)
        var upcomingTasks = _taskManager.GetActiveTasks()
            .Where(t => t.DueDate.HasValue && 
                       t.DueDate.Value > now &&
                       t.DueDate.Value <= now.AddHours(1))
            .OrderBy(t => t.DueDate)
            .ToList();

        if (upcomingTasks.Count > 0)
        {
            var task = upcomingTasks.First();
            var minutesUntilDue = (task.DueDate!.Value - now).TotalMinutes;
            
            _lastSuggestionTime = now;
            return new ProactiveSuggestion
            {
                Type = SuggestionType.TaskReminder,
                Message = $"⏰ Reminder: Task \"{task.Title}\" akan jatuh tempo dalam {(int)minutesUntilDue} menit.",
                GeneratedAt = now,
                Data = new Dictionary<string, object>
                {
                    ["TaskId"] = task.Id,
                    ["TaskTitle"] = task.Title,
                    ["MinutesUntilDue"] = (int)minutesUntilDue
                }
            };
        }

        // Check for overdue tasks
        var overdueTasks = _taskManager.GetActiveTasks()
            .Where(t => t.DueDate.HasValue && t.DueDate.Value < now)
            .ToList();

        if (overdueTasks.Count > 0)
        {
            _lastSuggestionTime = now;
            return new ProactiveSuggestion
            {
                Type = SuggestionType.TaskReminder,
                Message = $"⚠️ Anda memiliki {overdueTasks.Count} task yang melewati deadline. Ingin saya tampilkan?",
                GeneratedAt = now,
                Data = new Dictionary<string, object>
                {
                    ["OverdueCount"] = overdueTasks.Count
                }
            };
        }

        // Suggest break during productive hours if user has been active
        if ((hour >= 10 && hour <= 11) || (hour >= 14 && hour <= 15))
        {
            var productiveHours = _analyticsService.GetMostProductiveHours();
            if (productiveHours.Contains(hour))
            {
                _lastSuggestionTime = now;
                return new ProactiveSuggestion
                {
                    Type = SuggestionType.BreakReminder,
                    Message = "☕ Anda sudah bekerja cukup lama. Bagaimana kalau istirahat sebentar?",
                    GeneratedAt = now
                };
            }
        }

        // Suggest productivity tips during work hours
        if (hour >= 9 && hour <= 17)
        {
            var tips = new[]
            {
                "💡 Tip: Prioritaskan task dengan deadline terdekat untuk menghindari keterlambatan.",
                "💡 Tip: Pecah task besar menjadi sub-task kecil agar lebih mudah diselesaikan.",
                "💡 Tip: Gunakan prioritas HIGH untuk task yang paling penting hari ini.",
                "💡 Tip: Review task Anda setiap pagi untuk merencanakan hari dengan lebih baik."
            };

            var random = new Random();
            _lastSuggestionTime = now;
            return new ProactiveSuggestion
            {
                Type = SuggestionType.ProductivityTip,
                Message = tips[random.Next(tips.Length)],
                GeneratedAt = now
            };
        }

        return null;
    }

    /// <summary>
    /// Generate suggestion based on specific context
    /// </summary>
    public ProactiveSuggestion? GenerateContextualSuggestion(string context)
    {
        var now = DateTime.Now;

        switch (context.ToLower())
        {
            case "morning":
                return new ProactiveSuggestion
                {
                    Type = SuggestionType.TaskReminder,
                    Message = "🌅 Selamat pagi! Ingin saya tampilkan task untuk hari ini?",
                    GeneratedAt = now
                };

            case "evening":
                var completedToday = _taskManager.GetCompletedTasks()
                    .Count(t => t.CompletedAt.HasValue && 
                               t.CompletedAt.Value.Date == DateTime.Today);
                
                return new ProactiveSuggestion
                {
                    Type = SuggestionType.TaskReminder,
                    Message = $"🌙 Anda telah menyelesaikan {completedToday} task hari ini. Ingin lihat ringkasan?",
                    GeneratedAt = now,
                    Data = new Dictionary<string, object>
                    {
                        ["CompletedCount"] = completedToday
                    }
                };

            case "idle":
                var activeTasks = _taskManager.GetActiveTaskCount();
                if (activeTasks > 0)
                {
                    return new ProactiveSuggestion
                    {
                        Type = SuggestionType.TaskReminder,
                        Message = $"📋 Anda memiliki {activeTasks} task aktif. Ada yang ingin dikerjakan sekarang?",
                        GeneratedAt = now,
                        Data = new Dictionary<string, object>
                        {
                            ["ActiveCount"] = activeTasks
                        }
                    };
                }
                break;

            case "task_completed":
                var remaining = _taskManager.GetActiveTaskCount();
                if (remaining > 0)
                {
                    return new ProactiveSuggestion
                    {
                        Type = SuggestionType.TaskReminder,
                        Message = $"🎉 Bagus! Masih ada {remaining} task lagi. Lanjutkan?",
                        GeneratedAt = now,
                        Data = new Dictionary<string, object>
                        {
                            ["RemainingCount"] = remaining
                        }
                    };
                }
                else
                {
                    return new ProactiveSuggestion
                    {
                        Type = SuggestionType.ProductivityTip,
                        Message = "🎊 Hebat! Semua task sudah selesai. Anda bisa istirahat atau tambah task baru.",
                        GeneratedAt = now
                    };
                }
        }

        return null;
    }

    /// <summary>
    /// Update configuration
    /// </summary>
    public void UpdateConfig(ProactiveSuggestionConfig config)
    {
        _config.Enabled = config.Enabled;
        _config.MinIntervalMinutes = config.MinIntervalMinutes;
    }

    /// <summary>
    /// Get current configuration
    /// </summary>
    public ProactiveSuggestionConfig GetConfig()
    {
        return _config;
    }

    /// <summary>
    /// Reset suggestion timer (useful for testing or manual triggers)
    /// </summary>
    public void ResetTimer()
    {
        _lastSuggestionTime = DateTime.MinValue;
    }
}
