using System.Text.RegularExpressions;
using VIRA.Shared.Models;

namespace VIRA.Shared.Services.Handlers;

/// <summary>
/// Handler for completing tasks
/// Pattern: "selesai|done|complete task [optional: task description]"
/// </summary>
public class CompleteTaskHandler : ICommandHandler
{
    private readonly TaskManager _taskManager;

    public CompleteTaskHandler(TaskManager taskManager)
    {
        _taskManager = taskManager;
    }

    public async Task<CommandResult> HandleAsync(Match match, ConversationContext context)
    {
        // Extract optional task description from the match
        // Pattern groups: 1=action (selesai/done/etc), 2=task keyword, 3=optional description
        string taskDescription = match.Groups.Count > 3 
            ? match.Groups[3].Value.Trim() 
            : string.Empty;

        // If no specific task mentioned, try to complete the most recent active task
        if (string.IsNullOrWhiteSpace(taskDescription))
        {
            var activeTasks = _taskManager.GetActiveTasks();
            if (activeTasks.Count == 0)
            {
                return new CommandResult(
                    response: "Tidak ada task aktif yang bisa diselesaikan.",
                    confidence: 1.0f,
                    speak: true
                );
            }

            // Complete the most recent task
            var task = activeTasks.First();
            _taskManager.CompleteTask(task.Id);

            return await Task.FromResult(new CommandResult(
                response: $"✅ Task \"{task.Title}\" sudah ditandai selesai. Bagus!",
                action: new { Type = "task_completed", TaskId = task.Id, TaskTitle = task.Title },
                confidence: 1.0f,
                speak: true
            ));
        }

        // Try to find and complete task by description
        var completedTask = _taskManager.CompleteTaskByTitle(taskDescription);
        if (completedTask != null)
        {
            return await Task.FromResult(new CommandResult(
                response: $"✅ Task \"{completedTask.Title}\" sudah ditandai selesai. Bagus!",
                action: new { Type = "task_completed", TaskId = completedTask.Id, TaskTitle = completedTask.Title },
                confidence: 1.0f,
                speak: true
            ));
        }

        // Task not found
        return new CommandResult(
            response: $"Maaf, saya tidak menemukan task dengan deskripsi \"{taskDescription}\".",
            confidence: 0.7f,
            speak: true
        );
    }
}
