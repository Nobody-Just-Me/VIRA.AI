using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VIRA.Shared.Models;

namespace VIRA.Shared.Views
{
    public sealed partial class ChatHistorySidebar : UserControl, INotifyPropertyChanged
    {
        private bool _isOpen;
        private ObservableCollection<ChatSession> _chatSessions = new();

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<ChatSession>? ChatSelected;
        public event EventHandler? NewChatRequested;
        public event EventHandler? ClearHistoryRequested;

        public bool IsOpen
        {
            get => _isOpen;
            private set
            {
                if (_isOpen != value)
                {
                    _isOpen = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<ChatSession> ChatSessions
        {
            get => _chatSessions;
            set
            {
                if (_chatSessions != value)
                {
                    _chatSessions = value;
                    OnPropertyChanged();
                }
            }
        }

        public ChatHistorySidebar()
        {
            this.InitializeComponent();
            
            // Load sample chat sessions for demo
            LoadSampleSessions();
        }

        private void LoadSampleSessions()
        {
            _chatSessions.Add(new ChatSession
            {
                Id = "1",
                Title = "Weather and Traffic",
                Timestamp = DateTime.Now.AddHours(-2),
                Messages = new System.Collections.Generic.List<Message>()
            });
            
            _chatSessions.Add(new ChatSession
            {
                Id = "2",
                Title = "Morning Briefing",
                Timestamp = DateTime.Now.AddDays(-1),
                Messages = new System.Collections.Generic.List<Message>()
            });
            
            _chatSessions.Add(new ChatSession
            {
                Id = "3",
                Title = "Task Management",
                Timestamp = DateTime.Now.AddDays(-2),
                Messages = new System.Collections.Generic.List<Message>()
            });
        }

        public async System.Threading.Tasks.Task ShowAsync()
        {
            IsOpen = true;
            RootGrid.Visibility = Visibility.Visible;
            
            // Prepare transform
            var transform = new TranslateTransform { X = -300 };
            SidebarPanel.RenderTransform = transform;
            
            // Fade in overlay
            Overlay.Opacity = 0;
            var overlayFade = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            
            Storyboard.SetTarget(overlayFade, Overlay);
            Storyboard.SetTargetProperty(overlayFade, "Opacity");
            
            // Slide in animation
            var slideAnimation = new DoubleAnimation
            {
                From = -300,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            
            Storyboard.SetTarget(slideAnimation, transform);
            Storyboard.SetTargetProperty(slideAnimation, "X");
            
            var storyboard = new Storyboard();
            storyboard.Children.Add(overlayFade);
            storyboard.Children.Add(slideAnimation);
            storyboard.Begin();
            
            await System.Threading.Tasks.Task.Delay(300);
        }

        public async System.Threading.Tasks.Task HideAsync()
        {
            var transform = SidebarPanel.RenderTransform as TranslateTransform;
            if (transform == null)
            {
                transform = new TranslateTransform();
                SidebarPanel.RenderTransform = transform;
            }
            
            // Fade out overlay
            var overlayFade = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
            };
            
            Storyboard.SetTarget(overlayFade, Overlay);
            Storyboard.SetTargetProperty(overlayFade, "Opacity");
            
            // Slide out animation
            var slideAnimation = new DoubleAnimation
            {
                From = 0,
                To = -300,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
            };
            
            Storyboard.SetTarget(slideAnimation, transform);
            Storyboard.SetTargetProperty(slideAnimation, "X");
            
            var storyboard = new Storyboard();
            storyboard.Children.Add(overlayFade);
            storyboard.Children.Add(slideAnimation);
            storyboard.Begin();
            
            await System.Threading.Tasks.Task.Delay(300);
            
            IsOpen = false;
            RootGrid.Visibility = Visibility.Collapsed;
        }

        private async void OnOverlayTapped(object sender, TappedRoutedEventArgs e)
        {
            await HideAsync();
        }

        private void OnNewChatClick(object sender, RoutedEventArgs e)
        {
            NewChatRequested?.Invoke(this, EventArgs.Empty);
        }

        private void OnChatItemTapped(object sender, TappedRoutedEventArgs e)
        {
            if (sender is Border border && border.Tag is ChatSession session)
            {
                ChatSelected?.Invoke(this, session);
            }
        }

        private async void OnClearHistoryClick(object sender, RoutedEventArgs e)
        {
            // Show confirmation dialog
            var dialog = new ContentDialog
            {
                Title = "Clear History",
                Content = "Are you sure you want to clear all chat history? This action cannot be undone.",
                PrimaryButtonText = "Clear",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync();
            
            if (result == ContentDialogResult.Primary)
            {
                ClearHistoryRequested?.Invoke(this, EventArgs.Empty);
                _chatSessions.Clear();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
