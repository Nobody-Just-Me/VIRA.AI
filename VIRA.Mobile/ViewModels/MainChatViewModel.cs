using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Text.Json;
using System.IO;
using VIRA.Mobile.SharedModels;
using VIRA.Mobile.SharedServices;

namespace VIRA.Mobile.ViewModels;

/// <summary>
/// Simplified MainChatViewModel without Uno/MVVM Toolkit dependencies
/// </summary>
public class MainChatViewModel : INotifyPropertyChanged
{
    private readonly IMessageProcessor _messageProcessor;
    private readonly IGeminiService _geminiService;
    private readonly IVoiceService _voiceService;
    private readonly IPreferencesService _preferencesService;
    private readonly string _historyFilePath;

    private string _inputText = string.Empty;
    private bool _isTyping = false;
    private string _processingType = string.Empty;
    private long _lastProcessingLatency = 0;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string InputText
    {
        get => _inputText;
        set
        {
            if (_inputText != value)
            {
                _inputText = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsTyping
    {
        get => _isTyping;
        set
        {
            if (_isTyping != value)
            {
                _isTyping = value;
                OnPropertyChanged();
            }
        }
    }

    public string ProcessingType
    {
        get => _processingType;
        set
        {
            if (_processingType != value)
            {
                _processingType = value;
                OnPropertyChanged();
            }
        }
    }

    public long LastProcessingLatency
    {
        get => _lastProcessingLatency;
        set
        {
            if (_lastProcessingLatency != value)
            {
                _lastProcessingLatency = value;
                OnPropertyChanged();
            }
        }
    }

    public string Greeting => GetGreeting();

    public ObservableCollection<ChatMessage> Messages { get; } = new();

    public ICommand SendMessageCommand { get; }

    public MainChatViewModel(
        IMessageProcessor messageProcessor,
        IGeminiService geminiService,
        IVoiceService voiceService,
        IPreferencesService? preferencesService = null)
    {
        _messageProcessor = messageProcessor;
        _geminiService = geminiService;
        _voiceService = voiceService;
        _preferencesService = preferencesService ?? new PreferencesService();
        
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var viraFolder = Path.Combine(appDataPath, "VIRA");
        if (!Directory.Exists(viraFolder))
        {
            Directory.CreateDirectory(viraFolder);
        }
        _historyFilePath = Path.Combine(viraFolder, "chat_history.json");

        SendMessageCommand = new RelayCommand(async () => await SendMessageAsync());
        
        LoadChatHistory();
    }

    private void LoadChatHistory()
    {
        try
        {
            if (File.Exists(_historyFilePath))
            {
                var json = File.ReadAllText(_historyFilePath);
                var history = JsonSerializer.Deserialize<List<ChatMessage>>(json);
                if (history != null)
                {
                    foreach (var msg in history)
                    {
                        Messages.Add(msg);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load chat history: {ex.Message}");
            // Start fresh if corrupted
            Messages.Clear();
        }
    }

    private async Task SaveChatHistoryAsync()
    {
        if (_preferencesService.GetBoolPreference("privacy_mode_enabled", false))
        {
            return;
        }

        try
        {
            var history = Messages.ToList();
            var json = JsonSerializer.Serialize(history, new JsonSerializerOptions { WriteIndented = true });
            
            // Fire and forget writing to avoid blocking
            _ = Task.Run(() => 
            {
                try
                {
                    File.WriteAllText(_historyFilePath, json);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to save chat history: {ex.Message}");
                }
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to serialize chat history: {ex.Message}");
        }
    }

    private string GetGreeting()
    {
        var hour = DateTime.Now.Hour;
        if (hour < 12) return "Selamat Pagi";
        if (hour < 17) return "Selamat Siang";
        return "Selamat Malam";
    }

    public async Task SendMessageAsync()
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
        
        _ = SaveChatHistoryAsync();

        // Show typing indicator
        IsTyping = true;

        try
        {
            // Send directly to Gemini service
            var response = await _geminiService.SendMessageAsync(userMessage, Messages.ToList());

            // Add response
            Messages.Add(response);
            _ = SaveChatHistoryAsync();
        }
        catch (Exception ex)
        {
            Messages.Add(new ChatMessage
            {
                Role = ChatMessageRole.Assistant,
                Content = $"Maaf, terjadi kesalahan: {ex.Message}",
                Type = MessageType.Text
            });
            _ = SaveChatHistoryAsync();
        }
        finally
        {
            IsTyping = false;
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Simple RelayCommand implementation
    private class RelayCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool>? _canExecute;

        public event EventHandler? CanExecuteChanged;

        public RelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

        public async void Execute(object? parameter) => await _execute();

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
