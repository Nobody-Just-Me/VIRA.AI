namespace VIRA.Shared.Models.Entities;

/// <summary>
/// Database entity for chat messages
/// </summary>
public class MessageEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Role { get; set; } = string.Empty; // "User" or "Assistant"
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
    
    // Metadata fields
    public string? ProcessingType { get; set; } // "RuleBased", "AIEnhanced", "Error"
    public float? Confidence { get; set; }
    public long? LatencyMs { get; set; }
    public string? Provider { get; set; } // "Gemini", "Groq", "OpenAI"
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// Convert to domain model
    /// </summary>
    public ChatMessage ToDomain()
    {
        var message = new ChatMessage
        {
            Role = Role == "User" ? ChatMessageRole.User : ChatMessageRole.Assistant,
            Content = Content,
            Type = MessageType.Text,
            Timestamp = Timestamp
        };

        // Add metadata if available
        if (ProcessingType != null || Confidence.HasValue || LatencyMs.HasValue || Provider != null)
        {
            message.Metadata = new Dictionary<string, object>();
            
            if (ProcessingType != null)
                message.Metadata["ProcessingType"] = ProcessingType;
            if (Confidence.HasValue)
                message.Metadata["Confidence"] = Confidence.Value;
            if (LatencyMs.HasValue)
                message.Metadata["LatencyMs"] = LatencyMs.Value;
            if (Provider != null)
                message.Metadata["Provider"] = Provider;
            if (ErrorMessage != null)
                message.Metadata["ErrorMessage"] = ErrorMessage;
        }

        return message;
    }

    /// <summary>
    /// Create from domain model
    /// </summary>
    public static MessageEntity FromDomain(ChatMessage message)
    {
        var entity = new MessageEntity
        {
            Role = message.Role == ChatMessageRole.User ? "User" : "Assistant",
            Content = message.Content,
            Timestamp = message.Timestamp
        };

        // Extract metadata
        if (message.Metadata != null)
        {
            if (message.Metadata.TryGetValue("ProcessingType", out var processingType))
                entity.ProcessingType = processingType?.ToString();
            if (message.Metadata.TryGetValue("Confidence", out var confidence))
                entity.Confidence = Convert.ToSingle(confidence);
            if (message.Metadata.TryGetValue("LatencyMs", out var latency))
                entity.LatencyMs = Convert.ToInt64(latency);
            if (message.Metadata.TryGetValue("Provider", out var provider))
                entity.Provider = provider?.ToString();
            if (message.Metadata.TryGetValue("ErrorMessage", out var error))
                entity.ErrorMessage = error?.ToString();
        }

        return entity;
    }
}
