using VIRA.Shared.Models;

namespace VIRA.Shared.Services;

public interface IGeminiService
{
    Task<ChatMessage> SendMessageAsync(string userMessage, List<ChatMessage> conversationHistory);
    Task<bool> TestConnectionAsync();
    void SetApiKey(string apiKey);
}
