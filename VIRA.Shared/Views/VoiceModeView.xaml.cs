using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VIRA.Shared.Services;

namespace VIRA.Shared.Views
{
    public sealed partial class VoiceModeView : Page, INotifyPropertyChanged
    {
        private string _transcript = "Tap the microphone to start speaking...";
        private string _statusMessage = "Ready";
        private bool _isListening;
        private bool _isSpeaking;
        private readonly IVoiceService? _voiceService;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Transcript
        {
            get => _transcript;
            set
            {
                if (_transcript != value)
                {
                    _transcript = value;
                    OnPropertyChanged();
                }
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsListening
        {
            get => _isListening;
            set
            {
                if (_isListening != value)
                {
                    _isListening = value;
                    UpdateVoiceButtonState();
                    OnPropertyChanged();
                }
            }
        }

        public bool IsSpeaking
        {
            get => _isSpeaking;
            set
            {
                if (_isSpeaking != value)
                {
                    _isSpeaking = value;
                    UpdateVoiceButtonState();
                    OnPropertyChanged();
                }
            }
        }

        public VoiceModeView()
        {
            this.InitializeComponent();
            
            // Get voice service from dependency injection
            // For now, we'll handle the case where it might not be available
            try
            {
                _voiceService = App.Current.Services?.GetService(typeof(IVoiceService)) as IVoiceService;
            }
            catch
            {
                _voiceService = null;
            }
        }

        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            // Navigate back to main chat
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private void OnVoiceButtonClick(object sender, RoutedEventArgs e)
        {
            if (IsListening)
            {
                StopListening();
            }
            else
            {
                StartListening();
            }
        }

        private async void StartListening()
        {
            IsListening = true;
            StatusMessage = "Listening...";
            Transcript = "";
            
            // Update waveform state
            if (Waveform != null)
            {
                Waveform.IsActive = true;
                Waveform.IsSpeaking = false;
            }
            
            // Use actual speech recognition if available
            if (_voiceService != null && _voiceService.IsAvailable)
            {
                try
                {
                    var recognizedText = await _voiceService.RecognizeSpeechAsync();
                    
                    if (!string.IsNullOrWhiteSpace(recognizedText))
                    {
                        Transcript = recognizedText;
                        StatusMessage = "Processing...";
                        
                        // Stop listening and process
                        await System.Threading.Tasks.Task.Delay(500);
                        StopListening();
                        
                        // Simulate AI response
                        await SimulateAIResponse();
                    }
                    else
                    {
                        Transcript = "No speech detected. Please try again.";
                        StatusMessage = "Ready";
                        IsListening = false;
                    }
                }
                catch (Exception ex)
                {
                    Transcript = $"Error: {ex.Message}";
                    StatusMessage = "Error occurred";
                    IsListening = false;
                }
            }
            else
            {
                // Fallback to simulation if voice service not available
                SimulateSpeechRecognition();
            }
        }

        private void StopListening()
        {
            IsListening = false;
            StatusMessage = "Processing...";
            
            // Update waveform state
            if (Waveform != null)
            {
                Waveform.IsActive = false;
            }
            
            // TODO: Process the transcript and get AI response
        }

        private async void SimulateSpeechRecognition()
        {
            // Simulate speech recognition for demo purposes
            await System.Threading.Tasks.Task.Delay(2000);
            
            if (IsListening)
            {
                Transcript = "What's the weather like today?";
                await System.Threading.Tasks.Task.Delay(1000);
                StopListening();
                
                // Simulate AI response
                await SimulateAIResponse();
            }
        }
        
        private async System.Threading.Tasks.Task SimulateAIResponse()
        {
            // Simulate AI speaking
            IsSpeaking = true;
            StatusMessage = "Speaking...";
            
            await System.Threading.Tasks.Task.Delay(3000);
            
            IsSpeaking = false;
            StatusMessage = "Ready";
        }

        private void UpdateVoiceButtonState()
        {
            if (VoiceButton == null) return;

            var brush = new SolidColorBrush();
            
            if (IsListening)
            {
                // Red when recording
                brush.Color = Windows.UI.Color.FromArgb(255, 239, 68, 68);
                StartPulseAnimation();
            }
            else if (IsSpeaking)
            {
                // Purple when AI is speaking
                brush.Color = Windows.UI.Color.FromArgb(255, 139, 92, 246);
                
                // Update waveform
                if (Waveform != null)
                {
                    Waveform.IsSpeaking = true;
                }
            }
            else
            {
                // Purple when idle
                brush.Color = Windows.UI.Color.FromArgb(255, 139, 92, 246);
                StopPulseAnimation();
                
                // Update waveform
                if (Waveform != null)
                {
                    Waveform.IsActive = false;
                    Waveform.IsSpeaking = false;
                }
            }
            
            VoiceButton.Background = brush;
        }

        private Storyboard _pulseStoryboard;

        private void StartPulseAnimation()
        {
            if (_pulseStoryboard != null)
            {
                _pulseStoryboard.Stop();
            }

            var scaleTransform = new ScaleTransform
            {
                CenterX = 40,
                CenterY = 40
            };
            VoiceButton.RenderTransform = scaleTransform;

            var scaleXAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = 1.1,
                Duration = TimeSpan.FromMilliseconds(500),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever,
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
            };

            var scaleYAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = 1.1,
                Duration = TimeSpan.FromMilliseconds(500),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever,
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
            };

            Storyboard.SetTarget(scaleXAnimation, scaleTransform);
            Storyboard.SetTargetProperty(scaleXAnimation, "ScaleX");
            Storyboard.SetTarget(scaleYAnimation, scaleTransform);
            Storyboard.SetTargetProperty(scaleYAnimation, "ScaleY");

            _pulseStoryboard = new Storyboard();
            _pulseStoryboard.Children.Add(scaleXAnimation);
            _pulseStoryboard.Children.Add(scaleYAnimation);
            _pulseStoryboard.Begin();
        }

        private void StopPulseAnimation()
        {
            if (_pulseStoryboard != null)
            {
                _pulseStoryboard.Stop();
                _pulseStoryboard = null;
            }

            VoiceButton.RenderTransform = null;
        }

        private void OnMessageClick(object sender, RoutedEventArgs e)
        {
            // Navigate back to message view with transcript
            if (Frame.CanGoBack)
            {
                // Store transcript for retrieval by MainChatView
                if (!string.IsNullOrWhiteSpace(Transcript) && 
                    Transcript != "Tap the microphone to start speaking...")
                {
                    // Use navigation parameter to pass transcript back
                    Frame.GoBack();
                    // Note: In a production app, we'd use a proper navigation service
                    // or event aggregator to pass data between views
                }
                else
                {
                    Frame.GoBack();
                }
            }
        }

        private void OnVolumeClick(object sender, RoutedEventArgs e)
        {
            // TODO: Implement volume control
            StatusMessage = "Volume control not yet implemented";
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
