using VIRA.Mobile.SharedModels;

namespace VIRA.Mobile.SharedServices;

public interface IGeminiService
{
    Task<ChatMessage> SendMessageAsync(string userMessage, List<ChatMessage> conversationHistory);
    Task<bool> TestConnectionAsync();
    void SetApiKey(string apiKey);
}
