using System.Net.Http.Json;
using System.Text.Json;
using VIRA.Shared.Models;

namespace VIRA.Shared.Services;

/// <summary>
/// Gemini AI provider implementation with multimodal support.
/// Implements IProviderService interface for multi-provider support.
/// </summary>
public class GeminiProvider : IProviderService
{
    private readonly HttpClient _httpClient;
    private readonly ISecureStorageManager _storageManager;
    private string _apiKey = string.Empty;
    private string _selectedModel = "gemini-2.0-flash";
    
    private const string BaseUrlTemplate = "https://generativelanguage.googleapis.com/v1beta/models/{0}:generateContent?key={1}";
    
    // Available Gemini models
    private static readonly List<string> AvailableModels = new()
    {
        "gemini-2.0-flash",
        "gemini-pro"
    };

    public string ProviderName => "Gemini";

    public bool IsConfigured => !string.IsNullOrEmpty(_apiKey);

    public GeminiProvider(HttpClient httpClient, ISecureStorageManager storageManager)
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
        var apiKey = await _storageManager.GetApiKeyAsync("Gemini");
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
        await _storageManager.SaveApiKeyAsync("Gemini", apiKey);
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
    /// Validates the connection to Gemini API
    /// </summary>
    public async Task<bool> ValidateConnectionAsync()
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
            return false;
        }

        try
        {
            var url = string.Format(BaseUrlTemplate, _selectedModel, _apiKey);
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = "Hello" }
                        }
                    }
                }
            };
            
            var response = await _httpClient.PostAsJsonAsync(url, requestBody);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Sends a message to Gemini API and returns the response
    /// </summary>
    public async Task<ChatMessage> SendMessageAsync(string message, List<ChatMessage> history)
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
            throw new InvalidOperationException("Gemini API Key has not been configured. Please set your API key in Settings.");
        }

        try
        {
            var url = string.Format(BaseUrlTemplate, _selectedModel, _apiKey);
            var contents = BuildContents(history, message);

            var requestBody = new
            {
                contents,
                generationConfig = new
                {
                    temperature = 0.9,
                    maxOutputTokens = 1024,
                    topP = 0.95
                }
            };

            _httpClient.DefaultRequestHeaders.Clear();
            
            var response = await _httpClient.PostAsJsonAsync(url, requestBody);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Gemini API error: {response.StatusCode} - {error}");
            }

            var result = await response.Content.ReadFromJsonAsync<JsonElement>();
            var text = result
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
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
            throw new Exception($"Failed to send message to Gemini: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Sends a message to Gemini API and returns a streaming response
    /// Note: Gemini supports streaming through streamGenerateContent endpoint
    /// </summary>
    public async Task<Stream> SendMessageStreamAsync(string message, List<ChatMessage> history)
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
            throw new InvalidOperationException("Gemini API Key has not been configured. Please set your API key in Settings.");
        }

        try
        {
            // Use streaming endpoint
            var streamUrl = string.Format(
                "https://generativelanguage.googleapis.com/v1beta/models/{0}:streamGenerateContent?key={1}",
                _selectedModel,
                _apiKey
            );
            
            var contents = BuildContents(history, message);

            var requestBody = new
            {
                contents,
                generationConfig = new
                {
                    temperature = 0.9,
                    maxOutputTokens = 1024,
                    topP = 0.95
                }
            };

            _httpClient.DefaultRequestHeaders.Clear();
            
            var response = await _httpClient.PostAsJsonAsync(streamUrl, requestBody);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Gemini API error: {response.StatusCode} - {error}");
            }

            return await response.Content.ReadAsStreamAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to stream message from Gemini: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Gets the list of available models for Gemini
    /// </summary>
    public List<string> GetAvailableModels()
    {
        return new List<string>(AvailableModels);
    }

    /// <summary>
    /// Builds the contents array for the Gemini API request
    /// Gemini uses a different format than OpenAI
    /// </summary>
    private List<object> BuildContents(List<ChatMessage> history, string userMessage)
    {
        var contents = new List<object>();

        // Add system prompt as first user message (Gemini doesn't have system role)
        contents.Add(new
        {
            role = "user",
            parts = new[]
            {
                new { text = GetSystemPrompt() }
            }
        });

        // Add a model response acknowledging the system prompt
        contents.Add(new
        {
            role = "model",
            parts = new[]
            {
                new { text = "Understood. I'm Vira, your AI assistant. How can I help you today?" }
            }
        });

        // Include last 10 messages for context
        foreach (var msg in history.TakeLast(10))
        {
            var role = msg.Role == ChatMessageRole.User ? "user" : "model";
            contents.Add(new
            {
                role,
                parts = new[]
                {
                    new { text = msg.Content }
                }
            });
        }

        // Add current user message
        contents.Add(new
        {
            role = "user",
            parts = new[]
            {
                new { text = userMessage }
            }
        });

        return contents;
    }

    /// <summary>
    /// Gets the system prompt for Gemini
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
