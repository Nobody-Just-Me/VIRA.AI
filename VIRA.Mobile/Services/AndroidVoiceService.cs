#if __ANDROID__
using Android.Content;
using Android.Speech;
using Android.Speech.Tts;
using Android.Media;
using VIRA.Shared.Services;

namespace VIRA.Mobile.Services;

public class AndroidVoiceService : Java.Lang.Object, IVoiceService, TextToSpeech.IOnInitListener
{
    private TextToSpeech? _textToSpeech;
    private TaskCompletionSource<string>? _speechRecognitionTcs;
    private bool _isInitialized = false;
    private MediaPlayer? _mediaPlayer;

    public bool IsAvailable => _isInitialized;

    public AndroidVoiceService()
    {
        InitializeTextToSpeech();
    }

    private void InitializeTextToSpeech()
    {
        try
        {
            var context = Android.App.Application.Context;
            _textToSpeech = new TextToSpeech(context, this);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"TTS Init Error: {ex.Message}");
        }
    }

    public void OnInit(OperationResult status)
    {
        if (status == OperationResult.Success && _textToSpeech != null)
        {
            // Set Indonesian locale
            var locale = new Java.Util.Locale("id", "ID");
            var result = _textToSpeech.SetLanguage(locale);

            if (result == LanguageAvailableResult.Available || 
                result == LanguageAvailableResult.CountryAvailable)
            {
                // Configure voice for female-like characteristics
                _textToSpeech.SetPitch(1.1f);  // Slightly higher pitch
                _textToSpeech.SetSpeechRate(0.9f);  // Slightly slower for clarity
                
                _isInitialized = true;
                System.Diagnostics.Debug.WriteLine("TTS initialized successfully");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Indonesian language not available: {result}");
            }
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("TTS initialization failed");
        }
    }

    public async Task<string> RecognizeSpeechAsync()
    {
        _speechRecognitionTcs = new TaskCompletionSource<string>();

        try
        {
            var context = Android.App.Application.Context;
            var intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            intent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            intent.PutExtra(RecognizerIntent.ExtraLanguage, "id-ID");
            intent.PutExtra(RecognizerIntent.ExtraPrompt, "Katakan sesuatu...");
            intent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);

            // Start speech recognition activity
            var activity = GetCurrentActivity();
            if (activity != null)
            {
                activity.StartActivityForResult(intent, 10);
            }
            else
            {
                throw new InvalidOperationException("Cannot get current activity");
            }

            return await _speechRecognitionTcs.Task;
        }
        catch (Exception ex)
        {
            _speechRecognitionTcs?.TrySetException(ex);
            throw;
        }
    }

    public Task SpeakAsync(string text)
    {
        if (!_isInitialized || _textToSpeech == null)
        {
            return Task.CompletedTask;
        }

        try
        {
            // Clean text for better speech
            var cleanText = CleanTextForSpeech(text);
            
            _textToSpeech.Speak(cleanText, QueueMode.Flush, null, null);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"TTS Speak Error: {ex.Message}");
            return Task.CompletedTask;
        }
    }

    private string CleanTextForSpeech(string text)
    {
        // Remove markdown formatting
        text = System.Text.RegularExpressions.Regex.Replace(text, @"\*\*(.*?)\*\*", "$1");
        text = System.Text.RegularExpressions.Regex.Replace(text, @"\*(.*?)\*", "$1");
        
        // Remove emojis (optional - TTS might handle them)
        // text = System.Text.RegularExpressions.Regex.Replace(text, @"[\u00A0-\uFFFF]+", "");
        
        return text;
    }

    private Android.App.Activity? GetCurrentActivity()
    {
        // Get the current activity from Uno Platform's context
        // The MainActivity is the current activity in Uno Platform Android apps
        var activity = Uno.UI.ContextHelper.Current as Android.App.Activity;
        return activity;
    }

    public void HandleActivityResult(int requestCode, Android.App.Result resultCode, Intent? data)
    {
        if (requestCode == 10 && resultCode == Android.App.Result.Ok && data != null)
        {
            var matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
            if (matches != null && matches.Count > 0)
            {
                var recognizedText = matches[0] ?? string.Empty;
                _speechRecognitionTcs?.TrySetResult(recognizedText);
            }
            else
            {
                _speechRecognitionTcs?.TrySetResult(string.Empty);
            }
        }
        else
        {
            _speechRecognitionTcs?.TrySetResult(string.Empty);
        }
    }

    public async Task PlayAudioAsync(byte[] audioData)
    {
        try
        {
            Android.Util.Log.Info("VIRA_Voice", $"🔊 Playing audio (size: {audioData.Length} bytes)");
            
            // Stop any currently playing audio
            StopAudio();
            
            // Create temp file for audio
            var tempPath = System.IO.Path.Combine(
                Android.App.Application.Context.CacheDir?.AbsolutePath ?? "/data/local/tmp",
                $"vira_tts_{Guid.NewGuid()}.mp3"
            );
            
            // Write audio data to file
            await System.IO.File.WriteAllBytesAsync(tempPath, audioData);
            Android.Util.Log.Info("VIRA_Voice", $"✅ Audio saved to: {tempPath}");
            
            // Play audio using MediaPlayer
            _mediaPlayer = new MediaPlayer();
            _mediaPlayer.SetDataSource(tempPath);
            _mediaPlayer.Prepare();
            
            var tcs = new TaskCompletionSource<bool>();
            
            _mediaPlayer.Completion += (sender, e) =>
            {
                Android.Util.Log.Info("VIRA_Voice", "✅ Audio playback completed");
                
                // Cleanup
                _mediaPlayer?.Release();
                _mediaPlayer = null;
                
                // Delete temp file
                try
                {
                    if (System.IO.File.Exists(tempPath))
                    {
                        System.IO.File.Delete(tempPath);
                    }
                }
                catch { }
                
                tcs.TrySetResult(true);
            };
            
            _mediaPlayer.Error += (sender, e) =>
            {
                Android.Util.Log.Error("VIRA_Voice", $"❌ Audio playback error: {e.What}");
                
                // Cleanup
                _mediaPlayer?.Release();
                _mediaPlayer = null;
                
                tcs.TrySetResult(false);
            };
            
            _mediaPlayer.Start();
            Android.Util.Log.Info("VIRA_Voice", "▶️ Audio playback started");
            
            await tcs.Task;
        }
        catch (Exception ex)
        {
            Android.Util.Log.Error("VIRA_Voice", $"❌ Error playing audio: {ex.Message}");
            throw;
        }
    }
    
    public void StopAudio()
    {
        try
        {
            if (_mediaPlayer != null)
            {
                if (_mediaPlayer.IsPlaying)
                {
                    _mediaPlayer.Stop();
                }
                _mediaPlayer.Release();
                _mediaPlayer = null;
                
                Android.Util.Log.Info("VIRA_Voice", "⏹️ Audio playback stopped");
            }
        }
        catch (Exception ex)
        {
            Android.Util.Log.Error("VIRA_Voice", $"Error stopping audio: {ex.Message}");
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            StopAudio();
            _textToSpeech?.Stop();
            _textToSpeech?.Shutdown();
            _textToSpeech?.Dispose();
        }
        base.Dispose(disposing);
    }
}
#endif
