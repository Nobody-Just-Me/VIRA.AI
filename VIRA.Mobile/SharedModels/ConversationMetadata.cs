namespace VIRA.Mobile.SharedModels;

/// <summary>
/// Lightweight metadata for conversations without full message history
/// Used for efficient list display and caching
/// </summary>
public class ConversationMetadata
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public long CreatedAt { get; set; }
    public long UpdatedAt { get; set; }
    public int MessageCount { get; set; }
    
    /// <summary>
    /// Gets formatted timestamp for display
    /// </summary>
    public string GetFormattedTimestamp()
    {
        var date = DateTimeOffset.FromUnixTimeMilliseconds(UpdatedAt).LocalDateTime;
        var now = DateTime.Now;
        
        if (date.Date == now.Date)
        {
            return date.ToString("HH:mm");
        }
        else if (date.Date == now.AddDays(-1).Date)
        {
            return "Yesterday";
        }
        else if (date.Year == now.Year)
        {
            return date.ToString("MMM dd");
        }
        else
        {
            return date.ToString("MMM dd, yyyy");
        }
    }
}
