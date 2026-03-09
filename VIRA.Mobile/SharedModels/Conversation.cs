using System;
using System.Collections.Generic;
using System.Globalization;

namespace VIRA.Mobile.SharedModels;

/// <summary>
/// Represents a complete chat session with message history
/// </summary>
public class Conversation
{
    public string Id { get; set; }
    public string Title { get; set; }
    public long CreatedAt { get; set; }
    public long UpdatedAt { get; set; }
    public List<ChatMessage> Messages { get; set; }

    public Conversation()
    {
        Id = Guid.NewGuid().ToString();
        Title = string.Empty;
        CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        Messages = new List<ChatMessage>();
    }

    public Conversation(string id, string title, long createdAt, long updatedAt, List<ChatMessage> messages)
    {
        Id = id;
        Title = title;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        Messages = messages ?? new List<ChatMessage>();
    }

    /// <summary>
    /// Gets the display title for the conversation
    /// </summary>
    public string GetDisplayTitle()
    {
        return !string.IsNullOrEmpty(Title) ? Title : "New Conversation";
    }

    /// <summary>
    /// Gets the formatted timestamp for display
    /// </summary>
    public string GetFormattedTimestamp()
    {
        var dateTime = DateTimeOffset.FromUnixTimeMilliseconds(UpdatedAt).LocalDateTime;
        return dateTime.ToString("MMM dd, yyyy HH:mm", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Gets the number of messages in the conversation
    /// </summary>
    public int GetMessageCount()
    {
        return Messages.Count;
    }
}
