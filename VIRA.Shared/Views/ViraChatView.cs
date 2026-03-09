using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using VIRA.Shared.ViewModels;
using VIRA.Shared.Models;
using VIRA.Shared.Services;
using Windows.UI;
using XamlButton = Microsoft.UI.Xaml.Controls.Button;
using XamlTextBox = Microsoft.UI.Xaml.Controls.TextBox;
using XamlOrientation = Microsoft.UI.Xaml.Controls.Orientation;
using Microsoft.UI.Dispatching;

namespace VIRA.Shared.Views;

// Plain class that builds UI - does NOT inherit from UserControl to avoid source generator
public sealed class ViraChatView
{
    public MainChatViewModel? ViewModel { get; }
    private readonly IPreferencesService? _preferencesService;
    private ScrollViewer? _chatScrollViewer;
    private StackPanel? _messagesPanel;
    private XamlTextBox? _inputBox;
    private XamlButton? _sendButton;
    private XamlButton? _voiceButton;
    private Grid? _mainGrid;
    private Border? _voicePulseRing;
    private Border? _tapToSpeakHint;
    private Grid? _tutorialOverlay;
    private Grid? _confirmationOverlay;
    private VoiceStateIndicator? _voiceStateIndicator;
    private bool _isVoiceActive = false;
    private DispatcherTimer? _pulseTimer;

    public ViraChatView()
    {
        System.Diagnostics.Debug.WriteLine("ViraChatView: Constructor called");
        ViewModel = App.Current?.Services?.GetService<MainChatViewModel>();
        _preferencesService = App.Current?.Services?.GetService<IPreferencesService>();
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
            ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        // Add voice state indicator
        _voiceStateIndicator = new VoiceStateIndicator();
        var stateIndicator = _voiceStateIndicator.BuildUI();
        Grid.SetRow(stateIndicator, 1);
        Grid.SetRowSpan(stateIndicator, 2);
        _mainGrid.Children.Add(stateIndicator);

        // Show tutorial on first launch
        ShowTutorialIfNeeded();

        System.Diagnostics.Debug.WriteLine("ViraChatView: BuildUI completed");
        return _mainGrid;
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (ViewModel == null) return;

        switch (e.PropertyName)
        {
            case nameof(ViewModel.VoiceState):
                _voiceStateIndicator?.UpdateState(ViewModel.VoiceState, ViewModel.VoiceStateDetail);
                break;

            case nameof(ViewModel.ShowTranscriptionConfirmation):
                if (ViewModel.ShowTranscriptionConfirmation)
                {
                    ShowTranscriptionConfirmation(ViewModel.TranscribedText);
                }
                else
                {
                    HideTranscriptionConfirmation();
                }
                break;
        }
    }

    private void ShowTutorialIfNeeded()
    {
        if (_preferencesService == null || _mainGrid == null) return;

        // Show tutorial only on first launch
        if (_preferencesService.IsFirstLaunch && !_preferencesService.HasShownVoiceTutorial)
        {
            var tutorial = new VoiceTutorialOverlay();
            _tutorialOverlay = tutorial.BuildUI(() =>
            {
                // Mark tutorial as shown
                _preferencesService.HasShownVoiceTutorial = true;
                _preferencesService.MarkFirstLaunchComplete();
                
                // Remove overlay
                if (_tutorialOverlay != null && _mainGrid != null)
                {
                    _mainGrid.Children.Remove(_tutorialOverlay);
                    _tutorialOverlay = null;
                }
                
                // Show "Tap to speak" hint after tutorial
                ShowTapToSpeakHint();
            }) as Grid;

            if (_tutorialOverlay != null)
            {
                Grid.SetRowSpan(_tutorialOverlay, 3);
                _mainGrid.Children.Add(_tutorialOverlay);
            }
        }
        else if (!_preferencesService.HasShownVoiceTutorial)
        {
            // Show hint if tutorial was skipped
            ShowTapToSpeakHint();
        }
    }

    private void ShowTapToSpeakHint()
    {
        if (_mainGrid == null || _voiceButton == null) return;

        // Create "Tap to speak" hint near voice button
        _tapToSpeakHint = new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(230, 139, 92, 246)),
            CornerRadius = new CornerRadius(12),
            Padding = new Thickness(16, 8, 16, 8),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Bottom,
            Margin = new Thickness(90, 0, 0, 80), // Position above voice button
            Opacity = 0
        };

        var hintStack = new StackPanel
        {
            Orientation = XamlOrientation.Horizontal,
            Spacing = 8
        };

        var hintIcon = new TextBlock
        {
            Text = "👆",
            FontSize = 20
        };
        hintStack.Children.Add(hintIcon);

        var hintText = new TextBlock
        {
            Text = "Tap to speak",
            FontSize = 14,
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            Foreground = new SolidColorBrush(Colors.White)
        };
        hintStack.Children.Add(hintText);

        _tapToSpeakHint.Child = hintStack;

        // Add to main grid
        Grid.SetRow(_tapToSpeakHint, 2);
        _mainGrid.Children.Add(_tapToSpeakHint);

        // Animate hint: fade in, pulse, then fade out
        AnimateTapToSpeakHint();
    }

    private async void AnimateTapToSpeakHint()
    {
        if (_tapToSpeakHint == null) return;

        // Fade in
        for (double opacity = 0; opacity <= 1; opacity += 0.1)
        {
            _tapToSpeakHint.Opacity = opacity;
            await Task.Delay(30);
        }

        // Stay visible for 3 seconds with pulse
        for (int i = 0; i < 3; i++)
        {
            await Task.Delay(500);
            _tapToSpeakHint.Opacity = 0.7;
            await Task.Delay(500);
            _tapToSpeakHint.Opacity = 1.0;
        }

        // Fade out
        for (double opacity = 1; opacity >= 0; opacity -= 0.1)
        {
            _tapToSpeakHint.Opacity = opacity;
            await Task.Delay(30);
        }

        // Remove hint
        if (_mainGrid != null && _tapToSpeakHint != null)
        {
            _mainGrid.Children.Remove(_tapToSpeakHint);
            _tapToSpeakHint = null;
        }
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
        header.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        // Menu Button
        var menuButton = new XamlButton
        {
            Content = "☰",
            FontSize = 20,
            Background = new SolidColorBrush(Colors.Transparent),
            Foreground = new SolidColorBrush(Colors.White)
        };
        menuButton.Click += (s, e) => ViewModel?.ClearChatCommand.Execute(null);
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

        // Continuous Listening Toggle
        var continuousButton = new XamlButton
        {
            Content = "🔄",
            FontSize = 18,
            Width = 40,
            Height = 40,
            Background = new SolidColorBrush(Colors.Transparent),
            Foreground = new SolidColorBrush(Colors.White),
            Margin = new Thickness(0, 0, 8, 0)
        };
        ToolTipService.SetToolTip(continuousButton, new ToolTip { Content = "Toggle Continuous Listening" });
        continuousButton.Click += (s, e) =>
        {
            ViewModel?.ToggleContinuousListeningCommand.Execute(null);
            // Update button appearance based on state
            if (ViewModel != null)
            {
                continuousButton.Background = new SolidColorBrush(
                    ViewModel.IsContinuousListening 
                        ? Color.FromArgb(255, 139, 92, 246) 
                        : Colors.Transparent
                );
            }
        };
        Grid.SetColumn(continuousButton, 2);
        header.Children.Add(continuousButton);

        // Settings Button
        var settingsButton = new XamlButton
        {
            Content = "⚙",
            FontSize = 20,
            Background = new SolidColorBrush(Colors.Transparent),
            Foreground = new SolidColorBrush(Colors.White)
        };
        // settingsButton.Click += (s, e) => { /* TODO: Navigate to settings */ };
        Grid.SetColumn(settingsButton, 3);
        header.Children.Add(settingsButton);

        return header;
    }

    private Grid CreateInputArea()
    {
        var inputGrid = new Grid
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 16, 22, 34)),
            Padding = new Thickness(16, 12, 16, 20)
        };

        inputGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Voice button (prominent)
        inputGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Input
        inputGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Send button

        // PROMINENT Voice Button - 2x larger, primary position
        _voiceButton = new XamlButton
        {
            Content = "🎤",
            FontSize = 32, // Increased from 24
            Width = 70,    // Increased from 50 (2x larger)
            Height = 70,   // Increased from 50
            Margin = new Thickness(0, 0, 12, 0),
            Background = new SolidColorBrush(Color.FromArgb(255, 139, 92, 246)), // Purple gradient start
            Foreground = new SolidColorBrush(Colors.White),
            CornerRadius = new CornerRadius(35), // Fully rounded
            BorderThickness = new Thickness(0),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center
        };
        
        // Add shadow effect for prominence
        _voiceButton.Shadow = new Microsoft.UI.Xaml.Media.ThemeShadow();
        _voiceButton.Translation = new System.Numerics.Vector3(0, 0, 8);
        
        _voiceButton.Click += async (s, e) => await OnVoiceButtonClick();
        Grid.SetColumn(_voiceButton, 0);
        inputGrid.Children.Add(_voiceButton);

        // Input Box - Sleeker design
        _inputBox = new XamlTextBox
        {
            PlaceholderText = "Tap to speak or type here...",
            Height = 56,
            Margin = new Thickness(0, 0, 12, 0),
            Background = new SolidColorBrush(Color.FromArgb(255, 30, 40, 60)),
            Foreground = new SolidColorBrush(Colors.White),
            CornerRadius = new CornerRadius(28),
            Padding = new Thickness(20, 0, 20, 0),
            BorderThickness = new Thickness(0),
            VerticalAlignment = VerticalAlignment.Center
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

        // Send Button - Slightly smaller, secondary action
        _sendButton = new XamlButton
        {
            Content = "➤",
            FontSize = 20,
            Width = 56,
            Height = 56,
            Background = new SolidColorBrush(Color.FromArgb(255, 37, 106, 244)),
            Foreground = new SolidColorBrush(Colors.White),
            CornerRadius = new CornerRadius(28),
            BorderThickness = new Thickness(0),
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center
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
        
        // Toggle voice active state
        _isVoiceActive = !_isVoiceActive;
        
        if (_isVoiceActive)
        {
            // Start pulsing animation
            StartVoicePulseAnimation();
            
            // Change button appearance to "listening" state
            if (_voiceButton != null)
            {
                _voiceButton.Background = new SolidColorBrush(Color.FromArgb(255, 239, 68, 68)); // Red for recording
                _voiceButton.Content = "⏹"; // Stop icon
            }
            
            // Update input box placeholder
            if (_inputBox != null)
            {
                _inputBox.PlaceholderText = ViewModel.IsContinuousListening 
                    ? "Listening continuously... Tap stop to end" 
                    : "Listening...";
            }
            
            await ViewModel.StartVoiceInputCommand.ExecuteAsync(null);
            
            // Stop animation after voice input completes (if not continuous)
            if (!ViewModel.IsContinuousListening)
            {
                StopVoicePulseAnimation();
                ResetVoiceButton();
            }
        }
        else
        {
            // Stop listening
            ViewModel.StopVoiceInputCommand.Execute(null);
            StopVoicePulseAnimation();
            ResetVoiceButton();
        }
    }
    
    private void StartVoicePulseAnimation()
    {
        if (_voiceButton == null) return;
        
        // Create pulsing ring effect
        _voicePulseRing = new Border
        {
            Width = 70,
            Height = 70,
            CornerRadius = new CornerRadius(35),
            BorderBrush = new SolidColorBrush(Color.FromArgb(100, 239, 68, 68)),
            BorderThickness = new Thickness(3),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 0, 12, 0)
        };
        
        // Add to parent grid
        if (_mainGrid != null && _mainGrid.Children.Count > 2)
        {
            var inputArea = _mainGrid.Children[2] as Grid;
            if (inputArea != null)
            {
                Grid.SetColumn(_voicePulseRing, 0);
                inputArea.Children.Insert(0, _voicePulseRing);
            }
        }
        
        // Animate pulse
        _pulseTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(1000)
        };
        
        bool expanding = true;
        _pulseTimer.Tick += (s, e) =>
        {
            if (_voicePulseRing == null) return;
            
            if (expanding)
            {
                _voicePulseRing.Width = 90;
                _voicePulseRing.Height = 90;
                _voicePulseRing.Opacity = 0.3;
            }
            else
            {
                _voicePulseRing.Width = 70;
                _voicePulseRing.Height = 70;
                _voicePulseRing.Opacity = 0.8;
            }
            expanding = !expanding;
        };
        
        _pulseTimer.Start();
    }
    
    private void StopVoicePulseAnimation()
    {
        _pulseTimer?.Stop();
        _pulseTimer = null;
        
        // Remove pulse ring
        if (_voicePulseRing != null && _mainGrid != null && _mainGrid.Children.Count > 2)
        {
            var inputArea = _mainGrid.Children[2] as Grid;
            if (inputArea != null)
            {
                inputArea.Children.Remove(_voicePulseRing);
            }
            _voicePulseRing = null;
        }
    }
    
    private void ResetVoiceButton()
    {
        _isVoiceActive = false;
        
        if (_voiceButton != null)
        {
            _voiceButton.Background = new SolidColorBrush(Color.FromArgb(255, 139, 92, 246)); // Back to purple
            _voiceButton.Content = "🎤"; // Mic icon
        }
        
        if (_inputBox != null)
        {
            _inputBox.PlaceholderText = "Tap to speak or type here...";
        }
    }

    private void ShowTranscriptionConfirmation(string transcribedText)
    {
        if (_mainGrid == null || ViewModel == null) return;

        var confirmationDialog = new TranscriptionConfirmationDialog();
        _confirmationOverlay = confirmationDialog.BuildUI(transcribedText, async (confirmed, text) =>
        {
            if (confirmed && !string.IsNullOrEmpty(text))
            {
                await ViewModel.ConfirmTranscriptionCommand.ExecuteAsync(text);
            }
            else if (!confirmed && !string.IsNullOrEmpty(text))
            {
                // Retry was clicked
                await ViewModel.RetryVoiceInputCommand.ExecuteAsync(null);
            }
            else
            {
                // Cancel was clicked
                ViewModel.CancelTranscriptionCommand.Execute(null);
            }

            HideTranscriptionConfirmation();
        }) as Grid;

        if (_confirmationOverlay != null)
        {
            Grid.SetRowSpan(_confirmationOverlay, 3);
            _mainGrid.Children.Add(_confirmationOverlay);
        }
    }

    private void HideTranscriptionConfirmation()
    {
        if (_confirmationOverlay != null && _mainGrid != null)
        {
            _mainGrid.Children.Remove(_confirmationOverlay);
            _confirmationOverlay = null;
        }
    }
}
