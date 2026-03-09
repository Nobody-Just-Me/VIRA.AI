using VIRA.Shared.Models;

namespace VIRA.Shared.Services;

/// <summary>
/// Configuration for task reminders
/// </summary>
public class ReminderConfig
{
    /// <summary>
    /// Enable/disable task reminders
    /// </summary>
    public bool Enabled { get; set; } = true;
    
    /// <summary>
    /// Minutes before due time to send reminder
    /// </summary>
    public int MinutesBeforeDue { get; set; } = 15;
    
    /// <summary>
    /// Enable evening summary of incomplete tasks
    /// </summary>
    public bool EveningSummaryEnabled { get; set; } = true;
    
    /// <summary>
    /// Time for evening summary (24-hour format)
    /// </summary>
    public TimeSpan EveningSummaryTime { get; set; } = new TimeSpan(20, 0, 0); // 8:00 PM
}

/// <summary>
/// Service for managing task reminders and notifications
/// Platform-specific implementation required for actual notifications
/// </summary>
public class ReminderService
{
    private readonly TaskManager _taskManager;
    private readonly ReminderConfig _config;

    public ReminderService(TaskManager taskManager, ReminderConfig? config = null)
    {
        _taskManager = taskManager;
        _config = config ?? new ReminderConfig();
    }

    /// <summary>
    /// Get tasks that need reminders (due within configured time)
    /// </summary>
    public List<ViraTask> GetTasksNeedingReminders()
    {
        if (!_config.Enabled)
        {
            return new List<ViraTask>();
        }

        var now = DateTime.Now;
        var reminderThreshold = now.AddMinutes(_config.MinutesBeforeDue);

        return _taskManager.GetActiveTasks()
            .Where(t => t.DueDate.HasValue && 
                       t.DueDate.Value <= reminderThreshold &&
                       t.DueDate.Value > now)
            .ToList();
    }

    /// <summary>
    /// Get overdue tasks
    /// </summary>
    public List<ViraTask> GetOverdueTasks()
    {
        var now = DateTime.Now;
        
        return _taskManager.GetActiveTasks()
            .Where(t => t.DueDate.HasValue && t.DueDate.Value < now)
            .ToList();
    }

    /// <summary>
    /// Generate evening summary of incomplete tasks
    /// </summary>
    public string GenerateEveningSummary()
    {
        if (!_config.EveningSummaryEnabled)
        {
            return string.Empty;
        }

        var activeTasks = _taskManager.GetActiveTasks();
        var completedToday = _taskManager.GetCompletedTasks()
            .Where(t => t.CompletedAt.HasValue && 
                       t.CompletedAt.Value.Date == DateTime.Today)
            .ToList();

        if (activeTasks.Count == 0 && completedToday.Count == 0)
        {
            return "Tidak ada task hari ini. Selamat beristirahat! 😊";
        }

        var summary = "📊 **Ringkasan Malam Hari**\n\n";
        
        if (completedToday.Count > 0)
        {
            summary += $"✅ Task selesai hari ini: {completedToday.Count}\n";
        }
        
        if (activeTasks.Count > 0)
        {
            summary += $"📋 Task yang belum selesai: {activeTasks.Count}\n";
            
            var highPriority = activeTasks.Where(t => t.Priority == TaskPriority.HIGH).ToList();
            if (highPriority.Count > 0)
            {
                summary += $"🔴 Prioritas tinggi: {highPriority.Count}\n";
            }
            
            var dueTomorrow = activeTasks.Where(t => 
                t.DueDate.HasValue && 
                t.DueDate.Value.Date == DateTime.Today.AddDays(1))
                .ToList();
            
            if (dueTomorrow.Count > 0)
            {
                summary += $"⏰ Deadline besok: {dueTomorrow.Count}\n";
            }
        }

        return summary;
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
    /// Schedule reminder for a specific task
    /// Platform-specific implementation required
    /// </summary>
    public void ScheduleReminder(ViraTask task)
    {
        if (!_config.Enabled || !task.DueDate.HasValue)
        {
            return;
        }

        var reminderTime = task.DueDate.Value.AddMinutes(-_config.MinutesBeforeDue);
        
        if (reminderTime <= DateTime.Now)
        {
            return; // Too late to schedule
        }

        // TODO: Platform-specific implementation
        // On Android: Use WorkManager or AlarmManager
        // On iOS: Use UNUserNotificationCenter
        // On Windows: Use ToastNotification
    }

    /// <summary>
    /// Cancel reminder for a specific task
    /// Platform-specific implementation required
    /// </summary>
    public void CancelReminder(string taskId)
    {
        // TODO: Platform-specific implementation
    }

    /// <summary>
    /// Update reminder configuration
    /// </summary>
    public void UpdateConfig(ReminderConfig config)
    {
        _config.Enabled = config.Enabled;
        _config.MinutesBeforeDue = config.MinutesBeforeDue;
        _config.EveningSummaryEnabled = config.EveningSummaryEnabled;
        _config.EveningSummaryTime = config.EveningSummaryTime;
    }

    /// <summary>
    /// Get current configuration
    /// </summary>
    public ReminderConfig GetConfig()
    {
        return _config;
    }
}
