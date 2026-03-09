using System.Text.RegularExpressions;
using System.Text;
using VIRA.Shared.Models;

namespace VIRA.Shared.Services.Handlers;

/// <summary>
/// Handler for schedule queries
/// Pattern: "jadwal|schedule|agenda|appointment"
/// </summary>
public class ScheduleHandler : ICommandHandler
{
    private readonly TaskManager _taskManager;

    public ScheduleHandler(TaskManager taskManager)
    {
        _taskManager = taskManager;
    }

    public async Task<CommandResult> HandleAsync(Match match, ConversationContext context)
    {
        // Get today's tasks with due dates
        var todaysTasks = _taskManager.GetActiveTasks()
            .Where(t => t.DueDate.HasValue && t.DueDate.Value.Date == DateTime.Today)
            .OrderBy(t => t.DueDate)
            .ToList();

        if (todaysTasks.Count == 0)
        {
            return new CommandResult(
                response: "📅 Anda tidak memiliki jadwal atau task dengan deadline hari ini. Hari ini bebas!",
                confidence: 1.0f,
                speak: true
            );
        }

        // Build response
        var response = new StringBuilder();
        response.AppendLine($"📅 **Jadwal Hari Ini ({DateTime.Today:dddd, dd MMMM yyyy})**\n");

        foreach (var task in todaysTasks)
        {
            string priorityIcon = task.Priority switch
            {
                TaskPriority.HIGH => "🔴",
                TaskPriority.MEDIUM => "🟡",
                TaskPriority.LOW => "🟢",
                _ => "⚪"
            };

            string timeInfo = task.DueDate.HasValue 
                ? task.DueDate.Value.ToString("HH:mm") 
                : "Sepanjang hari";

            response.AppendLine($"{priorityIcon} **{timeInfo}** - {task.Title}");
        }

        // Spoken summary
        string spokenSummary = $"Anda memiliki {todaysTasks.Count} jadwal hari ini.";

        return await Task.FromResult(new CommandResult(
            response: response.ToString().Trim(),
            action: new { Type = "schedule_query", Count = todaysTasks.Count },
            confidence: 1.0f,
            speak: true
        ));
    }
}
