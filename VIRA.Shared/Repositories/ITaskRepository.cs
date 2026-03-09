using VIRA.Shared.Models;
using VIRA.Shared.Models.Entities;

namespace VIRA.Shared.Repositories;

/// <summary>
/// Repository interface for task persistence
/// </summary>
public interface ITaskRepository
{
    /// <summary>
    /// Save a task
    /// </summary>
    Task<bool> SaveTaskAsync(ViraTask task);

    /// <summary>
    /// Get all tasks
    /// </summary>
    Task<List<ViraTask>> GetAllTasksAsync();

    /// <summary>
    /// Get active tasks
    /// </summary>
    Task<List<ViraTask>> GetActiveTasksAsync();

    /// <summary>
    /// Get completed tasks
    /// </summary>
    Task<List<ViraTask>> GetCompletedTasksAsync();

    /// <summary>
    /// Get tasks by priority
    /// </summary>
    Task<List<ViraTask>> GetTasksByPriorityAsync(TaskPriority priority);

    /// <summary>
    /// Get tasks due today
    /// </summary>
    Task<List<ViraTask>> GetTasksDueTodayAsync();

    /// <summary>
    /// Get overdue tasks
    /// </summary>
    Task<List<ViraTask>> GetOverdueTasksAsync();

    /// <summary>
    /// Get task by ID
    /// </summary>
    Task<ViraTask?> GetTaskByIdAsync(string id);

    /// <summary>
    /// Update a task
    /// </summary>
    Task<bool> UpdateTaskAsync(ViraTask task);

    /// <summary>
    /// Delete a task
    /// </summary>
    Task<bool> DeleteTaskAsync(string id);

    /// <summary>
    /// Clear all tasks
    /// </summary>
    Task<bool> ClearAllTasksAsync();
}

/// <summary>
/// In-memory implementation of task repository
/// </summary>
public class InMemoryTaskRepository : ITaskRepository
{
    private readonly List<TaskEntity> _tasks = new();

    public Task<bool> SaveTaskAsync(ViraTask task)
    {
        var entity = TaskEntity.FromDomain(task);
        _tasks.Add(entity);
        return Task.FromResult(true);
    }

    public Task<List<ViraTask>> GetAllTasksAsync()
    {
        return Task.FromResult(_tasks
            .OrderByDescending(t => t.Priority)
            .ThenBy(t => t.DueDate)
            .Select(t => t.ToDomain())
            .ToList());
    }

    public Task<List<ViraTask>> GetActiveTasksAsync()
    {
        return Task.FromResult(_tasks
            .Where(t => !t.IsCompleted)
            .OrderByDescending(t => t.Priority)
            .ThenBy(t => t.DueDate)
            .Select(t => t.ToDomain())
            .ToList());
    }

    public Task<List<ViraTask>> GetCompletedTasksAsync()
    {
        return Task.FromResult(_tasks
            .Where(t => t.IsCompleted)
            .OrderByDescending(t => t.CompletedAt)
            .Select(t => t.ToDomain())
            .ToList());
    }

    public Task<List<ViraTask>> GetTasksByPriorityAsync(TaskPriority priority)
    {
        return Task.FromResult(_tasks
            .Where(t => t.Priority == (int)priority && !t.IsCompleted)
            .OrderBy(t => t.DueDate)
            .Select(t => t.ToDomain())
            .ToList());
    }

    public Task<List<ViraTask>> GetTasksDueTodayAsync()
    {
        var today = DateTime.Today;
        return Task.FromResult(_tasks
            .Where(t => !t.IsCompleted && t.DueDate.HasValue && t.DueDate.Value.Date == today)
            .OrderByDescending(t => t.Priority)
            .Select(t => t.ToDomain())
            .ToList());
    }

    public Task<List<ViraTask>> GetOverdueTasksAsync()
    {
        var now = DateTime.Now;
        return Task.FromResult(_tasks
            .Where(t => !t.IsCompleted && t.DueDate.HasValue && t.DueDate.Value < now)
            .OrderBy(t => t.DueDate)
            .Select(t => t.ToDomain())
            .ToList());
    }

    public Task<ViraTask?> GetTaskByIdAsync(string id)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);
        return Task.FromResult(task?.ToDomain());
    }

    public Task<bool> UpdateTaskAsync(ViraTask task)
    {
        var existing = _tasks.FirstOrDefault(t => t.Id == task.Id);
        if (existing != null)
        {
            _tasks.Remove(existing);
            _tasks.Add(TaskEntity.FromDomain(task));
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<bool> DeleteTaskAsync(string id)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);
        if (task != null)
        {
            _tasks.Remove(task);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<bool> ClearAllTasksAsync()
    {
        _tasks.Clear();
        return Task.FromResult(true);
    }
}
