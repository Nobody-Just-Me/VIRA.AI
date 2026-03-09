using System.Text.RegularExpressions;
using VIRA.Shared.Models;

namespace VIRA.Shared.Services.Handlers;

/// <summary>
/// Handler for deleting tasks
/// Pattern: "hapus|delete|remove task [task description]"
/// </summary>
public class DeleteTaskHandler : ICommandHandler
{
    private readonly TaskManager _taskManager;

    public DeleteTaskHandler(TaskManager taskManager)
    {
        _taskManager = taskManager;
    }

    public async Task<CommandResult> HandleAsync(Match match, ConversationContext context)
    {
        // Extract task description from the match
        // Pattern groups: 1=action (hapus/delete/etc), 2=task keyword, 3=description
        string taskDescription = match.Groups.Count > 3 
            ? match.Groups[3].Value.Trim() 
            : string.Empty;

        if (string.IsNullOrWhiteSpace(taskDescription))
        {
            return new CommandResult(
                response: "Maaf, saya tidak menangkap task mana yang ingin dihapus. Coba lagi dengan format: 'hapus task [deskripsi]'",
                confidence: 0.5f,
                speak: true
            );
        }

        // Try to find and delete task by description
        var deletedTask = _taskManager.DeleteTaskByTitle(taskDescription);
        if (deletedTask != null)
        {
            return await Task.FromResult(new CommandResult(
                response: $"🗑️ Task \"{deletedTask.Title}\" sudah dihapus dari daftar.",
                action: new { Type = "task_deleted", TaskId = deletedTask.Id, TaskTitle = deletedTask.Title },
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
