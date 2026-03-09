namespace VIRA.Mobile.SharedModels;

/// <summary>
/// Context information for processing a conversation message
/// </summary>
public class ConversationContext
{
    /// <summary>
    /// User identifier
    /// </summary>
    public string UserId { get; set; }
    
    /// <summary>
    /// Current session identifier
    /// </summary>
    public string SessionId { get; set; }
    
    /// <summary>
    /// Previous messages in the conversation (for context)
    /// </summary>
    public List<ChatMessage> PreviousMessages { get; set; }
    
    /// <summary>
    /// Current date and time
    /// </summary>
    public DateTime CurrentTime { get; set; }
    
    /// <summary>
    /// User's current location (if available)
    /// </summary>
    public string? Location { get; set; }
    
    /// <summary>
    /// Device state information (battery, network, etc.)
    /// </summary>
    public Dictionary<string, object> DeviceState { get; set; }

    public ConversationContext()
    {
        UserId = "default_user";
        SessionId = Guid.NewGuid().ToString();
        PreviousMessages = new List<ChatMessage>();
        CurrentTime = DateTime.Now;
        DeviceState = new Dictionary<string, object>();
    }
}
