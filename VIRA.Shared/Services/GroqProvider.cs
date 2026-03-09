using System.Net.Http.Json;
using System.Text.Json;
using VIRA.Shared.Models;

namespace VIRA.Shared.Services;

/// <summary>
/// Groq AI provider implementation for fast inference with low latency.
/// Implements IProviderService interface for multi-provider support.
/// </summary>
public class GroqProvider : IProviderService
{
    private readonly HttpClient _httpClient;
    private readonly ISecureStorageManager _storageManager;
    private string _apiKey = string.Empty;
    private string _selectedModel = "mixtral-8x7b-32768";
    
    private const string BaseUrl = "https://api.groq.com/openai/v1/chat/completions";
    private const string ModelsUrl = "https://api.groq.com/openai/v1/models";
    
    // Available Groq models optimized for different use cases
    private static readonly List<string> AvailableModels = new()
    {
        "mixtral-8x7b-32768",      // Fast, balanced performance
        "llama-3.3-70b-versatile", // High quality, versatile
        "llama-3.1-8b-instant",    // Ultra-fast responses
        "gemma2-9b-it"             // Efficient, good for chat
    };

    public string ProviderName => "Groq";

    public bool IsConfigured => !string.IsNullOrEmpty(_apiKey);

    public GroqProvider(HttpClient httpClient, ISecureStorageManager storageManager)
    {
        _httpClient = httpClient;
        _storageManager = storageManager;
        
        // Load API key from secure storage
        LoadApiKeyAsync().Wait();
    }

    /// <summary>
    /// Load API key from secure storage
    /// </summary>
    private async Task LoadApiKeyAsync()
    {
        var apiKey = await _storageManager.GetApiKeyAsync("Groq");
        if (!string.IsNullOrEmpty(apiKey))
        {
            _apiKey = apiKey;
        }
    }

    /// <summary>
    /// Set API key and save to secure storage
    /// </summary>
    public async Task SetApiKeyAsync(string apiKey)
    {
        _apiKey = apiKey;
        await _storageManager.SaveApiKeyAsync("Groq", apiKey);
    }

    /// <summary>
    /// Set the selected model
    /// </summary>
    public void SetModel(string model)
    {
        if (AvailableModels.Contains(model))
        {
            _selectedModel = model;
        }
    }

    /// <summary>
    /// Validates the connection to Groq API
    /// </summary>
    public async Task<bool> ValidateConnectionAsync()
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
            return false;
        }

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, ModelsUrl);
            request.Headers.Add("Authorization", $"Bearer {_apiKey}");
            
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Sends a message to Groq API and returns the response
    /// </summary>
    public async Task<ChatMessage> SendMessageAsync(string message, List<ChatMessage> history)
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
            throw new InvalidOperationException("Groq API Key has not been configured. Please set your API key in Settings.");
        }

        try
        {
            var messages = BuildMessages(history, message);

            var requestBody = new
            {
                model = _selectedModel,
                messages,
                temperature = 0.9,
                max_tokens = 1024,
                top_p = 0.95
            };

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
            
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, requestBody);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Groq API error: {response.StatusCode} - {error}");
            }

            var result = await response.Content.ReadFromJsonAsync<JsonElement>();
            var text = result
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "Sorry, I couldn't process your request.";

            return new ChatMessage
            {
                Role = ChatMessageRole.Assistant,
                Content = text,
                Type = MessageType.Text,
                Timestamp = DateTime.Now
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to send message to Groq: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Sends a message to Groq API and returns a streaming response
    /// Note: Groq supports streaming but this is a basic implementation
    /// </summary>
    public async Task<Stream> SendMessageStreamAsync(string message, List<ChatMessage> history)
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
            throw new InvalidOperationException("Groq API Key has not been configured. Please set your API key in Settings.");
        }

        try
        {
            var messages = BuildMessages(history, message);

            var requestBody = new
            {
                model = _selectedModel,
                messages,
                temperature = 0.9,
                max_tokens = 1024,
                top_p = 0.95,
                stream = true
            };

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
            
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, requestBody);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Groq API error: {response.StatusCode} - {error}");
            }

            return await response.Content.ReadAsStreamAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to stream message from Groq: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Gets the list of available models for Groq
    /// </summary>
    public List<string> GetAvailableModels()
    {
        return new List<string>(AvailableModels);
    }

    /// <summary>
    /// Builds the message array for the API request
    /// </summary>
    private List<object> BuildMessages(List<ChatMessage> history, string userMessage)
    {
        var messages = new List<object>
        {
            new { role = "system", content = GetSystemPrompt() }
        };

        // Include last 10 messages for context
        foreach (var msg in history.TakeLast(10))
        {
            var role = msg.Role == ChatMessageRole.User ? "user" : "assistant";
            messages.Add(new { role, content = msg.Content });
        }

        messages.Add(new { role = "user", content = userMessage });

        return messages;
    }

    /// <summary>
    /// Gets the system prompt for Groq
    /// </summary>
    private string GetSystemPrompt()
    {
        return @"You are Vira, an intelligent, proactive, and elegant AI personal assistant.

IDENTITY:
- You speak naturally and friendly
- You help users with their daily routines
- You provide concise yet informative answers
- You are proactive in giving useful suggestions

CAPABILITIES:
- Answer general questions
- Provide weather information
- Display schedules and reminders
- Search for current news
- Check traffic conditions
- Give suggestions and recommendations

COMMUNICATION STYLE:
- Friendly and warm
- Professional but not stiff
- Use emojis wisely to add warmth
- Short and to-the-point answers

Current time: " + DateTime.Now.ToString("dd MMMM yyyy, HH:mm");
    }
}
