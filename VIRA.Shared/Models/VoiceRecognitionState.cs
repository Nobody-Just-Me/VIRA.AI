namespace VIRA.Shared.Models;

/// <summary>
/// Represents the current state of voice recognition
/// </summary>
public enum VoiceRecognitionState
{
    /// <summary>
    /// Voice recognition is idle/not active
    /// </summary>
    Idle,
    
    /// <summary>
    /// Listening for user speech
    /// </summary>
    Listening,
    
    /// <summary>
    /// Processing the recognized speech
    /// </summary>
    Processing,
    
    /// <summary>
    /// Waiting for user confirmation of transcribed text
    /// </summary>
    AwaitingConfirmation,
    
    /// <summary>
    /// An error occurred during voice recognition
    /// </summary>
    Error
}

/// <summary>
/// Contains information about a voice recognition error
/// </summary>
public class VoiceRecognitionError
{
    public string Message { get; set; } = string.Empty;
    public VoiceErrorType ErrorType { get; set; }
    public bool CanRetry { get; set; }
}
