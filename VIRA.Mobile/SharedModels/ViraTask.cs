namespace VIRA.Mobile.SharedModels;

/// <summary>
/// Represents a task in the VIRA task management system
/// </summary>
public class ViraTask
{
    /// <summary>
    /// Unique identifier for the task
    /// </summary>
    public string Id { get; set; }
    
    /// <summary>
    /// Task title/description
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Whether the task is completed
    /// </summary>
    public bool IsCompleted { get; set; }
    
    /// <summary>
    /// When the task was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// When the task was completed (if applicable)
    /// </summary>
    public DateTime? CompletedAt { get; set; }
    
    /// <summary>
    /// Task priority (HIGH, MEDIUM, LOW)
    /// </summary>
    public TaskPriority Priority { get; set; }
    
    /// <summary>
    /// Optional due date for the task
    /// </summary>
    public DateTime? DueDate { get; set; }

    public ViraTask()
    {
        Id = Guid.NewGuid().ToString();
        IsCompleted = false;
        CreatedAt = DateTime.Now;
        Priority = TaskPriority.MEDIUM;
    }

    public ViraTask(string title, TaskPriority priority = TaskPriority.MEDIUM, DateTime? dueDate = null)
    {
        Id = Guid.NewGuid().ToString();
        Title = title;
        IsCompleted = false;
        CreatedAt = DateTime.Now;
        Priority = priority;
        DueDate = dueDate;
    }
}

/// <summary>
/// Task priority levels
/// </summary>
public enum TaskPriority
{
    LOW,
    MEDIUM,
    HIGH
}
