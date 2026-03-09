using VIRA.Shared.Models;
using VIRA.Shared.Models.Entities;

namespace VIRA.Shared.Repositories;

/// <summary>
/// Repository interface for message persistence
/// </summary>
public interface IMessageRepository
{
    /// <summary>
    /// Save a message
    /// </summary>
    Task<bool> SaveMessageAsync(ChatMessage message);

    /// <summary>
    /// Get all messages
    /// </summary>
    Task<List<ChatMessage>> GetAllMessagesAsync();

    /// <summary>
    /// Get messages with pagination
    /// </summary>
    Task<List<ChatMessage>> GetMessagesAsync(int skip, int take);

    /// <summary>
    /// Get recent messages (last N)
    /// </summary>
    Task<List<ChatMessage>> GetRecentMessagesAsync(int count = 50);

    /// <summary>
    /// Search messages by content
    /// </summary>
    Task<List<ChatMessage>> SearchMessagesAsync(string query);

    /// <summary>
    /// Get messages by date range
    /// </summary>
    Task<List<ChatMessage>> GetMessagesByDateAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Delete a message
    /// </summary>
    Task<bool> DeleteMessageAsync(string id);

    /// <summary>
    /// Clear all messages
    /// </summary>
    Task<bool> ClearAllMessagesAsync();

    /// <summary>
    /// Get message count
    /// </summary>
    Task<int> GetMessageCountAsync();
}

/// <summary>
/// In-memory implementation of message repository
/// Platform-specific implementations should use SQLite or other persistent storage
/// </summary>
public class InMemoryMessageRepository : IMessageRepository
{
    private readonly List<MessageEntity> _messages = new();

    public Task<bool> SaveMessageAsync(ChatMessage message)
    {
        var entity = MessageEntity.FromDomain(message);
        _messages.Add(entity);
        return Task.FromResult(true);
    }

    public Task<List<ChatMessage>> GetAllMessagesAsync()
    {
        return Task.FromResult(_messages
            .OrderBy(m => m.Timestamp)
            .Select(m => m.ToDomain())
            .ToList());
    }

    public Task<List<ChatMessage>> GetMessagesAsync(int skip, int take)
    {
        return Task.FromResult(_messages
            .OrderBy(m => m.Timestamp)
            .Skip(skip)
            .Take(take)
            .Select(m => m.ToDomain())
            .ToList());
    }

    public Task<List<ChatMessage>> GetRecentMessagesAsync(int count = 50)
    {
        return Task.FromResult(_messages
            .OrderByDescending(m => m.Timestamp)
            .Take(count)
            .OrderBy(m => m.Timestamp)
            .Select(m => m.ToDomain())
            .ToList());
    }

    public Task<List<ChatMessage>> SearchMessagesAsync(string query)
    {
        return Task.FromResult(_messages
            .Where(m => m.Content.Contains(query, StringComparison.OrdinalIgnoreCase))
            .OrderBy(m => m.Timestamp)
            .Select(m => m.ToDomain())
            .ToList());
    }

    public Task<List<ChatMessage>> GetMessagesByDateAsync(DateTime startDate, DateTime endDate)
    {
        return Task.FromResult(_messages
            .Where(m => m.Timestamp >= startDate && m.Timestamp <= endDate)
            .OrderBy(m => m.Timestamp)
            .Select(m => m.ToDomain())
            .ToList());
    }

    public Task<bool> DeleteMessageAsync(string id)
    {
        var message = _messages.FirstOrDefault(m => m.Id == id);
        if (message != null)
        {
            _messages.Remove(message);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<bool> ClearAllMessagesAsync()
    {
        _messages.Clear();
        return Task.FromResult(true);
    }

    public Task<int> GetMessageCountAsync()
    {
        return Task.FromResult(_messages.Count);
    }
}
