namespace VIRA.Shared.Services;

/// <summary>
/// Constants for message processing configuration
/// </summary>
public static class ProcessingConstants
{
    /// <summary>
    /// Confidence threshold for rule-based processing.
    /// If rule-based confidence is above this threshold, use rule-based result.
    /// Otherwise, fall back to AI-enhanced processing.
    /// </summary>
    public const float CONFIDENCE_THRESHOLD = 0.8f;
    
    /// <summary>
    /// Maximum number of previous messages to include in conversation context
    /// </summary>
    public const int MAX_CONTEXT_MESSAGES = 5;
    
    /// <summary>
    /// Default timeout for AI API calls in milliseconds
    /// </summary>
    public const int AI_TIMEOUT_MS = 30000; // 30 seconds
}
