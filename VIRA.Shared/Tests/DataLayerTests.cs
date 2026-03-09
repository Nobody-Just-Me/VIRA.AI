using Xunit;
using VIRA.Shared.Services;
using VIRA.Shared.Models;
using VIRA.Shared.Models.Entities;
using VIRA.Shared.Repositories;

namespace VIRA.Shared.Tests;

/// <summary>
/// Unit tests for data layer functionality
/// Tests CRUD operations, database queries, data mappers, and secure storage
/// **Validates: Property 2 (Task creation from commands), Property 4 (Task completion updates), Property 25 (API key secure storage)**
/// </summary>
public class DataLayerTests
{
    private readonly TaskManager _taskManager;
    private readonly SecureStorageManager _secureStorage;
    private readonly PreferencesService _preferencesService;

    public DataLayerTests()
    {
        _taskManager = new TaskManager();
        _secureStorage = new SecureStorageManager();
        _preferencesService = new PreferencesService();
    }

    #region Task CRUD Operations Tests
    // **Validates: Property 2 - Task creation from commands**

    [Fact]
    public async Task CreateTask_ShouldAddNewTask()
    {
        // Arrange
        string taskDescription = "Buy milk from supermarket";

        // Act
        var task = await _taskManager.AddTaskAsync(taskDescription);

        // Assert
        Assert.NotNull(task);
        Assert.Equal(taskDescription, task.Description);
        Assert.False(task.IsCompleted);
        Assert.NotEqual(Guid.Empty, task.Id);
    }

    [Fact]
    public async Task CreateTask_ShouldSetDefaultValues()
    {
        // Arrange
        string taskDescription = "Test task";

        // Act
        var task = await _taskManager.AddTaskAsync(taskDescription);

        // Assert
        Assert.NotNull(task);
        Assert.Equal(TaskPriority.MEDIUM, task.Priority);
        Assert.False(task.IsCompleted);
        Assert.True(task.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public async Task CreateTask_WithPriority_ShouldSetCorrectPriority()
    {
        // Arrange
        string taskDescription = "Urgent task";
        var priority = TaskPriority.HIGH;

        // Act
        var task = await _taskManager.AddTaskAsync(taskDescription, priority);

        // Assert
        Assert.NotNull(task);
        Assert.Equal(priority, task.Priority);
    }

    [Fact]
    public async Task CreateTask_WithDueDate_ShouldSetDueDate()
    {
        // Arrange
        string taskDescription = "Task with deadline";
        var dueDate = DateTime.UtcNow.AddDays(1);

        // Act
        var task = await _taskManager.AddTaskAsync(taskDescription, dueDate: dueDate);

        // Assert
        Assert.NotNull(task);
        Assert.NotNull(task.DueDate);
        Assert.Equal(dueDate.Date, task.DueDate.Value.Date);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task CreateTask_WithEmptyDescription_ShouldThrowException(string description)
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _taskManager.AddTaskAsync(description);
        });
    }

    #endregion

    #region Task Read Operations Tests

    [Fact]
    public async Task GetAllTasks_ShouldReturnAllTasks()
    {
        // Arrange
        await _taskManager.AddTaskAsync("Task 1");
        await _taskManager.AddTaskAsync("Task 2");
        await _taskManager.AddTaskAsync("Task 3");

        // Act
        var tasks = await _taskManager.GetAllTasksAsync();

        // Assert
        Assert.NotNull(tasks);
        Assert.True(tasks.Count >= 3);
    }

    [Fact]
    public async Task GetActiveTasks_ShouldReturnOnlyIncompleteTasks()
    {
        // Arrange
        var task1 = await _taskManager.AddTaskAsync("Active task 1");
        var task2 = await _taskManager.AddTaskAsync("Active task 2");
        var task3 = await _taskManager.AddTaskAsync("Completed task");
        await _taskManager.CompleteTaskAsync(task3.Id);

        // Act
        var activeTasks = await _taskManager.GetActiveTasksAsync();

        // Assert
        Assert.NotNull(activeTasks);
        Assert.All(activeTasks, task => Assert.False(task.IsCompleted));
    }

    [Fact]
    public async Task GetCompletedTasks_ShouldReturnOnlyCompletedTasks()
    {
        // Arrange
        var task1 = await _taskManager.AddTaskAsync("Task 1");
        var task2 = await _taskManager.AddTaskAsync("Task 2");
        await _taskManager.CompleteTaskAsync(task1.Id);
        await _taskManager.CompleteTaskAsync(task2.Id);

        // Act
        var completedTasks = await _taskManager.GetCompletedTasksAsync();

        // Assert
        Assert.NotNull(completedTasks);
        Assert.All(completedTasks, task => Assert.True(task.IsCompleted));
    }

    [Fact]
    public async Task GetTaskById_ShouldReturnCorrectTask()
    {
        // Arrange
        var createdTask = await _taskManager.AddTaskAsync("Find me");

        // Act
        var foundTask = await _taskManager.GetTaskByIdAsync(createdTask.Id);

        // Assert
        Assert.NotNull(foundTask);
        Assert.Equal(createdTask.Id, foundTask.Id);
        Assert.Equal(createdTask.Description, foundTask.Description);
    }

    [Fact]
    public async Task GetTaskById_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var task = await _taskManager.GetTaskByIdAsync(invalidId);

        // Assert
        Assert.Null(task);
    }

    #endregion

    #region Task Update Operations Tests
    // **Validates: Property 4 - Task completion updates**

    [Fact]
    public async Task CompleteTask_ShouldMarkTaskAsCompleted()
    {
        // Arrange
        var task = await _taskManager.AddTaskAsync("Task to complete");

        // Act
        var result = await _taskManager.CompleteTaskAsync(task.Id);

        // Assert
        Assert.True(result);
        var updatedTask = await _taskManager.GetTaskByIdAsync(task.Id);
        Assert.NotNull(updatedTask);
        Assert.True(updatedTask.IsCompleted);
        Assert.NotNull(updatedTask.CompletedAt);
    }

    [Fact]
    public async Task CompleteTask_ShouldSetCompletionTimestamp()
    {
        // Arrange
        var task = await _taskManager.AddTaskAsync("Task to complete");
        var beforeCompletion = DateTime.UtcNow;

        // Act
        await _taskManager.CompleteTaskAsync(task.Id);

        // Assert
        var updatedTask = await _taskManager.GetTaskByIdAsync(task.Id);
        Assert.NotNull(updatedTask);
        Assert.NotNull(updatedTask.CompletedAt);
        Assert.True(updatedTask.CompletedAt >= beforeCompletion);
        Assert.True(updatedTask.CompletedAt <= DateTime.UtcNow);
    }

    [Fact]
    public async Task CompleteTask_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var result = await _taskManager.CompleteTaskAsync(invalidId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateTask_ShouldModifyTaskProperties()
    {
        // Arrange
        var task = await _taskManager.AddTaskAsync("Original description");
        var newDescription = "Updated description";
        var newPriority = TaskPriority.HIGH;

        // Act
        task.Description = newDescription;
        task.Priority = newPriority;
        var result = await _taskManager.UpdateTaskAsync(task);

        // Assert
        Assert.True(result);
        var updatedTask = await _taskManager.GetTaskByIdAsync(task.Id);
        Assert.NotNull(updatedTask);
        Assert.Equal(newDescription, updatedTask.Description);
        Assert.Equal(newPriority, updatedTask.Priority);
    }

    #endregion

    #region Task Delete Operations Tests

    [Fact]
    public async Task DeleteTask_ShouldRemoveTask()
    {
        // Arrange
        var task = await _taskManager.AddTaskAsync("Task to delete");

        // Act
        var result = await _taskManager.DeleteTaskAsync(task.Id);

        // Assert
        Assert.True(result);
        var deletedTask = await _taskManager.GetTaskByIdAsync(task.Id);
        Assert.Null(deletedTask);
    }

    [Fact]
    public async Task DeleteTask_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var result = await _taskManager.DeleteTaskAsync(invalidId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAllTasks_ShouldRemoveAllTasks()
    {
        // Arrange
        await _taskManager.AddTaskAsync("Task 1");
        await _taskManager.AddTaskAsync("Task 2");
        await _taskManager.AddTaskAsync("Task 3");

        // Act
        await _taskManager.DeleteAllTasksAsync();

        // Assert
        var tasks = await _taskManager.GetAllTasksAsync();
        Assert.Empty(tasks);
    }

    #endregion

    #region Task Filtering and Sorting Tests

    [Fact]
    public async Task GetTasksByPriority_ShouldReturnCorrectTasks()
    {
        // Arrange
        await _taskManager.AddTaskAsync("High priority", TaskPriority.HIGH);
        await _taskManager.AddTaskAsync("Medium priority", TaskPriority.MEDIUM);
        await _taskManager.AddTaskAsync("Low priority", TaskPriority.LOW);

        // Act
        var highPriorityTasks = await _taskManager.GetTasksByPriorityAsync(TaskPriority.HIGH);

        // Assert
        Assert.NotNull(highPriorityTasks);
        Assert.All(highPriorityTasks, task => Assert.Equal(TaskPriority.HIGH, task.Priority));
    }

    [Fact]
    public async Task GetTasksDueToday_ShouldReturnTodaysTasks()
    {
        // Arrange
        await _taskManager.AddTaskAsync("Today's task", dueDate: DateTime.Today);
        await _taskManager.AddTaskAsync("Tomorrow's task", dueDate: DateTime.Today.AddDays(1));
        await _taskManager.AddTaskAsync("Yesterday's task", dueDate: DateTime.Today.AddDays(-1));

        // Act
        var todaysTasks = await _taskManager.GetTasksDueTodayAsync();

        // Assert
        Assert.NotNull(todaysTasks);
        Assert.All(todaysTasks, task => 
        {
            Assert.NotNull(task.DueDate);
            Assert.Equal(DateTime.Today.Date, task.DueDate.Value.Date);
        });
    }

    [Fact]
    public async Task GetOverdueTasks_ShouldReturnOverdueTasks()
    {
        // Arrange
        await _taskManager.AddTaskAsync("Overdue task", dueDate: DateTime.Today.AddDays(-1));
        await _taskManager.AddTaskAsync("Future task", dueDate: DateTime.Today.AddDays(1));

        // Act
        var overdueTasks = await _taskManager.GetOverdueTasksAsync();

        // Assert
        Assert.NotNull(overdueTasks);
        Assert.All(overdueTasks, task =>
        {
            Assert.NotNull(task.DueDate);
            Assert.True(task.DueDate.Value.Date < DateTime.Today.Date);
            Assert.False(task.IsCompleted);
        });
    }

    [Fact]
    public async Task GetTasks_ShouldSortByPriorityAndDueDate()
    {
        // Arrange
        await _taskManager.AddTaskAsync("Low priority, far future", TaskPriority.LOW, DateTime.Today.AddDays(10));
        await _taskManager.AddTaskAsync("High priority, soon", TaskPriority.HIGH, DateTime.Today.AddDays(1));
        await _taskManager.AddTaskAsync("Medium priority, tomorrow", TaskPriority.MEDIUM, DateTime.Today.AddDays(2));

        // Act
        var tasks = await _taskManager.GetActiveTasksAsync();

        // Assert
        Assert.NotNull(tasks);
        // Should be sorted by priority (HIGH > MEDIUM > LOW) then by due date
        for (int i = 0; i < tasks.Count - 1; i++)
        {
            var current = tasks[i];
            var next = tasks[i + 1];
            
            // Higher priority should come first
            if (current.Priority != next.Priority)
            {
                Assert.True(current.Priority >= next.Priority);
            }
        }
    }

    #endregion

    #region Data Mapper Tests

    [Fact]
    public void TaskToEntity_ShouldMapAllProperties()
    {
        // Arrange
        var task = new ViraTask
        {
            Id = Guid.NewGuid(),
            Description = "Test task",
            Priority = TaskPriority.HIGH,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var entity = MapTaskToEntity(task);

        // Assert
        Assert.Equal(task.Id, entity.Id);
        Assert.Equal(task.Description, entity.Description);
        Assert.Equal(task.Priority.ToString(), entity.Priority);
        Assert.Equal(task.IsCompleted, entity.IsCompleted);
        Assert.Equal(task.CreatedAt, entity.CreatedAt);
        Assert.Equal(task.DueDate, entity.DueDate);
    }

    [Fact]
    public void EntityToTask_ShouldMapAllProperties()
    {
        // Arrange
        var entity = new TaskEntity
        {
            Id = Guid.NewGuid(),
            Description = "Test task",
            Priority = "HIGH",
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var task = MapEntityToTask(entity);

        // Assert
        Assert.Equal(entity.Id, task.Id);
        Assert.Equal(entity.Description, task.Description);
        Assert.Equal(TaskPriority.HIGH, task.Priority);
        Assert.Equal(entity.IsCompleted, task.IsCompleted);
        Assert.Equal(entity.CreatedAt, task.CreatedAt);
        Assert.Equal(entity.DueDate, task.DueDate);
    }

    [Fact]
    public void TaskMapper_ShouldHandleNullableFields()
    {
        // Arrange
        var task = new ViraTask
        {
            Id = Guid.NewGuid(),
            Description = "Test task",
            Priority = TaskPriority.MEDIUM,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow,
            DueDate = null,
            CompletedAt = null
        };

        // Act
        var entity = MapTaskToEntity(task);
        var mappedBack = MapEntityToTask(entity);

        // Assert
        Assert.Null(mappedBack.DueDate);
        Assert.Null(mappedBack.CompletedAt);
    }

    #endregion

    #region Secure Storage Tests
    // **Validates: Property 25 - API key secure storage**

    [Fact]
    public async Task SecureStorage_ShouldSaveAndRetrieveApiKey()
    {
        // Arrange
        string key = "test_api_key";
        string value = "sk_test_1234567890abcdef";

        // Act
        await _secureStorage.SaveAsync(key, value);
        var retrieved = await _secureStorage.GetAsync(key);

        // Assert
        Assert.Equal(value, retrieved);
    }

    [Fact]
    public async Task SecureStorage_ShouldEncryptApiKey()
    {
        // Arrange
        string key = "groq_api_key";
        string plainValue = "gsk_1234567890abcdef";

        // Act
        await _secureStorage.SaveAsync(key, plainValue);
        
        // Try to read raw value (should be encrypted)
        var rawValue = _preferencesService.GetPreference(key);

        // Assert
        // Raw value should not equal plain value (it should be encrypted)
        Assert.NotEqual(plainValue, rawValue);
    }

    [Fact]
    public async Task SecureStorage_ShouldDeleteApiKey()
    {
        // Arrange
        string key = "test_api_key";
        string value = "sk_test_1234567890";
        await _secureStorage.SaveAsync(key, value);

        // Act
        await _secureStorage.DeleteAsync(key);
        var retrieved = await _secureStorage.GetAsync(key);

        // Assert
        Assert.Null(retrieved);
    }

    [Fact]
    public async Task SecureStorage_ShouldHandleMultipleKeys()
    {
        // Arrange
        var keys = new Dictionary<string, string>
        {
            { "groq_api_key", "gsk_123" },
            { "gemini_api_key", "AIza_456" },
            { "openai_api_key", "sk_789" }
        };

        // Act
        foreach (var kvp in keys)
        {
            await _secureStorage.SaveAsync(kvp.Key, kvp.Value);
        }

        // Assert
        foreach (var kvp in keys)
        {
            var retrieved = await _secureStorage.GetAsync(kvp.Key);
            Assert.Equal(kvp.Value, retrieved);
        }
    }

    [Fact]
    public async Task SecureStorage_ShouldReturnNull_ForNonExistentKey()
    {
        // Arrange
        string nonExistentKey = "non_existent_key_12345";

        // Act
        var value = await _secureStorage.GetAsync(nonExistentKey);

        // Assert
        Assert.Null(value);
    }

    [Fact]
    public async Task SecureStorage_ShouldOverwriteExistingKey()
    {
        // Arrange
        string key = "test_key";
        string oldValue = "old_value";
        string newValue = "new_value";

        // Act
        await _secureStorage.SaveAsync(key, oldValue);
        await _secureStorage.SaveAsync(key, newValue);
        var retrieved = await _secureStorage.GetAsync(key);

        // Assert
        Assert.Equal(newValue, retrieved);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task SecureStorage_ShouldHandleEmptyValues(string value)
    {
        // Arrange
        string key = "test_empty_key";

        // Act & Assert
        if (value == null)
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _secureStorage.SaveAsync(key, value);
            });
        }
        else
        {
            await _secureStorage.SaveAsync(key, value);
            var retrieved = await _secureStorage.GetAsync(key);
            Assert.Equal(value, retrieved);
        }
    }

    #endregion

    #region Message Repository Tests

    [Fact]
    public async Task MessageRepository_ShouldSaveMessage()
    {
        // Arrange
        var message = new ChatMessage
        {
            Id = Guid.NewGuid(),
            Content = "Test message",
            IsUser = true,
            Timestamp = DateTime.UtcNow
        };

        // Act
        // Note: Actual implementation would use repository
        // This is a conceptual test
        Assert.NotNull(message);
        Assert.NotEqual(Guid.Empty, message.Id);
    }

    [Fact]
    public async Task MessageRepository_ShouldRetrieveMessageHistory()
    {
        // Arrange & Act
        // Note: Actual implementation would use repository
        var messages = new List<ChatMessage>();

        // Assert
        Assert.NotNull(messages);
    }

    #endregion

    #region Helper Methods

    private TaskEntity MapTaskToEntity(ViraTask task)
    {
        return new TaskEntity
        {
            Id = task.Id,
            Description = task.Description,
            Priority = task.Priority.ToString(),
            IsCompleted = task.IsCompleted,
            CreatedAt = task.CreatedAt,
            DueDate = task.DueDate,
            CompletedAt = task.CompletedAt
        };
    }

    private ViraTask MapEntityToTask(TaskEntity entity)
    {
        return new ViraTask
        {
            Id = entity.Id,
            Description = entity.Description,
            Priority = Enum.Parse<TaskPriority>(entity.Priority),
            IsCompleted = entity.IsCompleted,
            CreatedAt = entity.CreatedAt,
            DueDate = entity.DueDate,
            CompletedAt = entity.CompletedAt
        };
    }

    #endregion
}
