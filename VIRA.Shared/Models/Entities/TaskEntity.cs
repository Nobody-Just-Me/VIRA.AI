namespace VIRA.Shared.Models.Entities;

/// <summary>
/// Database entity for tasks
/// </summary>
public class TaskEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? CompletedAt { get; set; }
    public int Priority { get; set; } // 0=LOW, 1=MEDIUM, 2=HIGH
    public DateTime? DueDate { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }

    /// <summary>
    /// Convert to domain model
    /// </summary>
    public ViraTask ToDomain()
    {
        return new ViraTask
        {
            Id = Id,
            Title = Title,
            IsCompleted = IsCompleted,
            CreatedAt = CreatedAt,
            CompletedAt = CompletedAt,
            Priority = (TaskPriority)Priority,
            DueDate = DueDate
        };
    }

    /// <summary>
    /// Create from domain model
    /// </summary>
    public static TaskEntity FromDomain(ViraTask task)
    {
        return new TaskEntity
        {
            Id = task.Id,
            Title = task.Title,
            IsCompleted = task.IsCompleted,
            CreatedAt = task.CreatedAt,
            CompletedAt = task.CompletedAt,
            Priority = (int)task.Priority,
            DueDate = task.DueDate
        };
    }
}
