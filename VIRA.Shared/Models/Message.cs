namespace VIRA.Shared.Models;

/// <summary>
/// Represents a chat message with rich content support
/// Note: This is an alias for ChatMessage to maintain compatibility with new UI components
/// </summary>
public class Message
{
    public int Id { get; set; }
    public MessageRole Role { get; set; }
    public string Text { get; set; } = string.Empty;
    public MessageType Type { get; set; }
    public DateTime Timestamp { get; set; }
    
    // Rich content - using existing model classes from ChatMessage.cs
    public List<ScheduleItem>? Schedule { get; set; }
    public WeatherData? Weather { get; set; }
    public List<NewsItem>? NewsItems { get; set; }
    public List<TrafficRoute>? TrafficData { get; set; }
}

/// <summary>
/// Message role (User or AI)
/// </summary>
public enum MessageRole
{
    User,
    AI
}
