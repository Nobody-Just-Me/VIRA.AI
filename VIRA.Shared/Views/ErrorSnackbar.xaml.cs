using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Threading.Tasks;

namespace VIRA.Shared.Views
{
    /// <summary>
    /// Notification type for styling
    /// </summary>
    public enum NotificationType
    {
        Error,
        Success,
        Info,
        Warning
    }

    /// <summary>
    /// Snackbar notification component with auto-dismiss
    /// </summary>
    public sealed partial class ErrorSnackbar : UserControl
    {
        private DispatcherTimer? _dismissTimer;

        public ErrorSnackbar()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Shows a notification with the specified message and type
        /// </summary>
        /// <param name="message">Message to display</param>
        /// <param name="type">Type of notification (Error, Success, Info, Warning)</param>
        /// <param name="durationMs">Duration in milliseconds (default 3000)</param>
        public async Task ShowAsync(string message, NotificationType type = NotificationType.Error, int durationMs = 3000)
        {
            // Set message
            MessageText.Text = message;

            // Apply styling based on type
            ApplyStyling(type);

            // Show the snackbar
            RootGrid.Visibility = Visibility.Visible;

            // Animate in
            await AnimateInAsync();

            // Start dismiss timer
            StartDismissTimer(durationMs);
        }

        private void ApplyStyling(NotificationType type)
        {
            Windows.UI.Color borderColor;
            Windows.UI.Color accentColor;
            string icon;

            switch (type)
            {
                case NotificationType.Error:
                    borderColor = Windows.UI.Color.FromArgb(255, 239, 68, 68); // #ef4444
                    accentColor = Windows.UI.Color.FromArgb(255, 239, 68, 68);
                    icon = "❌";
                    break;

                case NotificationType.Success:
                    borderColor = Windows.UI.Color.FromArgb(255, 34, 197, 94); // #22c55e
                    accentColor = Windows.UI.Color.FromArgb(255, 34, 197, 94);
                    icon = "✅";
                    break;

                case NotificationType.Info:
                    borderColor = Windows.UI.Color.FromArgb(255, 32, 148, 243); // #2094f3
                    accentColor = Windows.UI.Color.FromArgb(255, 32, 148, 243);
                    icon = "ℹ️";
                    break;

                case NotificationType.Warning:
                    borderColor = Windows.UI.Color.FromArgb(255, 234, 179, 8); // #eab308
                    accentColor = Windows.UI.Color.FromArgb(255, 234, 179, 8);
                    icon = "⚠️";
                    break;

                default:
                    borderColor = Windows.UI.Color.FromArgb(255, 239, 68, 68);
                    accentColor = Windows.UI.Color.FromArgb(255, 239, 68, 68);
                    icon = "❌";
                    break;
            }

            SnackbarBorder.BorderBrush = new SolidColorBrush(borderColor);
            IconText.Text = icon;
            IconText.Foreground = new SolidColorBrush(accentColor);
        }

        private async Task AnimateInAsync()
        {
            // Prepare transform
            var transform = new TranslateTransform { Y = -50 };
            RootGrid.RenderTransform = transform;
            RootGrid.Opacity = 0;

            // Slide down and fade in
            var slideAnimation = new DoubleAnimation
            {
                From = -50,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(280),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            var fadeAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(280)
            };

            Storyboard.SetTarget(slideAnimation, transform);
            Storyboard.SetTargetProperty(slideAnimation, "Y");
            Storyboard.SetTarget(fadeAnimation, RootGrid);
            Storyboard.SetTargetProperty(fadeAnimation, "Opacity");

            var storyboard = new Storyboard();
            storyboard.Children.Add(slideAnimation);
            storyboard.Children.Add(fadeAnimation);
            storyboard.Begin();

            await Task.Delay(280);
        }

        private async Task AnimateOutAsync()
        {
            var transform = RootGrid.RenderTransform as TranslateTransform;
            if (transform == null)
            {
                transform = new TranslateTransform();
                RootGrid.RenderTransform = transform;
            }

            // Slide up and fade out
            var slideAnimation = new DoubleAnimation
            {
                From = 0,
                To = -50,
                Duration = TimeSpan.FromMilliseconds(280),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
            };

            var fadeAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(280)
            };

            Storyboard.SetTarget(slideAnimation, transform);
            Storyboard.SetTargetProperty(slideAnimation, "Y");
            Storyboard.SetTarget(fadeAnimation, RootGrid);
            Storyboard.SetTargetProperty(fadeAnimation, "Opacity");

            var storyboard = new Storyboard();
            storyboard.Children.Add(slideAnimation);
            storyboard.Children.Add(fadeAnimation);
            storyboard.Begin();

            await Task.Delay(280);
            RootGrid.Visibility = Visibility.Collapsed;
        }

        private void StartDismissTimer(int durationMs)
        {
            // Cancel existing timer if any
            _dismissTimer?.Stop();

            _dismissTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(durationMs)
            };

            _dismissTimer.Tick += async (s, e) =>
            {
                _dismissTimer.Stop();
                await DismissAsync();
            };

            _dismissTimer.Start();
        }

        private async Task DismissAsync()
        {
            _dismissTimer?.Stop();
            await AnimateOutAsync();
        }

        private async void OnCloseClick(object sender, RoutedEventArgs e)
        {
            await DismissAsync();
        }
    }
}
