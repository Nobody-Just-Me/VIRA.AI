using VIRA.Shared.Models;

namespace VIRA.Shared.Services;

/// <summary>
/// Service for analyzing task completion patterns and providing suggestions
/// </summary>
public class TaskAnalyticsService
{
    private readonly TaskManager _taskManager;

    public TaskAnalyticsService(TaskManager taskManager)
    {
        _taskManager = taskManager;
    }

    /// <summary>
    /// Analyze user's most productive hours based on task completion times
    /// </summary>
    public List<int> GetMostProductiveHours()
    {
        var completedTasks = _taskManager.GetCompletedTasks()
            .Where(t => t.CompletedAt.HasValue)
            .ToList();

        if (completedTasks.Count == 0)
        {
            // Default productive hours if no data
            return new List<int> { 9, 10, 14, 15 };
        }

        // Group by hour and count completions
        var hourCounts = completedTasks
            .GroupBy(t => t.CompletedAt!.Value.Hour)
            .Select(g => new { Hour = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(4)
            .Select(x => x.Hour)
            .ToList();

        return hourCounts;
    }

    /// <summary>
    /// Suggest optimal time for a task based on priority and user patterns
    /// </summary>
    public DateTime SuggestTaskTime(TaskPriority priority)
    {
        var productiveHours = GetMostProductiveHours();
        var now = DateTime.Now;

        // For high priority tasks, suggest earliest productive hour today or tomorrow
        if (priority == TaskPriority.HIGH)
        {
            var nextProductiveHour = productiveHours
                .Where(h => h > now.Hour)
                .OrderBy(h => h)
                .FirstOrDefault();

            if (nextProductiveHour > 0)
            {
                return new DateTime(now.Year, now.Month, now.Day, nextProductiveHour, 0, 0);
            }
            else
            {
                // Tomorrow at first productive hour
                var tomorrow = now.AddDays(1);
                return new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, productiveHours.First(), 0, 0);
            }
        }
        // For medium priority, suggest within next 2-3 days
        else if (priority == TaskPriority.MEDIUM)
        {
            var daysAhead = 2;
            var targetDate = now.AddDays(daysAhead);
            var suggestedHour = productiveHours.Skip(1).FirstOrDefault();
            if (suggestedHour == 0) suggestedHour = productiveHours.First();
            
            return new DateTime(targetDate.Year, targetDate.Month, targetDate.Day, suggestedHour, 0, 0);
        }
        // For low priority, suggest next week
        else
        {
            var nextWeek = now.AddDays(7);
            var suggestedHour = productiveHours.Last();
            
            return new DateTime(nextWeek.Year, nextWeek.Month, nextWeek.Day, suggestedHour, 0, 0);
        }
    }

    /// <summary>
    /// Get average task completion time by priority
    /// </summary>
    public Dictionary<TaskPriority, TimeSpan> GetAverageCompletionTime()
    {
        var completedTasks = _taskManager.GetCompletedTasks()
            .Where(t => t.CompletedAt.HasValue)
            .ToList();

        var result = new Dictionary<TaskPriority, TimeSpan>();

        foreach (TaskPriority priority in Enum.GetValues(typeof(TaskPriority)))
        {
            var tasksWithPriority = completedTasks
                .Where(t => t.Priority == priority)
                .ToList();

            if (tasksWithPriority.Count > 0)
            {
                var avgTicks = tasksWithPriority
                    .Average(t => (t.CompletedAt!.Value - t.CreatedAt).Ticks);
                
                result[priority] = TimeSpan.FromTicks((long)avgTicks);
            }
            else
            {
                // Default estimates if no data
                result[priority] = priority switch
                {
                    TaskPriority.HIGH => TimeSpan.FromHours(2),
                    TaskPriority.MEDIUM => TimeSpan.FromHours(6),
                    TaskPriority.LOW => TimeSpan.FromDays(2),
                    _ => TimeSpan.FromHours(4)
                };
            }
        }

        return result;
    }

    /// <summary>
    /// Get task completion rate by day of week
    /// </summary>
    public Dictionary<DayOfWeek, double> GetCompletionRateByDay()
    {
        var completedTasks = _taskManager.GetCompletedTasks()
            .Where(t => t.CompletedAt.HasValue)
            .ToList();

        var allTasks = _taskManager.GetAllTasks();

        var result = new Dictionary<DayOfWeek, double>();

        foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
        {
            var completedOnDay = completedTasks
                .Count(t => t.CompletedAt!.Value.DayOfWeek == day);
            
            var createdOnDay = allTasks
                .Count(t => t.CreatedAt.DayOfWeek == day);

            if (createdOnDay > 0)
            {
                result[day] = (double)completedOnDay / createdOnDay;
            }
            else
            {
                result[day] = 0.0;
            }
        }

        return result;
    }

    /// <summary>
    /// Generate task scheduling suggestion message
    /// </summary>
    public string GenerateSuggestionMessage(string taskTitle, TaskPriority priority)
    {
        var suggestedTime = SuggestTaskTime(priority);
        var avgCompletionTime = GetAverageCompletionTime()[priority];

        var priorityText = priority switch
        {
            TaskPriority.HIGH => "prioritas tinggi",
            TaskPriority.MEDIUM => "prioritas sedang",
            TaskPriority.LOW => "prioritas rendah",
            _ => ""
        };

        var message = $"💡 Saran untuk task \"{taskTitle}\" ({priorityText}):\n\n";
        message += $"⏰ Waktu optimal: {suggestedTime:dddd, dd MMMM yyyy HH:mm}\n";
        message += $"⌛ Estimasi waktu penyelesaian: {FormatTimeSpan(avgCompletionTime)}\n\n";
        message += "Saran ini berdasarkan pola produktivitas Anda.";

        return message;
    }

    /// <summary>
    /// Format TimeSpan to human-readable string
    /// </summary>
    private string FormatTimeSpan(TimeSpan timeSpan)
    {
        if (timeSpan.TotalDays >= 1)
        {
            return $"{(int)timeSpan.TotalDays} hari";
        }
        else if (timeSpan.TotalHours >= 1)
        {
            return $"{(int)timeSpan.TotalHours} jam";
        }
        else
        {
            return $"{(int)timeSpan.TotalMinutes} menit";
        }
    }

    /// <summary>
    /// Get productivity insights
    /// </summary>
    public string GetProductivityInsights()
    {
        var productiveHours = GetMostProductiveHours();
        var completionRates = GetCompletionRateByDay();
        var bestDay = completionRates.OrderByDescending(kvp => kvp.Value).First();

        var insights = "📊 **Insight Produktivitas Anda:**\n\n";
        insights += $"🕐 Jam paling produktif: {string.Join(", ", productiveHours.Select(h => $"{h}:00"))}\n";
        insights += $"📅 Hari paling produktif: {GetDayName(bestDay.Key)} ({bestDay.Value:P0} completion rate)\n";
        
        var completedCount = _taskManager.GetCompletedTasks().Count;
        var activeCount = _taskManager.GetActiveTaskCount();
        
        if (completedCount > 0)
        {
            var completionRate = (double)completedCount / (completedCount + activeCount);
            insights += $"✅ Task completion rate: {completionRate:P0}\n";
        }

        return insights;
    }

    /// <summary>
    /// Get Indonesian day name
    /// </summary>
    private string GetDayName(DayOfWeek day)
    {
        return day switch
        {
            DayOfWeek.Monday => "Senin",
            DayOfWeek.Tuesday => "Selasa",
            DayOfWeek.Wednesday => "Rabu",
            DayOfWeek.Thursday => "Kamis",
            DayOfWeek.Friday => "Jumat",
            DayOfWeek.Saturday => "Sabtu",
            DayOfWeek.Sunday => "Minggu",
            _ => day.ToString()
        };
    }
}
