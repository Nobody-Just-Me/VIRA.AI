using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI;
using XamlOrientation = Microsoft.UI.Xaml.Controls.Orientation;
using XamlButton = Microsoft.UI.Xaml.Controls.Button;

namespace VIRA.Shared.Views;

/// <summary>
/// Dialog for confirming transcribed voice input before sending
/// </summary>
public sealed class TranscriptionConfirmationDialog
{
    private Grid? _overlayGrid;
    private Action<bool, string>? _onResult;
    private TextBox? _transcriptionTextBox;

    public UIElement BuildUI(string transcribedText, Action<bool, string> onResult)
    {
        _onResult = onResult;

        // Semi-transparent overlay
        _overlayGrid = new Grid
        {
            Background = new SolidColorBrush(Color.FromArgb(180, 0, 0, 0)),
            Visibility = Visibility.Visible
        };

        // Center content
        var contentStack = new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            MaxWidth = 400,
            Spacing = 16
        };

        // Dialog card
        var dialogCard = new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 30, 40, 60)),
            CornerRadius = new CornerRadius(20),
            Padding = new Thickness(24),
            BorderBrush = new SolidColorBrush(Color.FromArgb(255, 139, 92, 246)),
            BorderThickness = new Thickness(2)
        };

        var cardContent = new StackPanel
        {
            Spacing = 16
        };

        // Title
        var title = new TextBlock
        {
            Text = "Confirm Transcription",
            FontSize = 20,
            FontWeight = Microsoft.UI.Text.FontWeights.Bold,
            Foreground = new SolidColorBrush(Colors.White),
            HorizontalAlignment = HorizontalAlignment.Center
        };
        cardContent.Children.Add(title);

        // Description
        var description = new TextBlock
        {
            Text = "Review and edit the transcribed text before sending:",
            FontSize = 14,
            Foreground = new SolidColorBrush(Color.FromArgb(255, 180, 180, 180)),
            TextWrapping = TextWrapping.Wrap,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        cardContent.Children.Add(description);

        // Transcription text box (editable)
        _transcriptionTextBox = new TextBox
        {
            Text = transcribedText,
            TextWrapping = TextWrapping.Wrap,
            AcceptsReturn = true,
            MinHeight = 100,
            MaxHeight = 200,
            Background = new SolidColorBrush(Color.FromArgb(255, 20, 30, 50)),
            Foreground = new SolidColorBrush(Colors.White),
            BorderBrush = new SolidColorBrush(Color.FromArgb(255, 139, 92, 246)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(12)
        };
        cardContent.Children.Add(_transcriptionTextBox);

        // Buttons
        var buttonStack = new StackPanel
        {
            Orientation = XamlOrientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            Spacing = 12
        };

        // Retry button
        var retryButton = new XamlButton
        {
            Content = "🔄 Retry",
            FontSize = 14,
            Width = 100,
            Height = 44,
            Background = new SolidColorBrush(Color.FromArgb(255, 60, 60, 80)),
            Foreground = new SolidColorBrush(Colors.White),
            CornerRadius = new CornerRadius(22)
        };
        retryButton.Click += (s, e) => Dismiss(false, string.Empty);
        buttonStack.Children.Add(retryButton);

        // Cancel button
        var cancelButton = new XamlButton
        {
            Content = "✕ Cancel",
            FontSize = 14,
            Width = 100,
            Height = 44,
            Background = new SolidColorBrush(Color.FromArgb(255, 239, 68, 68)),
            Foreground = new SolidColorBrush(Colors.White),
            CornerRadius = new CornerRadius(22)
        };
        cancelButton.Click += (s, e) => Dismiss(false, string.Empty);
        buttonStack.Children.Add(cancelButton);

        // Send button
        var sendButton = new XamlButton
        {
            Content = "✓ Send",
            FontSize = 14,
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            Width = 100,
            Height = 44,
            Background = new SolidColorBrush(Color.FromArgb(255, 139, 92, 246)),
            Foreground = new SolidColorBrush(Colors.White),
            CornerRadius = new CornerRadius(22)
        };
        sendButton.Click += (s, e) =>
        {
            var text = _transcriptionTextBox?.Text?.Trim() ?? string.Empty;
            if (!string.IsNullOrEmpty(text))
            {
                Dismiss(true, text);
            }
        };
        buttonStack.Children.Add(sendButton);

        cardContent.Children.Add(buttonStack);

        dialogCard.Child = cardContent;
        contentStack.Children.Add(dialogCard);

        _overlayGrid.Children.Add(contentStack);

        return _overlayGrid;
    }

    private void Dismiss(bool confirmed, string text)
    {
        if (_overlayGrid != null)
        {
            _overlayGrid.Visibility = Visibility.Collapsed;
        }
        
        _onResult?.Invoke(confirmed, text);
    }
}
