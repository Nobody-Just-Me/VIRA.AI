using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;

namespace VIRA.Shared.Views
{
    public sealed partial class LoadingIndicator : UserControl
    {
        private Storyboard? _animationStoryboard;

        public LoadingIndicator()
        {
            this.InitializeComponent();
        }

        public void Show(string message = "Loading...")
        {
            LoadingText.Text = message;
            RootGrid.Visibility = Visibility.Visible;
            StartAnimation();
        }

        public void Hide()
        {
            StopAnimation();
            RootGrid.Visibility = Visibility.Collapsed;
        }

        private void StartAnimation()
        {
            // Create pulsing animation for dots
            _animationStoryboard = new Storyboard
            {
                RepeatBehavior = RepeatBehavior.Forever
            };

            // Animate each dot with a delay
            AnimateDot(Dot1, 0);
            AnimateDot(Dot2, 200);
            AnimateDot(Dot3, 400);

            _animationStoryboard.Begin();
        }

        private void AnimateDot(UIElement dot, int delayMs)
        {
            var opacityAnimation = new DoubleAnimation
            {
                From = 0.3,
                To = 1.0,
                Duration = TimeSpan.FromMilliseconds(600),
                AutoReverse = true,
                BeginTime = TimeSpan.FromMilliseconds(delayMs),
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
            };

            Storyboard.SetTarget(opacityAnimation, dot);
            Storyboard.SetTargetProperty(opacityAnimation, "Opacity");

            _animationStoryboard?.Children.Add(opacityAnimation);
        }

        private void StopAnimation()
        {
            _animationStoryboard?.Stop();
            _animationStoryboard = null;
        }
    }
}
