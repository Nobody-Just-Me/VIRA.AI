namespace VIRA.Mobile.SharedModels;

/// <summary>
/// Base class for message processing results
/// Represents the outcome of processing a user message through rule-based or AI-enhanced methods
/// </summary>
public abstract class ProcessingResult
{
    /// <summary>
    /// The response message to display to the user
    /// </summary>
    public string Response { get; set; }
    
    /// <summary>
    /// Optional action to execute (e.g., add task, open app)
    /// </summary>
    public object? Action { get; set; }

    protected ProcessingResult(string response, object? action = null)
    {
        Response = response;
        Action = action;
    }
}

/// <summary>
/// Result from rule-based pattern matching processing
/// </summary>
public class RuleBasedResult : ProcessingResult
{
    /// <summary>
    /// Confidence score of the pattern match (0.0 to 1.0)
    /// </summary>
    public float Confidence { get; set; }

    public RuleBasedResult(string response, object? action = null, float confidence = 1.0f) 
        : base(response, action)
    {
        Confidence = confidence;
    }
}

/// <summary>
/// Result from AI-enhanced processing (Groq, Gemini, OpenAI)
/// </summary>
public class AIEnhancedResult : ProcessingResult
{
    /// <summary>
    /// The AI provider used (Groq, Gemini, OpenAI)
    /// </summary>
    public string Provider { get; set; }

    public AIEnhancedResult(string response, object? action = null, string provider = "Unknown") 
        : base(response, action)
    {
        Provider = provider;
    }
}

/// <summary>
/// Result indicating an error occurred during processing
/// </summary>
public class ErrorResult : ProcessingResult
{
    /// <summary>
    /// Error message describing what went wrong
    /// </summary>
    public string ErrorMessage { get; set; }
    
    /// <summary>
    /// Optional fallback response to show the user
    /// </summary>
    public string? FallbackResponse { get; set; }

    public ErrorResult(string errorMessage, string? fallbackResponse = null) 
        : base(fallbackResponse ?? "Maaf, terjadi kesalahan dalam memproses pesan Anda.", null)
    {
        ErrorMessage = errorMessage;
        FallbackResponse = fallbackResponse;
    }
}
