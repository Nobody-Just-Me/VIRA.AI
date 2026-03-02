using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using VIRA.Shared.ViewModels;
using VIRA.Shared.Models;
using Windows.UI;
using Button = Microsoft.UI.Xaml.Controls.Button;
using TextBox = Microsoft.UI.Xaml.Controls.TextBox;

namespace VIRA.Shared.Views;

// Plain class that builds UI - does NOT inherit from UserControl to avoid source generator
public sealed class ViraChatView
{
    public MainChatViewModel? ViewModel { get; }
    private ScrollViewer? _chatScrollViewer;
    private StackPanel? _messagesPanel;
    private TextBox? _inputBox;
    private Button? _sendButton;
    private Button? _voiceButton;
    private Grid? _mainGrid;

    public ViraChatView()
    {
        System.Diagnostics.Debug.WriteLine("ViraChatView: Constructor called");
        ViewModel = App.Current?.Services?.GetService<MainChatViewModel>();
        System.Diagnostics.Debug.WriteLine($"ViraChatView: ViewModel = {ViewModel != null}");
    }

    public UIElement BuildUI()
    {
        System.Diagnostics.Debug.WriteLine("ViraChatView: BuildUI started");
        
        // Main Grid
        _mainGrid = new Grid
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 16, 22, 34)) // #101622
        };

        _mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(60) }); // Header
        _mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // Chat
        _mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Input

        // Header
        var header = CreateHeader();
        Grid.SetRow(header, 0);
        _mainGrid.Children.Add(header);

        // Chat Area
        _messagesPanel = new StackPanel { Padding = new Thickness(16) };
        _chatScrollViewer = new ScrollViewer
        {
            Content = _messagesPanel,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto
        };
        Grid.SetRow(_chatScrollViewer, 1);
        _mainGrid.Children.Add(_chatScrollViewer);

        // Input Area
        var inputArea = CreateInputArea();
        Grid.SetRow(inputArea, 2);
        _mainGrid.Children.Add(inputArea);

        // Subscribe to ViewModel changes
        if (ViewModel != null)
        {
            ViewModel.Messages.CollectionChanged += (s, e) => UpdateMessages();
        }

        System.Diagnostics.Debug.WriteLine("ViraChatView: BuildUI completed");
        return _mainGrid;
    }

    private Grid CreateHeader()
    {
        var header = new Grid
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 16, 22, 34)),
            Padding = new Thickness(16, 8, 16, 8)
        };

        header.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        header.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        header.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        // Menu Button
        var menuButton = new Button
        {
            Content = "☰",
            FontSize = 20,
            Background = new SolidColorBrush(Colors.Transparent),
            Foreground = new SolidColorBrush(Colors.White)
        };
        menuButton.Click += (s, e) => ViewModel.ClearChatCommand.Execute(null);
        Grid.SetColumn(menuButton, 0);
        header.Children.Add(menuButton);

        // Title
        var title = new TextBlock
        {
            Text = "VIRA",
            FontSize = 24,
            FontWeight = Microsoft.UI.Text.FontWeights.Bold,
            Foreground = new SolidColorBrush(Color.FromArgb(255, 37, 106, 244)), // #256AF4
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(title, 1);
        header.Children.Add(title);

        // Settings Button
        var settingsButton = new Button
        {
            Content = "⚙",
            FontSize = 20,
            Background = new SolidColorBrush(Colors.Transparent),
            Foreground = new SolidColorBrush(Colors.White)
        };
        // settingsButton.Click += (s, e) => { /* TODO: Navigate to settings */ };
        Grid.SetColumn(settingsButton, 2);
        header.Children.Add(settingsButton);

        return header;
    }

    private Grid CreateInputArea()
    {
        var inputGrid = new Grid
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 16, 22, 34)),
            Padding = new Thickness(16)
        };

        inputGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        inputGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        inputGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        // Voice Button
        _voiceButton = new Button
        {
            Content = "🎤",
            FontSize = 24,
            Width = 50,
            Height = 50,
            Margin = new Thickness(0, 0, 8, 0),
            Background = new SolidColorBrush(Color.FromArgb(255, 37, 106, 244)),
            Foreground = new SolidColorBrush(Colors.White),
            CornerRadius = new CornerRadius(25)
        };
        _voiceButton.Click += async (s, e) => await OnVoiceButtonClick();
        Grid.SetColumn(_voiceButton, 0);
        inputGrid.Children.Add(_voiceButton);

        // Input Box
        _inputBox = new TextBox
        {
            PlaceholderText = "Ketik pesan...",
            Height = 50,
            Margin = new Thickness(0, 0, 8, 0),
            Background = new SolidColorBrush(Color.FromArgb(255, 30, 40, 60)),
            Foreground = new SolidColorBrush(Colors.White),
            CornerRadius = new CornerRadius(25),
            Padding = new Thickness(16, 0, 16, 0)
        };
        _inputBox.KeyDown += (s, e) =>
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                OnSendButtonClick();
            }
        };
        Grid.SetColumn(_inputBox, 1);
        inputGrid.Children.Add(_inputBox);

        // Send Button
        _sendButton = new Button
        {
            Content = "➤",
            FontSize = 24,
            Width = 50,
            Height = 50,
            Background = new SolidColorBrush(Color.FromArgb(255, 37, 106, 244)),
            Foreground = new SolidColorBrush(Colors.White),
            CornerRadius = new CornerRadius(25)
        };
        _sendButton.Click += (s, e) => OnSendButtonClick();
        Grid.SetColumn(_sendButton, 2);
        inputGrid.Children.Add(_sendButton);

        return inputGrid;
    }

    private void UpdateMessages()
    {
        if (_messagesPanel == null || ViewModel == null) return;
        
        _messagesPanel.Children.Clear();
        foreach (var message in ViewModel.Messages)
        {
            var isUser = message.Role == Models.ChatMessageRole.User;
            var messageCard = CreateMessageCard(message.Content, isUser);
            _messagesPanel.Children.Add(messageCard);
        }
    }

    private Border CreateMessageCard(string text, bool isUser)
    {
        var card = new Border
        {
            Background = new SolidColorBrush(isUser 
                ? Color.FromArgb(255, 37, 106, 244) 
                : Color.FromArgb(255, 30, 40, 60)),
            CornerRadius = new CornerRadius(12),
            Padding = new Thickness(16),
            Margin = new Thickness(0, 0, 0, 12),
            HorizontalAlignment = isUser ? HorizontalAlignment.Right : HorizontalAlignment.Left,
            MaxWidth = 300
        };

        var textBlock = new TextBlock
        {
            Text = text,
            Foreground = new SolidColorBrush(Colors.White),
            TextWrapping = TextWrapping.Wrap
        };

        card.Child = textBlock;
        return card;
    }

    private async void OnSendButtonClick()
    {
        if (_inputBox == null || ViewModel == null) return;
        
        var message = _inputBox.Text?.Trim();
        if (!string.IsNullOrEmpty(message))
        {
            _inputBox.Text = string.Empty;
            await ViewModel.SendMessageCommand.ExecuteAsync(message);
        }
    }

    private async Task OnVoiceButtonClick()
    {
        if (ViewModel == null) return;
        await ViewModel.StartVoiceInputCommand.ExecuteAsync(null);
    }
}
