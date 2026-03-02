using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VIRA.Shared.Models;
using VIRA.Shared.Services;

namespace VIRA.Shared.ViewModels;

public partial class MainChatViewModel : ObservableObject
{
    private readonly IGeminiService _geminiService;
    private readonly IVoiceService _voiceService;

    [ObservableProperty]
    private string _inputText = string.Empty;

    [ObservableProperty]
    private bool _isTyping = false;

    [ObservableProperty]
    private bool _isListening = false;

    public string Greeting => GetGreeting();

    public ObservableCollection<ChatMessage> Messages { get; } = new();

    public MainChatViewModel(IGeminiService geminiService, IVoiceService voiceService)
    {
        _geminiService = geminiService;
        _voiceService = voiceService;
        
        // Don't add welcome message here - let MainActivity handle it
        // This prevents duplicate or conflicting welcome messages
        // InitializeWelcomeMessage();
    }

    private void InitializeWelcomeMessage()
    {
        var greeting = GetGreeting();
        Messages.Add(new ChatMessage
        {
            Role = ChatMessageRole.Assistant,
            Content = $"{greeting}! ✨ Saya Vira, asisten pribadi AI Anda. Saya siap membantu Anda hari ini. Ada yang bisa saya bantu?",
            Type = MessageType.Text,
            Timestamp = DateTime.Now
        });
    }

    private string GetGreeting()
    {
        var hour = DateTime.Now.Hour;
        if (hour < 12) return "Selamat Pagi";
        if (hour < 17) return "Selamat Siang";
        return "Selamat Malam";
    }

    [RelayCommand]
    private async Task SendMessageAsync()
    {
        if (string.IsNullOrWhiteSpace(InputText))
            return;

        var userMessage = InputText.Trim();
        InputText = string.Empty;

        // Add user message
        Messages.Add(new ChatMessage
        {
            Role = ChatMessageRole.User,
            Content = userMessage,
            Type = MessageType.Text
        });

        // Show typing indicator
        IsTyping = true;

        try
        {
            // Simulate typing delay for natural feel
            await Task.Delay(Random.Shared.Next(800, 1500));

            // Get AI response
            var response = await _geminiService.SendMessageAsync(userMessage, Messages.ToList());
            
            Messages.Add(response);

            // NOTE: Voice synthesis is now handled by MainActivity using ElevenLabs TTS
            // The old Google TTS call has been removed to prevent conflict
            // MainActivity will call SynthesizeAndPlaySpeechAsync() after getting the response
        }
        catch (Exception ex)
        {
            Messages.Add(new ChatMessage
            {
                Role = ChatMessageRole.Assistant,
                Content = $"Maaf, terjadi kesalahan: {ex.Message}",
                Type = MessageType.Text
            });
        }
        finally
        {
            IsTyping = false;
        }
    }

    [RelayCommand]
    private async Task StartVoiceInputAsync()
    {
        if (!_voiceService.IsAvailable)
        {
            Messages.Add(new ChatMessage
            {
                Role = ChatMessageRole.Assistant,
                Content = "Maaf, fitur suara tidak tersedia di perangkat ini.",
                Type = MessageType.Text
            });
            return;
        }

        IsListening = true;

        try
        {
            var recognizedText = await _voiceService.RecognizeSpeechAsync();
            
            if (!string.IsNullOrWhiteSpace(recognizedText))
            {
                InputText = recognizedText;
                await SendMessageAsync();
            }
        }
        catch (Exception ex)
        {
            Messages.Add(new ChatMessage
            {
                Role = ChatMessageRole.Assistant,
                Content = $"Maaf, tidak dapat mengenali suara: {ex.Message}",
                Type = MessageType.Text
            });
        }
        finally
        {
            IsListening = false;
        }
    }

    [RelayCommand]
    private async Task ExecuteQuickActionAsync(string query)
    {
        InputText = query;
        await SendMessageAsync();
    }

    [RelayCommand]
    private void ClearChat()
    {
        Messages.Clear();
        InitializeWelcomeMessage();
    }
}
