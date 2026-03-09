using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Collections.ObjectModel;
using VIRA.Shared.Models;
using VIRA.Shared.Services;
using XamlButton = Microsoft.UI.Xaml.Controls.Button;

namespace VIRA.Shared.Views;

/// <summary>
/// Main chat interface with modern UI design
/// </summary>
public sealed partial class MainChatView : Page
{
    private readonly AIProviderManager _providerManager;
    private readonly ObservableCollection<Message> _messages;
    private readonly ObservableCollection<Models.QuickAction> _quickActions;

    public ObservableCollection<Message> Messages => _messages;
    public ObservableCollection<Models.QuickAction> QuickActions => _quickActions;

    public string Greeting => GetGreeting();

    public MainChatView()
    {
        this.InitializeComponent();
        
        _messages = new ObservableCollection<Message>();
        _quickActions = new ObservableCollection<Models.QuickAction>();
        
        // Initialize provider manager (will be injected in production)
        var storageManager = new SecureStorageManager();
        var httpClient = new HttpClient();
        _providerManager = new AIProviderManager(storageManager);
        
        // Register providers
        _providerManager.RegisterProvider(new GroqProvider(httpClient, storageManager));
        _providerManager.RegisterProvider(new OpenAIProvider(httpClient, storageManager));
        _providerManager.RegisterProvider(new GeminiProvider(httpClient, storageManager));
        
        LoadQuickActions();
    }

    /// <summary>
    /// Returns time-based greeting
    /// </summary>
    private string GetGreeting()
    {
        var hour = DateTime.Now.Hour;
        
        if (hour >= 5 && hour < 12)
        {
            return "Good Morning";
        }
        else if (hour >= 12 && hour < 17)
        {
            return "Good Afternoon";
        }
        else
        {
            return "Good Evening";
        }
    }

    private void LoadQuickActions()
    {
        _quickActions.Add(new Models.QuickAction 
        { 
            Icon = "☀️", 
            Label = "Weather", 
            Color = "#eab308", 
            Query = "What's the weather today?" 
        });
        _quickActions.Add(new Models.QuickAction 
        { 
            Icon = "📰", 
            Label = "News", 
            Color = "#2094f3", 
            Query = "Show me today's news" 
        });
        _quickActions.Add(new Models.QuickAction 
        { 
            Icon = "🔔", 
            Label = "Reminders", 
            Color = "#ef4444", 
            Query = "Show my reminders" 
        });
        _quickActions.Add(new Models.QuickAction 
        { 
            Icon = "🚗", 
            Label = "Traffic", 
            Color = "#22c55e", 
            Query = "Check traffic conditions" 
        });
        _quickActions.Add(new Models.QuickAction 
        { 
            Icon = "☕", 
            Label = "Coffee", 
            Color = "#d97706", 
            Query = "Order my usual coffee" 
        });
        _quickActions.Add(new Models.QuickAction 
        { 
            Icon = "🎵", 
            Label = "Music", 
            Color = "#a855f7", 
            Query = "Play some focus music" 
        });
    }

    private void OnMenuButtonClick(object sender, RoutedEventArgs e)
    {
        // Open chat history sidebar
        _ = ChatSidebar.ShowAsync();
    }

    private void OnSettingsButtonClick(object sender, RoutedEventArgs e)
    {
        // Navigate to settings page
        Frame.Navigate(typeof(SettingsView));
    }
    
    private async void OnChatSelected(object sender, ChatSession session)
    {
        // Load the selected conversation
        _messages.Clear();
        foreach (var message in session.Messages)
        {
            _messages.Add(message);
        }
        
        // Close the sidebar
        await ChatSidebar.HideAsync();
        
        // Scroll to bottom
        await Task.Delay(100);
        MessageScrollViewer.ChangeView(null, MessageScrollViewer.ScrollableHeight, null);
    }
    
    private async void OnNewChatRequested(object sender, EventArgs e)
    {
        // Clear current messages to start a new chat
        _messages.Clear();
        
        // Close the sidebar
        await ChatSidebar.HideAsync();
    }
    
    private void OnClearHistoryRequested(object sender, EventArgs e)
    {
        // Clear all chat history
        // In a production app, this would clear from persistent storage
        _messages.Clear();
    }

    private void OnQuickActionClick(object sender, RoutedEventArgs e)
    {
        if (sender is XamlButton button && button.Tag is Models.QuickAction action)
        {
            MessageInput.Text = action.Query;
            SendMessage();
        }
    }

    private void OnVoiceButtonClick(object sender, RoutedEventArgs e)
    {
        // Navigate to voice mode
        Frame.Navigate(typeof(VoiceModeView));
    }

    private async void OnSendButtonClick(object sender, RoutedEventArgs e)
    {
        await SendMessage();
    }

    private async void OnMessageInputKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            await SendMessage();
            e.Handled = true;
        }
    }

    private async Task SendMessage()
    {
        var messageText = MessageInput.Text.Trim();
        if (string.IsNullOrEmpty(messageText))
        {
            return;
        }

        // Add user message
        var userMessage = new Message
        {
            Id = _messages.Count + 1,
            Role = MessageRole.User,
            Text = messageText,
            Type = MessageType.Text,
            Timestamp = DateTime.Now
        };
        _messages.Add(userMessage);

        // Clear input
        MessageInput.Text = string.Empty;

        // Scroll to bottom
        await Task.Delay(100);
        MessageScrollViewer.ChangeView(null, MessageScrollViewer.ScrollableHeight, null);

        try
        {
            // Show loading indicator
            LoadingIndicator.Show("Thinking...");
            
            // Get AI response
            var history = _messages
                .Select(m => new ChatMessage 
                { 
                    Role = m.Role == MessageRole.User ? ChatMessageRole.User : ChatMessageRole.Assistant,
                    Content = m.Text 
                })
                .ToList();

            var response = await _providerManager.SendMessageAsync(messageText, history);

            // Hide loading indicator
            LoadingIndicator.Hide();

            // Add AI message
            var aiMessage = new Message
            {
                Id = _messages.Count + 1,
                Role = MessageRole.AI,
                Text = response.Content,
                Type = MessageType.Text,
                Timestamp = DateTime.Now
            };
            _messages.Add(aiMessage);

            // Scroll to bottom
            await Task.Delay(100);
            MessageScrollViewer.ChangeView(null, MessageScrollViewer.ScrollableHeight, null);
        }
        catch (Exception ex)
        {
            // Hide loading indicator
            LoadingIndicator.Hide();
            
            // Handle error with user-friendly message
            var errorMessage = await ProviderErrorHandler.HandleProviderError(ex, _providerManager.GetActiveProviderName() ?? "AI");
            
            // Show error notification
            await ErrorNotification.ShowAsync(errorMessage, NotificationType.Error);
            
            // Add error message to chat
            var errorMsg = new Message
            {
                Id = _messages.Count + 1,
                Role = MessageRole.AI,
                Text = errorMessage,
                Type = MessageType.Text,
                Timestamp = DateTime.Now
            };
            _messages.Add(errorMsg);
            
            // Navigate to settings if API key error
            if (ProviderErrorHandler.RequiresSettingsNavigation(ex))
            {
                await Task.Delay(2000); // Give user time to read the error
                Frame.Navigate(typeof(SettingsView));
            }
        }
    }
}
