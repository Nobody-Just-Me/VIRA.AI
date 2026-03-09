using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Graphics.Drawables;
using Android.Speech;
using System.Collections.Generic;

namespace VIRA.Mobile.Activities;

[Activity(Label = "Voice Active", Theme = "@style/AppTheme")]
public class VoiceActiveActivity : Activity
{
    private const int VOICE_RECOGNITION_REQUEST_CODE = 1001;
    private TextView? _statusText;
    private TextView? _transcriptionText;
    private TextView? _orbText;
    private View? _outerRing1;
    private View? _outerRing2;
    private TextView? _waveformText;
    private Button? _cancelButton;
    private Button? _sendButton;
    private Button? _tryAgainButton;
    private LinearLayout? _buttonContainer;
    private string _transcription = "";
    private string _currentState = "LISTENING";
    
    // Speech recognizer without dialog
    private SpeechRecognizer? _speechRecognizer;
    private Intent? _recognizerIntent;
    
    // Animation handlers
    private Android.OS.Handler? _animationHandler;
    private Java.Lang.Runnable? _orbPulseRunnable;
    private Java.Lang.Runnable? _ringPulseRunnable;
    private Java.Lang.Runnable? _waveformRunnable;
    private bool _isAnimating = false;
    
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        
        if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
        {
            Window?.SetStatusBarColor(Android.Graphics.Color.ParseColor("#101622"));
        }
        
        // Initialize animation handler
        _animationHandler = new Android.OS.Handler(Android.OS.Looper.MainLooper);
        
        BuildUI();
        StartAnimations();
        StartVoiceRecognition();
    }
    
    protected override void OnDestroy()
    {
        StopAnimations();
        
        // Clean up speech recognizer
        if (_speechRecognizer != null)
        {
            _speechRecognizer.Destroy();
            _speechRecognizer = null;
        }
        
        base.OnDestroy();
    }
    
    private void StartAnimations()
    {
        _isAnimating = true;
        StartOrbPulseAnimation();
        StartRingPulseAnimation();
        StartWaveformAnimation();
    }
    
    private void StopAnimations()
    {
        _isAnimating = false;
        
        if (_animationHandler != null)
        {
            if (_orbPulseRunnable != null)
                _animationHandler.RemoveCallbacks(_orbPulseRunnable);
            if (_ringPulseRunnable != null)
                _animationHandler.RemoveCallbacks(_ringPulseRunnable);
            if (_waveformRunnable != null)
                _animationHandler.RemoveCallbacks(_waveformRunnable);
        }
    }
    
    private void StartOrbPulseAnimation()
    {
        if (_orbText == null || _animationHandler == null) return;
        
        _orbPulseRunnable = new Java.Lang.Runnable(() =>
        {
            if (!_isAnimating || _orbText == null) return;
            
            // Pulse animation using scale
            _orbText.Animate()
                .ScaleX(1.1f)
                .ScaleY(1.1f)
                .SetDuration(800)
                .WithEndAction(new Java.Lang.Runnable(() =>
                {
                    if (_orbText != null)
                    {
                        _orbText.Animate()
                            .ScaleX(1.0f)
                            .ScaleY(1.0f)
                            .SetDuration(800)
                            .Start();
                    }
                }))
                .Start();
            
            // Repeat
            if (_isAnimating && _animationHandler != null && _orbPulseRunnable != null)
            {
                _animationHandler.PostDelayed(_orbPulseRunnable, 1600);
            }
        });
        
        _animationHandler.Post(_orbPulseRunnable);
    }
    
    private void StartRingPulseAnimation()
    {
        if (_outerRing1 == null || _outerRing2 == null || _animationHandler == null) return;
        
        _ringPulseRunnable = new Java.Lang.Runnable(() =>
        {
            if (!_isAnimating || _outerRing1 == null || _outerRing2 == null) return;
            
            // Ring 1 pulse
            _outerRing1.Animate()
                .ScaleX(1.15f)
                .ScaleY(1.15f)
                .Alpha(0.1f)
                .SetDuration(1200)
                .WithEndAction(new Java.Lang.Runnable(() =>
                {
                    if (_outerRing1 != null)
                    {
                        _outerRing1.Animate()
                            .ScaleX(1.0f)
                            .ScaleY(1.0f)
                            .Alpha(0.3f)
                            .SetDuration(1200)
                            .Start();
                    }
                }))
                .Start();
            
            // Ring 2 pulse (delayed)
            _animationHandler?.PostDelayed(new Java.Lang.Runnable(() =>
            {
                if (_outerRing2 != null)
                {
                    _outerRing2.Animate()
                        .ScaleX(1.12f)
                        .ScaleY(1.12f)
                        .Alpha(0.2f)
                        .SetDuration(1200)
                        .WithEndAction(new Java.Lang.Runnable(() =>
                        {
                            if (_outerRing2 != null)
                            {
                                _outerRing2.Animate()
                                    .ScaleX(1.0f)
                                    .ScaleY(1.0f)
                                    .Alpha(0.5f)
                                    .SetDuration(1200)
                                    .Start();
                            }
                        }))
                        .Start();
                }
            }), 600);
            
            // Repeat
            if (_isAnimating && _animationHandler != null && _ringPulseRunnable != null)
            {
                _animationHandler.PostDelayed(_ringPulseRunnable, 2400);
            }
        });
        
        _animationHandler.Post(_ringPulseRunnable);
    }
    
    private void StartWaveformAnimation()
    {
        if (_waveformText == null || _animationHandler == null) return;
        
        var waveforms = new[]
        {
            "▁ ▂ ▃ ▄ ▅ ▆ ▇ █ ▇ ▆ ▅ ▄ ▃ ▂ ▁",
            "▂ ▃ ▄ ▅ ▆ ▇ █ ▇ ▆ ▅ ▄ ▃ ▂ ▁ ▂",
            "▃ ▄ ▅ ▆ ▇ █ ▇ ▆ ▅ ▄ ▃ ▂ ▁ ▂ ▃",
            "▄ ▅ ▆ ▇ █ ▇ ▆ ▅ ▄ ▃ ▂ ▁ ▂ ▃ ▄",
            "▅ ▆ ▇ █ ▇ ▆ ▅ ▄ ▃ ▂ ▁ ▂ ▃ ▄ ▅",
            "▆ ▇ █ ▇ ▆ ▅ ▄ ▃ ▂ ▁ ▂ ▃ ▄ ▅ ▆",
            "▇ █ ▇ ▆ ▅ ▄ ▃ ▂ ▁ ▂ ▃ ▄ ▅ ▆ ▇",
            "█ ▇ ▆ ▅ ▄ ▃ ▂ ▁ ▂ ▃ ▄ ▅ ▆ ▇ █"
        };
        
        var currentIndex = 0;
        
        _waveformRunnable = new Java.Lang.Runnable(() =>
        {
            if (!_isAnimating || _waveformText == null) return;
            
            _waveformText.Text = waveforms[currentIndex % waveforms.Length];
            currentIndex++;
            
            // Repeat
            if (_isAnimating && _animationHandler != null && _waveformRunnable != null)
            {
                _animationHandler.PostDelayed(_waveformRunnable, 150);
            }
        });
        
        _animationHandler.Post(_waveformRunnable);
    }
    
    private void BuildUI()
    {
        var mainLayout = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.MatchParent)
        };
        
        // Gradient background
        var gradientDrawable = new GradientDrawable(
            GradientDrawable.Orientation.TopBottom,
            new int[] {
                Android.Graphics.Color.ParseColor("#101622"),
                Android.Graphics.Color.ParseColor("#1A8B5CF6")
            });
        mainLayout.Background = gradientDrawable;
        
        // Header
        var header = CreateHeader();
        mainLayout.AddView(header);
        
        // Center content
        var centerLayout = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                0)
            {
                Weight = 1
            }
        };
        centerLayout.SetGravity(GravityFlags.Center);
        
        // Orb container
        var orbContainer = new FrameLayout(this)
        {
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
        };
        
        // Outer ring 1
        _outerRing1 = new View(this)
        {
            LayoutParameters = new FrameLayout.LayoutParams(
                500,
                500)
            {
                Gravity = GravityFlags.Center
            }
        };
        var ring1Drawable = new GradientDrawable();
        ring1Drawable.SetShape(ShapeType.Oval);
        ring1Drawable.SetStroke(4, Android.Graphics.Color.ParseColor("#8B5CF6"));
        ring1Drawable.SetColor(Android.Graphics.Color.Transparent);
        _outerRing1.Background = ring1Drawable;
        _outerRing1.Alpha = 0.3f;
        orbContainer.AddView(_outerRing1);
        
        // Outer ring 2
        _outerRing2 = new View(this)
        {
            LayoutParameters = new FrameLayout.LayoutParams(
                400,
                400)
            {
                Gravity = GravityFlags.Center
            }
        };
        var ring2Drawable = new GradientDrawable();
        ring2Drawable.SetShape(ShapeType.Oval);
        ring2Drawable.SetStroke(4, Android.Graphics.Color.ParseColor("#8B5CF6"));
        ring2Drawable.SetColor(Android.Graphics.Color.Transparent);
        _outerRing2.Background = ring2Drawable;
        _outerRing2.Alpha = 0.5f;
        orbContainer.AddView(_outerRing2);
        
        // Main orb
        _orbText = new TextView(this)
        {
            Text = "V",
            TextSize = 72,
            LayoutParameters = new FrameLayout.LayoutParams(
                300,
                300)
            {
                Gravity = GravityFlags.Center
            }
        };
        _orbText.SetTextColor(Android.Graphics.Color.White);
        _orbText.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
        _orbText.Gravity = GravityFlags.Center;
        var orbDrawable = new GradientDrawable();
        orbDrawable.SetShape(ShapeType.Oval);
        var orbColors = new int[] {
            Android.Graphics.Color.ParseColor("#8B5CF6"),
            Android.Graphics.Color.ParseColor("#8B5CF6")
        };
        orbDrawable.SetColors(orbColors);
        _orbText.Background = orbDrawable;
        orbContainer.AddView(_orbText);
        
        centerLayout.AddView(orbContainer);
        
        // Waveform placeholder
        _waveformText = new TextView(this)
        {
            Text = "▁ ▂ ▃ ▄ ▅ ▆ ▇ █ ▇ ▆ ▅ ▄ ▃ ▂ ▁",
            TextSize = 20,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
            {
                TopMargin = 64
            }
        };
        _waveformText.SetTextColor(Android.Graphics.Color.ParseColor("#8B5CF6"));
        centerLayout.AddView(_waveformText);
        
        // Transcription text
        _transcriptionText = new TextView(this)
        {
            Text = "Listening...",
            TextSize = 24,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                TopMargin = 64,
                LeftMargin = 64,
                RightMargin = 64
            }
        };
        _transcriptionText.SetTextColor(Android.Graphics.Color.White);
        _transcriptionText.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
        _transcriptionText.Gravity = GravityFlags.Center;
        centerLayout.AddView(_transcriptionText);
        
        mainLayout.AddView(centerLayout);
        
        // Button container
        _buttonContainer = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                BottomMargin = 64
            }
        };
        _buttonContainer.SetGravity(GravityFlags.Center);
        _buttonContainer.SetPadding(64, 0, 64, 0);
        
        // Cancel button
        _cancelButton = new Button(this)
        {
            Text = "🔇 Cancel",
            LayoutParameters = new LinearLayout.LayoutParams(
                0,
                ViewGroup.LayoutParams.WrapContent)
            {
                Weight = 1
            }
        };
        var cancelDrawable = new GradientDrawable();
        cancelDrawable.SetColor(Android.Graphics.Color.ParseColor("#EF4444"));
        cancelDrawable.SetCornerRadius(60);
        _cancelButton.Background = cancelDrawable;
        _cancelButton.SetTextColor(Android.Graphics.Color.White);
        _cancelButton.SetAllCaps(false);
        _cancelButton.TextSize = 16;
        _cancelButton.SetPadding(0, 48, 0, 48);
        _cancelButton.Click += OnCancelClick;
        _buttonContainer.AddView(_cancelButton);
        
        // Send button (initially hidden)
        _sendButton = new Button(this)
        {
            Text = "Send to Vira",
            LayoutParameters = new LinearLayout.LayoutParams(
                0,
                ViewGroup.LayoutParams.WrapContent)
            {
                Weight = 1,
                LeftMargin = 16
            },
            Visibility = ViewStates.Gone
        };
        var sendDrawable = new GradientDrawable();
        sendDrawable.SetColor(Android.Graphics.Color.ParseColor("#8B5CF6"));
        sendDrawable.SetCornerRadius(60);
        _sendButton.Background = sendDrawable;
        _sendButton.SetTextColor(Android.Graphics.Color.White);
        _sendButton.SetAllCaps(false);
        _sendButton.TextSize = 16;
        _sendButton.SetPadding(0, 48, 0, 48);
        _sendButton.Click += OnSendClick;
        _buttonContainer.AddView(_sendButton);
        
        // Try Again button (initially hidden)
        _tryAgainButton = new Button(this)
        {
            Text = "Try Again",
            LayoutParameters = new LinearLayout.LayoutParams(
                0,
                ViewGroup.LayoutParams.WrapContent)
            {
                Weight = 1,
                LeftMargin = 16
            },
            Visibility = ViewStates.Gone
        };
        var tryAgainDrawable = new GradientDrawable();
        tryAgainDrawable.SetColor(Android.Graphics.Color.ParseColor("#8B5CF6"));
        tryAgainDrawable.SetCornerRadius(60);
        _tryAgainButton.Background = tryAgainDrawable;
        _tryAgainButton.SetTextColor(Android.Graphics.Color.White);
        _tryAgainButton.SetAllCaps(false);
        _tryAgainButton.TextSize = 16;
        _tryAgainButton.SetPadding(0, 48, 0, 48);
        _tryAgainButton.Click += OnTryAgainClick;
        _buttonContainer.AddView(_tryAgainButton);
        
        mainLayout.AddView(_buttonContainer);
        
        SetContentView(mainLayout);
    }
    
    private LinearLayout CreateHeader()
    {
        var header = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
        };
        header.SetPadding(32, 48, 32, 32);
        header.SetGravity(GravityFlags.CenterVertical);
        
        // Close button
        var closeButton = new Button(this)
        {
            Text = "✕",
            LayoutParameters = new LinearLayout.LayoutParams(
                100,
                100)
        };
        closeButton.SetBackgroundColor(Android.Graphics.Color.Transparent);
        closeButton.SetTextColor(Android.Graphics.Color.White);
        closeButton.TextSize = 24;
        closeButton.Click += OnCancelClick;
        header.AddView(closeButton);
        
        // Spacer
        var spacer = new View(this)
        {
            LayoutParameters = new LinearLayout.LayoutParams(0, 0) { Weight = 1 }
        };
        header.AddView(spacer);
        
        // Status indicator
        var statusContainer = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal
        };
        statusContainer.SetGravity(GravityFlags.CenterVertical);
        
        var statusDot = new View(this)
        {
            LayoutParameters = new LinearLayout.LayoutParams(16, 16)
            {
                RightMargin = 16
            }
        };
        var dotDrawable = new GradientDrawable();
        dotDrawable.SetShape(ShapeType.Oval);
        dotDrawable.SetColor(Android.Graphics.Color.ParseColor("#8B5CF6"));
        statusDot.Background = dotDrawable;
        statusContainer.AddView(statusDot);
        
        _statusText = new TextView(this)
        {
            Text = "● LISTENING",
            TextSize = 14
        };
        _statusText.SetTextColor(Android.Graphics.Color.ParseColor("#8B5CF6"));
        _statusText.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
        statusContainer.AddView(_statusText);
        
        header.AddView(statusContainer);
        
        // Options button
        var optionsButton = new Button(this)
        {
            Text = "⋯",
            LayoutParameters = new LinearLayout.LayoutParams(
                100,
                100)
        };
        optionsButton.SetBackgroundColor(Android.Graphics.Color.Transparent);
        optionsButton.SetTextColor(Android.Graphics.Color.White);
        optionsButton.TextSize = 24;
        header.AddView(optionsButton);
        
        return header;
    }
    
    private void StartVoiceRecognition()
    {
        // Create speech recognizer without dialog
        _speechRecognizer = SpeechRecognizer.CreateSpeechRecognizer(this);
        
        if (_speechRecognizer == null)
        {
            Toast.MakeText(this, "Voice recognition not available", ToastLength.Short)?.Show();
            Finish();
            return;
        }
        
        // Set up recognition listener
        _speechRecognizer.SetRecognitionListener(new VoiceRecognitionListener(this));
        
        // Create intent for recognition
        _recognizerIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
        _recognizerIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
        _recognizerIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);
        _recognizerIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);
        _recognizerIntent.PutExtra(RecognizerIntent.ExtraPartialResults, true);
        
        // Start listening (no dialog will appear)
        _speechRecognizer.StartListening(_recognizerIntent);
    }
    
    // Recognition listener class
    private class VoiceRecognitionListener : Java.Lang.Object, IRecognitionListener
    {
        private readonly VoiceActiveActivity _activity;
        
        public VoiceRecognitionListener(VoiceActiveActivity activity)
        {
            _activity = activity;
        }
        
        public void OnBeginningOfSpeech()
        {
            _activity.RunOnUiThread(() =>
            {
                if (_activity._statusText != null)
                    _activity._statusText.Text = "Listening...";
            });
        }
        
        public void OnBufferReceived(byte[]? buffer) { }
        
        public void OnEndOfSpeech()
        {
            _activity.RunOnUiThread(() =>
            {
                if (_activity._statusText != null)
                    _activity._statusText.Text = "Processing...";
            });
        }
        
        public void OnError(SpeechRecognizerError error)
        {
            _activity.RunOnUiThread(() =>
            {
                string errorMessage = error switch
                {
                    SpeechRecognizerError.NoMatch => "No speech detected. Please try again.",
                    SpeechRecognizerError.Network => "Network error. Please check your connection.",
                    SpeechRecognizerError.Audio => "Audio recording error.",
                    SpeechRecognizerError.Client => "Recognition cancelled.",
                    _ => "Recognition error. Please try again."
                };
                
                if (_activity._statusText != null)
                    _activity._statusText.Text = errorMessage;
                
                // Show try again button
                _activity.ShowTryAgainButton();
            });
        }
        
        public void OnEvent(int eventType, Bundle? @params) { }
        
        public void OnPartialResults(Bundle? partialResults)
        {
            if (partialResults == null) return;
            
            var matches = partialResults.GetStringArrayList(SpeechRecognizer.ResultsRecognition);
            if (matches != null && matches.Count > 0)
            {
                _activity.RunOnUiThread(() =>
                {
                    if (_activity._transcriptionText != null)
                        _activity._transcriptionText.Text = matches[0];
                });
            }
        }
        
        public void OnReadyForSpeech(Bundle? @params)
        {
            _activity.RunOnUiThread(() =>
            {
                if (_activity._statusText != null)
                    _activity._statusText.Text = "Speak now...";
            });
        }
        
        public void OnResults(Bundle? results)
        {
            if (results == null) return;
            
            var matches = results.GetStringArrayList(SpeechRecognizer.ResultsRecognition);
            if (matches != null && matches.Count > 0)
            {
                _activity._transcription = matches[0] ?? "";
                _activity.RunOnUiThread(() =>
                {
                    _activity.OnRecognitionComplete(_activity._transcription);
                });
            }
        }
        
        public void OnRmsChanged(float rmsdB)
        {
            // Update waveform based on audio level
            _activity.RunOnUiThread(() =>
            {
                _activity.UpdateWaveformFromRms(rmsdB);
            });
        }
    }
    
    private void UpdateWaveformFromRms(float rmsdB)
    {
        // Convert RMS to visual representation
        int bars = (int)((rmsdB + 2) / 2); // Normalize to 0-10 range
        bars = Math.Max(0, Math.Min(10, bars));
        
        string waveform = new string('|', bars) + new string('.', 10 - bars);
        
        if (_waveformText != null)
            _waveformText.Text = waveform;
    }
    
    private void ShowTryAgainButton()
    {
        if (_buttonContainer != null && _tryAgainButton != null)
        {
            _buttonContainer.Visibility = ViewStates.Visible;
            _tryAgainButton.Visibility = ViewStates.Visible;
            _sendButton!.Visibility = ViewStates.Gone;
        }
    }
    
    private void OnRecognitionComplete(string text)
    {
        _currentState = "DONE";
        
        // Stop animations
        StopAnimations();
        
        if (_statusText != null)
        {
            _statusText.Text = "● DONE";
            _statusText.SetTextColor(Android.Graphics.Color.ParseColor("#10B981"));
        }
        
        if (_transcriptionText != null)
        {
            _transcriptionText.Text = $"\"{text}\"";
        }
        
        // Change orb color to green
        if (_orbText != null)
        {
            var orbDrawable = new GradientDrawable();
            orbDrawable.SetShape(ShapeType.Oval);
            var orbColors = new int[] {
                Android.Graphics.Color.ParseColor("#10B981"),
                Android.Graphics.Color.ParseColor("#22C55E")
            };
            orbDrawable.SetColors(orbColors);
            _orbText.Background = orbDrawable;
            
            // Scale animation for completion
            _orbText.Animate()
                .ScaleX(1.2f)
                .ScaleY(1.2f)
                .SetDuration(300)
                .WithEndAction(new Java.Lang.Runnable(() =>
                {
                    if (_orbText != null)
                    {
                        _orbText.Animate()
                            .ScaleX(1.0f)
                            .ScaleY(1.0f)
                            .SetDuration(300)
                            .Start();
                    }
                }))
                .Start();
        }
        
        // Hide waveform
        if (_waveformText != null)
        {
            _waveformText.Animate()
                .Alpha(0f)
                .SetDuration(300)
                .Start();
        }
        
        // Show Send and Try Again buttons
        if (_cancelButton != null)
            _cancelButton.Visibility = ViewStates.Gone;
        
        if (_sendButton != null)
            _sendButton.Visibility = ViewStates.Visible;
        
        if (_tryAgainButton != null)
            _tryAgainButton.Visibility = ViewStates.Visible;
    }
    
    private void OnRecognitionFailed()
    {
        _currentState = "FAILED";
        
        // Stop animations
        StopAnimations();
        
        if (_statusText != null)
        {
            _statusText.Text = "● FAILED";
            _statusText.SetTextColor(Android.Graphics.Color.ParseColor("#EF4444"));
        }
        
        if (_transcriptionText != null)
        {
            _transcriptionText.Text = "Could not recognize speech. Please try again.";
        }
        
        // Change orb color to red
        if (_orbText != null)
        {
            var orbDrawable = new GradientDrawable();
            orbDrawable.SetShape(ShapeType.Oval);
            var orbColors = new int[] {
                Android.Graphics.Color.ParseColor("#EF4444"),
                Android.Graphics.Color.ParseColor("#DC2626")
            };
            orbDrawable.SetColors(orbColors);
            _orbText.Background = orbDrawable;
            
            // Shake animation for error
            _orbText.Animate()
                .TranslationX(-20f)
                .SetDuration(100)
                .WithEndAction(new Java.Lang.Runnable(() =>
                {
                    if (_orbText != null)
                    {
                        _orbText.Animate()
                            .TranslationX(20f)
                            .SetDuration(100)
                            .WithEndAction(new Java.Lang.Runnable(() =>
                            {
                                if (_orbText != null)
                                {
                                    _orbText.Animate()
                                        .TranslationX(0f)
                                        .SetDuration(100)
                                        .Start();
                                }
                            }))
                            .Start();
                    }
                }))
                .Start();
        }
        
        // Hide waveform
        if (_waveformText != null)
        {
            _waveformText.Animate()
                .Alpha(0f)
                .SetDuration(300)
                .Start();
        }
        
        // Show Try Again button
        if (_cancelButton != null)
            _cancelButton.Visibility = ViewStates.Gone;
        
        if (_tryAgainButton != null)
            _tryAgainButton.Visibility = ViewStates.Visible;
    }
    
    private void OnSendClick(object? sender, System.EventArgs e)
    {
        var resultIntent = new Intent();
        resultIntent.PutExtra("transcription", _transcription);
        SetResult(Result.Ok, resultIntent);
        Finish();
    }
    
    private void OnCancelClick(object? sender, System.EventArgs e)
    {
        SetResult(Result.Canceled);
        Finish();
    }
    
    private void OnTryAgainClick(object? sender, System.EventArgs e)
    {
        // Reset UI
        _currentState = "LISTENING";
        
        if (_statusText != null)
        {
            _statusText.Text = "● LISTENING";
            _statusText.SetTextColor(Android.Graphics.Color.ParseColor("#8B5CF6"));
        }
        
        if (_transcriptionText != null)
        {
            _transcriptionText.Text = "Listening...";
        }
        
        // Reset orb color
        if (_orbText != null)
        {
            var orbDrawable = new GradientDrawable();
            orbDrawable.SetShape(ShapeType.Oval);
            var orbColors = new int[] {
                Android.Graphics.Color.ParseColor("#8B5CF6"),
                Android.Graphics.Color.ParseColor("#8B5CF6")
            };
            orbDrawable.SetColors(orbColors);
            _orbText.Background = orbDrawable;
        }
        
        // Show waveform
        if (_waveformText != null)
        {
            _waveformText.Alpha = 1f;
        }
        
        if (_cancelButton != null)
            _cancelButton.Visibility = ViewStates.Visible;
        
        if (_sendButton != null)
            _sendButton.Visibility = ViewStates.Gone;
        
        if (_tryAgainButton != null)
            _tryAgainButton.Visibility = ViewStates.Gone;
        
        // Restart animations
        StartAnimations();
        
        // Start recognition again
        StartVoiceRecognition();
    }
}
