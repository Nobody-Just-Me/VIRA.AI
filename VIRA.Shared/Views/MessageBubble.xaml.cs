using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using VIRA.Shared.Models;
using Windows.UI;

namespace VIRA.Shared.Views;

/// <summary>
/// Message bubble component with styling based on message role
/// </summary>
public sealed partial class MessageBubble : UserControl
{
    public static readonly DependencyProperty MessageContentProperty =
        DependencyProperty.Register(
            nameof(MessageContent),
            typeof(Message),
            typeof(MessageBubble),
            new PropertyMetadata(null, OnMessageContentChanged));

    public Message MessageContent
    {
        get => (Message)GetValue(MessageContentProperty);
        set => SetValue(MessageContentProperty, value);
    }

    public bool IsUserMessage => MessageContent?.Role == MessageRole.User;

    public string FormattedTimestamp
    {
        get
        {
            if (MessageContent == null) return string.Empty;
            
            var now = DateTime.Now;
            var msgTime = MessageContent.Timestamp;
            
            if (msgTime.Date == now.Date)
            {
                return msgTime.ToString("HH:mm");
            }
            else if (msgTime.Date == now.Date.AddDays(-1))
            {
                return $"Yesterday {msgTime:HH:mm}";
            }
            else
            {
                return msgTime.ToString("MMM dd, HH:mm");
            }
        }
    }

    public MessageBubble()
    {
        this.InitializeComponent();
        this.Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        // Apply entrance animation: fade-in and slide-up
        await Services.AnimationService.SlideInFromBottomAsync(this, 12);
    }

    private static void OnMessageContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is MessageBubble bubble)
        {
            bubble.UpdateStyling();
        }
    }

    private void UpdateStyling()
    {
        if (MessageContent == null) return;

        // Set text content
        MessageText.Text = MessageContent.Text;
        TimestampText.Text = FormattedTimestamp;

        // Set alignment based on role
        RootGrid.HorizontalAlignment = MessageContent.Role == MessageRole.User 
            ? HorizontalAlignment.Right 
            : HorizontalAlignment.Left;

        if (MessageContent.Role == MessageRole.User)
        {
            // User message: Purple gradient with shadow
            var gradientBrush = new LinearGradientBrush
            {
                StartPoint = new Windows.Foundation.Point(0, 0),
                EndPoint = new Windows.Foundation.Point(1, 1)
            };
            gradientBrush.GradientStops.Add(new GradientStop 
            { 
                Color = Color.FromArgb(255, 139, 92, 246), // #8b5cf6
                Offset = 0 
            });
            gradientBrush.GradientStops.Add(new GradientStop 
            { 
                Color = Color.FromArgb(255, 124, 58, 237), // #7c3aed
                Offset = 1 
            });
            
            MessageBorder.Background = gradientBrush;
            MessageText.Foreground = new SolidColorBrush(Colors.White);
            
            // Add shadow
            MessageBorder.Shadow = new Microsoft.UI.Xaml.Media.ThemeShadow();
            MessageBorder.Translation = new System.Numerics.Vector3(0, 0, 8);
        }
        else
        {
            // AI message: Semi-transparent white with backdrop blur
            MessageBorder.Background = new SolidColorBrush(
                Color.FromArgb(13, 255, 255, 255) // #0DFFFFFF (5% opacity)
            );
            MessageText.Foreground = new SolidColorBrush(
                Color.FromArgb(255, 241, 245, 249) // #F1F5F9
            );
            
            // No shadow for AI messages
            MessageBorder.Shadow = null;
            MessageBorder.Translation = new System.Numerics.Vector3(0, 0, 0);
        }
    }
}
