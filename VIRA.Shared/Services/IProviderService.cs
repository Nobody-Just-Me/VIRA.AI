using VIRA.Shared.Models;

namespace VIRA.Shared.Services;

/// <summary>
/// Common interface for all AI provider implementations (Groq, OpenAI, Gemini).
/// Provides standardized methods for message sending, connection validation, and model discovery.
/// </summary>
public interface IProviderService
{
    /// <summary>
    /// Gets the name of the provider (e.g., "Groq", "OpenAI", "Gemini").
    /// </summary>
    string ProviderName { get; }
    
    /// <summary>
    /// Gets whether the provider is properly configured with a valid API key.
    /// </summary>
    bool IsConfigured { get; }
    
    /// <summary>
    /// Validates the connection to the provider's API.
    /// </summary>
    /// <returns>True if the connection is valid and the API key works; otherwise, false.</returns>
    Task<bool> ValidateConnectionAsync();
    
    /// <summary>
    /// Sends a message to the AI provider and returns the response.
    /// </summary>
    /// <param name="message">The user's message to send.</param>
    /// <param name="history">The conversation history for context.</param>
    /// <returns>A ChatMessage containing the AI's response.</returns>
    Task<ChatMessage> SendMessageAsync(string message, List<ChatMessage> history);
    
    /// <summary>
    /// Sends a message to the AI provider and returns a streaming response.
    /// </summary>
    /// <param name="message">The user's message to send.</param>
    /// <param name="history">The conversation history for context.</param>
    /// <returns>A Stream containing the AI's streaming response.</returns>
    Task<Stream> SendMessageStreamAsync(string message, List<ChatMessage> history);
    
    /// <summary>
    /// Gets the list of available models for this provider.
    /// </summary>
    /// <returns>A list of model names/identifiers available for this provider.</returns>
    List<string> GetAvailableModels();
}
