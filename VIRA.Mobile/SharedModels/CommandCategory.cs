namespace VIRA.Mobile.SharedModels;

/// <summary>
/// Categories for command patterns to organize different types of user commands
/// </summary>
public enum CommandCategory
{
    /// <summary>
    /// Task management commands (add, complete, list, delete tasks)
    /// </summary>
    TASK_MANAGEMENT,
    
    /// <summary>
    /// Information query commands (weather, news, schedule, time, battery)
    /// </summary>
    INFORMATION_QUERY,
    
    /// <summary>
    /// Android system integration commands (open apps, send messages, make calls)
    /// </summary>
    ANDROID_INTEGRATION,
    
    /// <summary>
    /// Greeting and conversation commands (hello, goodbye, thank you)
    /// </summary>
    GREETING,
    
    /// <summary>
    /// System control commands (WiFi, Bluetooth, flashlight, volume, brightness)
    /// </summary>
    SYSTEM_CONTROL,
    
    /// <summary>
    /// Media control commands (play, pause, next, previous)
    /// </summary>
    MEDIA_CONTROL,
    
    /// <summary>
    /// Contact management commands (find contacts, send WhatsApp, make calls)
    /// </summary>
    CONTACT_MANAGEMENT
}
