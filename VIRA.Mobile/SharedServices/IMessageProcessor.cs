using VIRA.Mobile.SharedModels;

namespace VIRA.Mobile.SharedServices;

/// <summary>
/// Interface for processing user messages with hybrid rule-based and AI-enhanced strategies
/// </summary>
public interface IMessageProcessor
{
    /// <summary>
    /// Process a user message and return the appropriate response
    /// </summary>
    /// <param name="message">The user's input message</param>
    /// <param name="context">Conversation context including history and device state</param>
    /// <returns>Processing result with response and action</returns>
    Task<ProcessingResult> ProcessMessageAsync(string message, ConversationContext context);
}
