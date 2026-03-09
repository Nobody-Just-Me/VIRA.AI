namespace VIRA.Mobile.SharedModels;

/// <summary>
/// Result of executing a command handler
/// </summary>
public class CommandResult
{
    /// <summary>
    /// The response message to display to the user
    /// </summary>
    public string Response { get; set; }
    
    /// <summary>
    /// Optional action to execute (e.g., add task, open app)
    /// </summary>
    public object? Action { get; set; }
    
    /// <summary>
    /// Confidence score of the pattern match (0.0 to 1.0)
    /// </summary>
    public float Confidence { get; set; }
    
    /// <summary>
    /// Whether the response should be spoken via TTS
    /// </summary>
    public bool Speak { get; set; }

    public CommandResult(string response, object? action = null, float confidence = 1.0f, bool speak = false)
    {
        Response = response;
        Action = action;
        Confidence = confidence;
        Speak = speak;
    }
}
