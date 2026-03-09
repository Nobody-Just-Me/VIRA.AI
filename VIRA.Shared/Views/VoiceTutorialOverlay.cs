using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Windows.UI;
using XamlButton = Microsoft.UI.Xaml.Controls.Button;

namespace VIRA.Shared.Views;

/// <summary>
/// Tutorial overlay that shows voice feature instructions on first launch
/// </summary>
public sealed class VoiceTutorialOverlay
{
    private Grid? _overlayGrid;
    private Action? _onDismiss;

    public UIElement BuildUI(Action onDismiss)
    {
        _onDismiss = onDismiss;
        
        // Semi-transparent overlay
        _overlayGrid = new Grid
        {
            Background = new SolidColorBrush(Color.FromArgb(220, 0, 0, 0)), // Semi-transparent black
            Visibility = Visibility.Visible
        };

        // Center content
        var contentStack = new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            MaxWidth = 400,
            Spacing = 24
        };

        // Tutorial card
        var tutorialCard = new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 30, 40, 60)),
            CornerRadius = new CornerRadius(20),
            Padding = new Thickness(32),
            BorderBrush = new SolidColorBrush(Color.FromArgb(255, 139, 92, 246)),
            BorderThickness = new Thickness(2)
        };

        var cardContent = new StackPanel
        {
            Spacing = 20
        };

        // Icon
        var icon = new TextBlock
        {
            Text = "🎤",
            FontSize = 64,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        cardContent.Children.Add(icon);

        // Title
        var title = new TextBlock
        {
            Text = "Selamat Datang di VIRA!",
            FontSize = 24,
            FontWeight = Microsoft.UI.Text.FontWeights.Bold,
            Foreground = new SolidColorBrush(Colors.White),
            HorizontalAlignment = HorizontalAlignment.Center,
            TextWrapping = TextWrapping.Wrap
        };
        cardContent.Children.Add(title);

        // Description
        var description = new TextBlock
        {
            Text = "VIRA adalah asisten AI pribadi Anda yang dapat dikendalikan dengan suara.",
            FontSize = 16,
            Foreground = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200)),
            HorizontalAlignment = HorizontalAlignment.Center,
            TextWrapping = TextWrapping.Wrap,
            TextAlignment = TextAlignment.Center
        };
        cardContent.Children.Add(description);

        // Separator
        var separator = new Border
        {
            Height = 1,
            Background = new SolidColorBrush(Color.FromArgb(100, 255, 255, 255)),
            Margin = new Thickness(0, 8, 0, 8)
        };
        cardContent.Children.Add(separator);

        // Instructions
        var instructionsStack = new StackPanel
        {
            Spacing = 16
        };

        AddInstruction(instructionsStack, "🎤", "Tap to Speak", 
            "Tekan tombol mikrofon ungu besar untuk berbicara dengan VIRA");
        
        AddInstruction(instructionsStack, "⌨️", "Type or Speak", 
            "Anda juga bisa mengetik pesan jika lebih nyaman");
        
        AddInstruction(instructionsStack, "🔄", "Continuous Mode", 
            "Aktifkan mode mendengar berkelanjutan di pengaturan untuk hands-free");

        cardContent.Children.Add(instructionsStack);

        // Got it button
        var gotItButton = new XamlButton
        {
            Content = "Mengerti, Mulai!",
            FontSize = 16,
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            Height = 50,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Background = new SolidColorBrush(Color.FromArgb(255, 139, 92, 246)),
            Foreground = new SolidColorBrush(Colors.White),
            CornerRadius = new CornerRadius(25),
            Margin = new Thickness(0, 16, 0, 0)
        };
        gotItButton.Click += (s, e) => Dismiss();
        cardContent.Children.Add(gotItButton);

        tutorialCard.Child = cardContent;
        contentStack.Children.Add(tutorialCard);

        _overlayGrid.Children.Add(contentStack);

        // Fade in animation
        var fadeIn = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = TimeSpan.FromMilliseconds(300)
        };
        
        Storyboard.SetTarget(fadeIn, _overlayGrid);
        Storyboard.SetTargetProperty(fadeIn, "Opacity");
        
        var storyboard = new Storyboard();
        storyboard.Children.Add(fadeIn);
        storyboard.Begin();

        return _overlayGrid;
    }

    private void AddInstruction(StackPanel parent, string emoji, string title, string description)
    {
        var instructionGrid = new Grid
        {
            ColumnSpacing = 12
        };
        
        instructionGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        instructionGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        // Emoji
        var emojiText = new TextBlock
        {
            Text = emoji,
            FontSize = 32,
            VerticalAlignment = VerticalAlignment.Top
        };
        Grid.SetColumn(emojiText, 0);
        instructionGrid.Children.Add(emojiText);

        // Text content
        var textStack = new StackPanel
        {
            Spacing = 4
        };

        var titleText = new TextBlock
        {
            Text = title,
            FontSize = 16,
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            Foreground = new SolidColorBrush(Colors.White)
        };
        textStack.Children.Add(titleText);

        var descText = new TextBlock
        {
            Text = description,
            FontSize = 14,
            Foreground = new SolidColorBrush(Color.FromArgb(255, 180, 180, 180)),
            TextWrapping = TextWrapping.Wrap
        };
        textStack.Children.Add(descText);

        Grid.SetColumn(textStack, 1);
        instructionGrid.Children.Add(textStack);

        parent.Children.Add(instructionGrid);
    }

    private void Dismiss()
    {
        if (_overlayGrid == null) return;

        // Fade out animation
        var fadeOut = new DoubleAnimation
        {
            From = 1,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(200)
        };
        
        Storyboard.SetTarget(fadeOut, _overlayGrid);
        Storyboard.SetTargetProperty(fadeOut, "Opacity");
        
        var storyboard = new Storyboard();
        storyboard.Children.Add(fadeOut);
        storyboard.Completed += (s, e) =>
        {
            _overlayGrid.Visibility = Visibility.Collapsed;
            _onDismiss?.Invoke();
        };
        storyboard.Begin();
    }
}
