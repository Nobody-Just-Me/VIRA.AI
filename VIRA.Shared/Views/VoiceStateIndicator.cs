using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using VIRA.Shared.Models;
using Windows.UI;
using XamlOrientation = Microsoft.UI.Xaml.Controls.Orientation;

namespace VIRA.Shared.Views;

/// <summary>
/// Visual indicator for voice recognition state
/// </summary>
public sealed class VoiceStateIndicator
{
    private Border? _indicatorBorder;
    private StackPanel? _contentStack;
    private TextBlock? _stateText;
    private TextBlock? _detailText;
    private ProgressRing? _progressRing;
    private Storyboard? _pulseAnimation;

    public UIElement BuildUI()
    {
        _indicatorBorder = new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(240, 30, 40, 60)),
            CornerRadius = new CornerRadius(16),
            Padding = new Thickness(20, 16, 20, 16),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(0, 80, 0, 0),
            Visibility = Visibility.Collapsed,
            BorderBrush = new SolidColorBrush(Color.FromArgb(255, 139, 92, 246)),
            BorderThickness = new Thickness(2)
        };

        _contentStack = new StackPanel
        {
            Orientation = XamlOrientation.Horizontal,
            Spacing = 12
        };

        // Progress ring for loading states
        _progressRing = new ProgressRing
        {
            Width = 24,
            Height = 24,
            IsActive = false,
            Foreground = new SolidColorBrush(Color.FromArgb(255, 139, 92, 246))
        };
        _contentStack.Children.Add(_progressRing);

        // Text content
        var textStack = new StackPanel
        {
            Spacing = 4
        };

        _stateText = new TextBlock
        {
            FontSize = 16,
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            Foreground = new SolidColorBrush(Colors.White)
        };
        textStack.Children.Add(_stateText);

        _detailText = new TextBlock
        {
            FontSize = 13,
            Foreground = new SolidColorBrush(Color.FromArgb(255, 180, 180, 180)),
            TextWrapping = TextWrapping.Wrap,
            MaxWidth = 300
        };
        textStack.Children.Add(_detailText);

        _contentStack.Children.Add(textStack);
        _indicatorBorder.Child = _contentStack;

        return _indicatorBorder;
    }

    public void UpdateState(VoiceRecognitionState state, string? detail = null)
    {
        if (_indicatorBorder == null || _stateText == null || _detailText == null || _progressRing == null)
            return;

        // Show indicator
        _indicatorBorder.Visibility = Visibility.Visible;

        switch (state)
        {
            case VoiceRecognitionState.Idle:
                _indicatorBorder.Visibility = Visibility.Collapsed;
                StopPulseAnimation();
                break;

            case VoiceRecognitionState.Listening:
                _stateText.Text = "🎤 Listening...";
                _detailText.Text = detail ?? "Speak now";
                _progressRing.IsActive = false;
                _indicatorBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 139, 92, 246));
                _indicatorBorder.Background = new SolidColorBrush(Color.FromArgb(240, 139, 92, 246));
                StartPulseAnimation();
                break;

            case VoiceRecognitionState.Processing:
                _stateText.Text = "⚙️ Processing...";
                _detailText.Text = detail ?? "Recognizing speech";
                _progressRing.IsActive = true;
                _indicatorBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 37, 106, 244));
                _indicatorBorder.Background = new SolidColorBrush(Color.FromArgb(240, 30, 40, 60));
                StopPulseAnimation();
                break;

            case VoiceRecognitionState.AwaitingConfirmation:
                _stateText.Text = "✓ Transcribed";
                _detailText.Text = detail ?? "Confirm to send";
                _progressRing.IsActive = false;
                _indicatorBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 34, 197, 94));
                _indicatorBorder.Background = new SolidColorBrush(Color.FromArgb(240, 30, 40, 60));
                StopPulseAnimation();
                break;

            case VoiceRecognitionState.Error:
                _stateText.Text = "❌ Error";
                _detailText.Text = detail ?? "Voice recognition failed";
                _progressRing.IsActive = false;
                _indicatorBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 239, 68, 68));
                _indicatorBorder.Background = new SolidColorBrush(Color.FromArgb(240, 60, 30, 30));
                StopPulseAnimation();
                break;
        }

        // Fade in animation
        FadeIn();
    }

    public void Hide()
    {
        if (_indicatorBorder == null) return;
        
        FadeOut(() =>
        {
            if (_indicatorBorder != null)
            {
                _indicatorBorder.Visibility = Visibility.Collapsed;
            }
        });
    }

    private void StartPulseAnimation()
    {
        if (_indicatorBorder == null) return;

        StopPulseAnimation();

        var opacityAnimation = new DoubleAnimation
        {
            From = 1.0,
            To = 0.6,
            Duration = TimeSpan.FromMilliseconds(800),
            AutoReverse = true,
            RepeatBehavior = RepeatBehavior.Forever
        };

        Storyboard.SetTarget(opacityAnimation, _indicatorBorder);
        Storyboard.SetTargetProperty(opacityAnimation, "Opacity");

        _pulseAnimation = new Storyboard();
        _pulseAnimation.Children.Add(opacityAnimation);
        _pulseAnimation.Begin();
    }

    private void StopPulseAnimation()
    {
        if (_pulseAnimation != null)
        {
            _pulseAnimation.Stop();
            _pulseAnimation = null;
        }

        if (_indicatorBorder != null)
        {
            _indicatorBorder.Opacity = 1.0;
        }
    }

    private void FadeIn()
    {
        if (_indicatorBorder == null) return;

        var fadeIn = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = TimeSpan.FromMilliseconds(200)
        };

        Storyboard.SetTarget(fadeIn, _indicatorBorder);
        Storyboard.SetTargetProperty(fadeIn, "Opacity");

        var storyboard = new Storyboard();
        storyboard.Children.Add(fadeIn);
        storyboard.Begin();
    }

    private void FadeOut(Action? onComplete = null)
    {
        if (_indicatorBorder == null) return;

        var fadeOut = new DoubleAnimation
        {
            From = 1,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(200)
        };

        Storyboard.SetTarget(fadeOut, _indicatorBorder);
        Storyboard.SetTargetProperty(fadeOut, "Opacity");

        var storyboard = new Storyboard();
        storyboard.Children.Add(fadeOut);
        
        if (onComplete != null)
        {
            storyboard.Completed += (s, e) => onComplete();
        }
        
        storyboard.Begin();
    }
}
