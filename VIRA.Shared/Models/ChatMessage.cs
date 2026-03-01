namespace VIRA.Shared.Models;

public enum ChatMessageRole
{
    User,
    Assistant,
    System
}

public enum MessageType
{
    Text,
    Schedule,
    Weather,
    News,
    Reminder,
    Traffic
}

public class ChatMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Content { get; set; } = string.Empty;
    public ChatMessageRole Role { get; set; }
    public MessageType Type { get; set; } = MessageType.Text;
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public bool IsVoiceGenerated { get; set; }
    
    // Optional structured data
    public List<ScheduleItem>? Schedule { get; set; }
    public WeatherData? Weather { get; set; }
    public List<NewsItem>? NewsItems { get; set; }
    public List<TrafficRoute>? TrafficData { get; set; }
}

public class ScheduleItem
{
    public string Time { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Color { get; set; } = "#256AF4";
}

public class WeatherData
{
    public string City { get; set; } = string.Empty;
    public string Temp { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public string Humidity { get; set; } = string.Empty;
    public string UV { get; set; } = string.Empty;
    public string Tomorrow { get; set; } = string.Empty;
}

public class NewsItem
{
    public string Category { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
}

public class TrafficRoute
{
    public string Route { get; set; } = string.Empty;
    public string ETA { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Color { get; set; } = "#22C55E";
}
