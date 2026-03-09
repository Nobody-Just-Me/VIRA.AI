using System.Text.RegularExpressions;
using VIRA.Shared.Models;

namespace VIRA.Shared.Services.Handlers;

/// <summary>
/// Handler for adding new tasks
/// Pattern: "tambah|add|buat|create task [task description]"
/// </summary>
public class AddTaskHandler : ICommandHandler
{
    private readonly TaskManager _taskManager;
    private readonly TaskAnalyticsService? _analyticsService;

    public AddTaskHandler(TaskManager taskManager, TaskAnalyticsService? analyticsService = null)
    {
        _taskManager = taskManager;
        _analyticsService = analyticsService;
    }

    public async Task<CommandResult> HandleAsync(Match match, ConversationContext context)
    {
        // Extract task description from the match
        // Pattern groups: 1=action (tambah/add/etc), 2=task keyword, 3=description
        string fullDescription = match.Groups.Count > 3 
            ? match.Groups[3].Value.Trim() 
            : string.Empty;

        if (string.IsNullOrWhiteSpace(fullDescription))
        {
            return new CommandResult(
                response: "Maaf, saya tidak menangkap deskripsi task-nya. Coba lagi dengan format: 'tambah task [deskripsi]'",
                confidence: 0.5f,
                speak: true
            );
        }

        // Extract priority from description
        var priority = ExtractPriority(ref fullDescription);
        
        // Extract due date from description
        var dueDate = ExtractDueDate(ref fullDescription);

        // Add the task with priority and due date
        var task = _taskManager.AddTask(fullDescription, priority, dueDate);

        // Create response with priority and due date info
        string priorityText = priority switch
        {
            TaskPriority.HIGH => " dengan prioritas tinggi",
            TaskPriority.LOW => " dengan prioritas rendah",
            _ => ""
        };
        
        string dueDateText = dueDate.HasValue 
            ? $" (deadline: {dueDate.Value:dd/MM/yyyy})" 
            : "";

        string response = $"✅ Baik, saya sudah menambahkan task \"{task.Title}\"{priorityText}{dueDateText} ke daftar Anda.";
        
        // Add time suggestion if no due date was specified and analytics service is available
        if (!dueDate.HasValue && _analyticsService != null)
        {
            var suggestedTime = _analyticsService.SuggestTaskTime(priority);
            response += $"\n\n💡 Saran: Berdasarkan pola produktivitas Anda, waktu optimal untuk task ini adalah {suggestedTime:dddd, dd MMMM HH:mm}.";
        }

        return await Task.FromResult(new CommandResult(
            response: response,
            action: new { Type = "task_added", TaskId = task.Id, TaskTitle = task.Title, Priority = priority, DueDate = dueDate },
            confidence: 1.0f,
            speak: true
        ));
    }

    /// <summary>
    /// Extract priority from task description
    /// Removes priority keywords from description
    /// </summary>
    private TaskPriority ExtractPriority(ref string description)
    {
        var priority = TaskPriority.MEDIUM; // Default
        
        // Check for high priority keywords
        var highPriorityPattern = @"\b(penting|urgent|prioritas tinggi|high priority|segera|asap)\b";
        if (Regex.IsMatch(description, highPriorityPattern, RegexOptions.IgnoreCase))
        {
            priority = TaskPriority.HIGH;
            description = Regex.Replace(description, highPriorityPattern, "", RegexOptions.IgnoreCase).Trim();
        }
        // Check for low priority keywords
        else if (Regex.IsMatch(description, @"\b(tidak penting|low priority|prioritas rendah|nanti|kapan-kapan)\b", RegexOptions.IgnoreCase))
        {
            priority = TaskPriority.LOW;
            description = Regex.Replace(description, @"\b(tidak penting|low priority|prioritas rendah|nanti|kapan-kapan)\b", "", RegexOptions.IgnoreCase).Trim();
        }
        
        return priority;
    }

    /// <summary>
    /// Extract due date from task description
    /// Removes date keywords from description
    /// </summary>
    private DateTime? ExtractDueDate(ref string description)
    {
        DateTime? dueDate = null;
        
        // Check for "hari ini" (today)
        if (Regex.IsMatch(description, @"\b(hari ini|today)\b", RegexOptions.IgnoreCase))
        {
            dueDate = DateTime.Today;
            description = Regex.Replace(description, @"\b(hari ini|today)\b", "", RegexOptions.IgnoreCase).Trim();
        }
        // Check for "besok" (tomorrow)
        else if (Regex.IsMatch(description, @"\b(besok|tomorrow)\b", RegexOptions.IgnoreCase))
        {
            dueDate = DateTime.Today.AddDays(1);
            description = Regex.Replace(description, @"\b(besok|tomorrow)\b", "", RegexOptions.IgnoreCase).Trim();
        }
        // Check for "minggu depan" (next week)
        else if (Regex.IsMatch(description, @"\b(minggu depan|next week)\b", RegexOptions.IgnoreCase))
        {
            dueDate = DateTime.Today.AddDays(7);
            description = Regex.Replace(description, @"\b(minggu depan|next week)\b", "", RegexOptions.IgnoreCase).Trim();
        }
        // Check for specific date format: DD/MM/YYYY or DD-MM-YYYY
        var dateMatch = Regex.Match(description, @"\b(\d{1,2})[/-](\d{1,2})[/-](\d{4})\b");
        if (dateMatch.Success)
        {
            if (int.TryParse(dateMatch.Groups[1].Value, out int day) &&
                int.TryParse(dateMatch.Groups[2].Value, out int month) &&
                int.TryParse(dateMatch.Groups[3].Value, out int year))
            {
                try
                {
                    dueDate = new DateTime(year, month, day);
                    description = description.Replace(dateMatch.Value, "").Trim();
                }
                catch
                {
                    // Invalid date, ignore
                }
            }
        }
        
        return dueDate;
    }
}
