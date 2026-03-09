using VIRA.Mobile.SharedModels;

namespace VIRA.Mobile.SharedServices;

/// <summary>
/// Service for managing tasks (CRUD operations)
/// </summary>
public class TaskManager
{
    private readonly List<ViraTask> _tasks;

    public TaskManager()
    {
        _tasks = new List<ViraTask>();
    }

    /// <summary>
    /// Add a new task
    /// </summary>
    public ViraTask AddTask(string title, TaskPriority priority = TaskPriority.MEDIUM, DateTime? dueDate = null)
    {
        var task = new ViraTask(title, priority, dueDate);
        _tasks.Add(task);
        return task;
    }

    /// <summary>
    /// Get all tasks
    /// </summary>
    public List<ViraTask> GetAllTasks()
    {
        return _tasks.OrderByDescending(t => t.Priority)
                     .ThenBy(t => t.DueDate)
                     .ThenByDescending(t => t.CreatedAt)
                     .ToList();
    }

    /// <summary>
    /// Get active (incomplete) tasks
    /// </summary>
    public List<ViraTask> GetActiveTasks()
    {
        return _tasks.Where(t => !t.IsCompleted)
                     .OrderByDescending(t => t.Priority)
                     .ThenBy(t => t.DueDate)
                     .ThenByDescending(t => t.CreatedAt)
                     .ToList();
    }

    /// <summary>
    /// Get completed tasks
    /// </summary>
    public List<ViraTask> GetCompletedTasks()
    {
        return _tasks.Where(t => t.IsCompleted)
                     .OrderByDescending(t => t.CompletedAt)
                     .ToList();
    }

    /// <summary>
    /// Mark a task as completed
    /// </summary>
    public bool CompleteTask(string taskId)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == taskId);
        if (task != null && !task.IsCompleted)
        {
            task.IsCompleted = true;
            task.CompletedAt = DateTime.Now;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Complete task by title (fuzzy match)
    /// </summary>
    public ViraTask? CompleteTaskByTitle(string title)
    {
        var task = _tasks.FirstOrDefault(t => 
            !t.IsCompleted && 
            t.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
        
        if (task != null)
        {
            task.IsCompleted = true;
            task.CompletedAt = DateTime.Now;
            return task;
        }
        return null;
    }

    /// <summary>
    /// Delete a task
    /// </summary>
    public bool DeleteTask(string taskId)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == taskId);
        if (task != null)
        {
            _tasks.Remove(task);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Delete task by title (fuzzy match)
    /// </summary>
    public ViraTask? DeleteTaskByTitle(string title)
    {
        var task = _tasks.FirstOrDefault(t => 
            t.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
        
        if (task != null)
        {
            _tasks.Remove(task);
            return task;
        }
        return null;
    }

    /// <summary>
    /// Get task count
    /// </summary>
    public int GetTaskCount()
    {
        return _tasks.Count;
    }

    /// <summary>
    /// Get active task count
    /// </summary>
    public int GetActiveTaskCount()
    {
        return _tasks.Count(t => !t.IsCompleted);
    }

    /// <summary>
    /// Clear all tasks (useful for testing)
    /// </summary>
    public void ClearAllTasks()
    {
        _tasks.Clear();
    }
}
