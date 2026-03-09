using System.Text.RegularExpressions;
using System.Text;
using VIRA.Shared.Models;

namespace VIRA.Shared.Services.Handlers;

/// <summary>
/// Handler for listing tasks
/// Pattern: "daftar|list|show task"
/// </summary>
public class ListTasksHandler : ICommandHandler
{
    private readonly TaskManager _taskManager;

    public ListTasksHandler(TaskManager taskManager)
    {
        _taskManager = taskManager;
    }

    public async Task<CommandResult> HandleAsync(Match match, ConversationContext context)
    {
        var activeTasks = _taskManager.GetActiveTasks();
        var completedTasks = _taskManager.GetCompletedTasks();

        if (activeTasks.Count == 0 && completedTasks.Count == 0)
        {
            return new CommandResult(
                response: "Anda belum memiliki task. Tambahkan task dengan mengatakan 'tambah task [deskripsi]'.",
                confidence: 1.0f,
                speak: true
            );
        }

        var response = new StringBuilder();

        // List active tasks
        if (activeTasks.Count > 0)
        {
            response.AppendLine($"📋 **Task Aktif ({activeTasks.Count}):**");
            for (int i = 0; i < activeTasks.Count; i++)
            {
                var task = activeTasks[i];
                string priorityIcon = task.Priority switch
                {
                    TaskPriority.HIGH => "🔴",
                    TaskPriority.MEDIUM => "🟡",
                    TaskPriority.LOW => "🟢",
                    _ => "⚪"
                };
                
                string dueDateInfo = task.DueDate.HasValue 
                    ? $" (Due: {task.DueDate.Value:dd/MM/yyyy})" 
                    : "";
                
                response.AppendLine($"{i + 1}. {priorityIcon} {task.Title}{dueDateInfo}");
            }
            response.AppendLine();
        }

        // List completed tasks (limit to 5 most recent)
        if (completedTasks.Count > 0)
        {
            var recentCompleted = completedTasks.Take(5).ToList();
            response.AppendLine($"✅ **Task Selesai ({completedTasks.Count}):**");
            for (int i = 0; i < recentCompleted.Count; i++)
            {
                var task = recentCompleted[i];
                response.AppendLine($"{i + 1}. ~~{task.Title}~~");
            }
            
            if (completedTasks.Count > 5)
            {
                response.AppendLine($"... dan {completedTasks.Count - 5} task lainnya");
            }
        }

        // Create spoken summary
        string spokenSummary = activeTasks.Count > 0
            ? $"Anda memiliki {activeTasks.Count} task aktif."
            : "Semua task sudah selesai!";

        return await Task.FromResult(new CommandResult(
            response: response.ToString().Trim(),
            action: new { Type = "tasks_listed", ActiveCount = activeTasks.Count, CompletedCount = completedTasks.Count },
            confidence: 1.0f,
            speak: false // Don't speak the full list, just show it
        ));
    }
}
