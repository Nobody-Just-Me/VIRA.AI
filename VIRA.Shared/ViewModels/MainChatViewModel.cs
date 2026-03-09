using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VIRA.Shared.Models;
using VIRA.Shared.Services;

namespace VIRA.Shared.ViewModels;

public partial class MainChatViewModel : ObservableObject
{
    private readonly IMessageProcessor _messageProcessor;
    private readonly IGeminiService _geminiService;
    private readonly IVoiceService _voiceService;
    private readonly IPreferencesService? _preferencesService;

    [ObservableProperty]
    private string _inputText = string.Empty;

    [ObservableProperty]
    private bool _isTyping = false;
    
    [ObservableProperty]
    private string _processingType = string.Empty;
    
    [ObservableProperty]
    private long _lastProcessingLatency = 0;

    [ObservableProperty]
    private bool _isListening = false;

    [ObservableProperty]
    private bool _isContinuousListening = false;

    [ObservableProperty]
    private VoiceRecognitionState _voiceState = VoiceRecognitionState.Idle;

    [ObservableProperty]
    private string _voiceStateDetail = string.Empty;

    [ObservableProperty]
    private string _transcribedText = string.Empty;

    [ObservableProperty]
    private bool _showTranscriptionConfirmation = false;

    public string Greeting => GetGreeting();

    public ObservableCollection<ChatMessage> Messages { get; } = new();

    public MainChatViewModel(
        IMessageProcessor messageProcessor,
        IGeminiService geminiService, 
        IVoiceService voiceService,
        IPreferencesService? preferencesService = null)
    {
        _messageProcessor = messageProcessor;
        _geminiService = geminiService;
        _voiceService = voiceService;
        _preferencesService = preferencesService;
        
        // Load continuous listening preference
        if (_preferencesService != null)
        {
            _isContinuousListening = _preferencesService.IsContinuousListeningEnabled;
        }
        
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
            // Track processing start time
            var startTime = DateTime.Now;
            
            // Create conversation context
            var context = new ConversationContext
            {
                PreviousMessages = Messages.ToList()
            };
            
            // Process message using hybrid processor
            var result = await _messageProcessor.ProcessMessageAsync(userMessage, context);
            
            // Calculate latency
            var latency = (DateTime.Now - startTime).TotalMilliseconds;
            LastProcessingLatency = (long)latency;
            
            // Handle different result types
            ChatMessage response;
            
            if (result is RuleBasedResult ruleBasedResult)
            {
                ProcessingType = $"Rule-Based ({ruleBasedResult.Confidence:P0})";
                response = new ChatMessage
                {
                    Role = ChatMessageRole.Assistant,
                    Content = ruleBasedResult.Response,
                    Type = MessageType.Text,
                    Timestamp = DateTime.Now,
                    Metadata = new Dictionary<string, object>
                    {
                        ["ProcessingType"] = "RuleBased",
                        ["Confidence"] = ruleBasedResult.Confidence,
                        ["LatencyMs"] = latency
                    }
                };
            }
            else if (result is AIEnhancedResult aiResult)
            {
                ProcessingType = $"AI ({aiResult.Provider})";
                response = new ChatMessage
                {
                    Role = ChatMessageRole.Assistant,
                    Content = aiResult.Response,
                    Type = MessageType.Text,
                    Timestamp = DateTime.Now,
                    Metadata = new Dictionary<string, object>
                    {
                        ["ProcessingType"] = "AIEnhanced",
                        ["Provider"] = aiResult.Provider,
                        ["LatencyMs"] = latency
                    }
                };
            }
            else if (result is ErrorResult errorResult)
            {
                ProcessingType = "Error";
                response = new ChatMessage
                {
                    Role = ChatMessageRole.Assistant,
                    Content = errorResult.Response,
                    Type = MessageType.Text,
                    Timestamp = DateTime.Now,
                    Metadata = new Dictionary<string, object>
                    {
                        ["ProcessingType"] = "Error",
                        ["ErrorMessage"] = errorResult.ErrorMessage,
                        ["LatencyMs"] = latency
                    }
                };
            }
            else
            {
                // Fallback for unknown result type
                ProcessingType = "Unknown";
                response = new ChatMessage
                {
                    Role = ChatMessageRole.Assistant,
                    Content = "Maaf, terjadi kesalahan dalam memproses pesan Anda.",
                    Type = MessageType.Text,
                    Timestamp = DateTime.Now
                };
            }
            
            Messages.Add(response);

            // NOTE: Voice synthesis is now handled by MainActivity using ElevenLabs TTS
            // The old Google TTS call has been removed to prevent conflict
            // MainActivity will call SynthesizeAndPlaySpeechAsync() after getting the response
        }
        catch (Exception ex)
        {
            ProcessingType = "Exception";
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
            VoiceState = VoiceRecognitionState.Error;
            VoiceStateDetail = "Voice feature not available on this device";
            
            Messages.Add(new ChatMessage
            {
                Role = ChatMessageRole.Assistant,
                Content = "Maaf, fitur suara tidak tersedia di perangkat ini.",
                Type = MessageType.Text
            });
            
            await Task.Delay(3000);
            VoiceState = VoiceRecognitionState.Idle;
            return;
        }

        IsListening = true;
        int retryCount = 0;
        const int maxRetries = 2;

        do
        {
            try
            {
                // Update state to listening
                VoiceState = VoiceRecognitionState.Listening;
                VoiceStateDetail = retryCount > 0 
                    ? $"Retry {retryCount}/{maxRetries} - Speak now" 
                    : "Speak now";

                var recognizedText = await _voiceService.RecognizeSpeechAsync();
                
                if (!string.IsNullOrWhiteSpace(recognizedText))
                {
                    // Update state to processing
                    VoiceState = VoiceRecognitionState.Processing;
                    VoiceStateDetail = "Processing speech...";
                    
                    await Task.Delay(500); // Brief delay for visual feedback
                    
                    // Show transcription for confirmation
                    TranscribedText = recognizedText;
                    VoiceState = VoiceRecognitionState.AwaitingConfirmation;
                    VoiceStateDetail = $"Transcribed: \"{recognizedText}\"";
                    ShowTranscriptionConfirmation = true;
                    
                    // Wait for confirmation (handled by UI)
                    // The UI will call ConfirmTranscription or RetryVoiceInput
                    return;
                }
                else
                {
                    // No speech detected
                    retryCount++;
                    
                    if (retryCount <= maxRetries)
                    {
                        VoiceState = VoiceRecognitionState.Error;
                        VoiceStateDetail = "No speech detected. Retrying...";
                        await Task.Delay(1500);
                    }
                    else
                    {
                        VoiceState = VoiceRecognitionState.Error;
                        VoiceStateDetail = "No speech detected after multiple attempts";
                        
                        Messages.Add(new ChatMessage
                        {
                            Role = ChatMessageRole.Assistant,
                            Content = "Maaf, tidak ada suara yang terdeteksi. Silakan coba lagi atau ketik pesan Anda.",
                            Type = MessageType.Text
                        });
                        
                        await Task.Delay(3000);
                        VoiceState = VoiceRecognitionState.Idle;
                        break;
                    }
                }
                
                // If continuous listening is enabled, wait a bit before listening again
                if (IsContinuousListening && IsListening && retryCount == 0)
                {
                    await Task.Delay(500);
                }
            }
            catch (Exception ex)
            {
                retryCount++;
                
                // Determine error type and message
                var errorMessage = GetVoiceErrorMessage(ex);
                
                if (retryCount <= maxRetries)
                {
                    VoiceState = VoiceRecognitionState.Error;
                    VoiceStateDetail = $"{errorMessage} - Retrying...";
                    await Task.Delay(1500);
                }
                else
                {
                    VoiceState = VoiceRecognitionState.Error;
                    VoiceStateDetail = errorMessage;
                    
                    Messages.Add(new ChatMessage
                    {
                        Role = ChatMessageRole.Assistant,
                        Content = $"Maaf, terjadi kesalahan: {errorMessage}. Silakan coba lagi atau ketik pesan Anda.",
                        Type = MessageType.Text
                    });
                    
                    await Task.Delay(3000);
                    VoiceState = VoiceRecognitionState.Idle;
                    break;
                }
            }
        }
        while ((IsContinuousListening && IsListening) || retryCount <= maxRetries);
        
        IsListening = false;
    }

    [RelayCommand]
    private async Task ConfirmTranscriptionAsync(string? text = null)
    {
        ShowTranscriptionConfirmation = false;
        
        var messageText = text ?? TranscribedText;
        
        if (!string.IsNullOrWhiteSpace(messageText))
        {
            InputText = messageText;
            VoiceState = VoiceRecognitionState.Idle;
            await SendMessageAsync();
        }
        
        TranscribedText = string.Empty;
    }

    [RelayCommand]
    private void CancelTranscription()
    {
        ShowTranscriptionConfirmation = false;
        TranscribedText = string.Empty;
        VoiceState = VoiceRecognitionState.Idle;
        IsListening = false;
    }

    [RelayCommand]
    private async Task RetryVoiceInputAsync()
    {
        ShowTranscriptionConfirmation = false;
        TranscribedText = string.Empty;
        await StartVoiceInputAsync();
    }

    private string GetVoiceErrorMessage(Exception ex)
    {
        var message = ex.Message.ToLower();
        
        if (message.Contains("permission") || message.Contains("denied"))
        {
            return "Microphone permission denied";
        }
        else if (message.Contains("network") || message.Contains("connection"))
        {
            return "Network error - check your connection";
        }
        else if (message.Contains("microphone") || message.Contains("audio"))
        {
            return "Microphone unavailable";
        }
        else if (message.Contains("timeout"))
        {
            return "Speech recognition timeout";
        }
        else
        {
            return "Voice recognition failed";
        }
    }

    [RelayCommand]
    private void StopVoiceInput()
    {
        IsListening = false;
    }

    [RelayCommand]
    private void ToggleContinuousListening()
    {
        IsContinuousListening = !IsContinuousListening;
        
        // Save preference
        if (_preferencesService != null)
        {
            _preferencesService.IsContinuousListeningEnabled = IsContinuousListening;
        }
        
        // Show feedback message
        Messages.Add(new ChatMessage
        {
            Role = ChatMessageRole.Assistant,
            Content = IsContinuousListening 
                ? "Mode mendengar berkelanjutan diaktifkan. Saya akan terus mendengarkan setelah setiap respons." 
                : "Mode mendengar berkelanjutan dinonaktifkan.",
            Type = MessageType.Text
        });
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
