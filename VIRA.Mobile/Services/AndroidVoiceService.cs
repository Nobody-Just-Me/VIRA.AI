#if __ANDROID__
using Android.Content;
using Android.Speech;
using Android.Speech.Tts;
using VIRA.Shared.Services;

namespace VIRA.Mobile.Services;

public class AndroidVoiceService : Java.Lang.Object, IVoiceService, TextToSpeech.IOnInitListener
{
    private TextToSpeech? _textToSpeech;
    private TaskCompletionSource<string>? _speechRecognitionTcs;
    private bool _isInitialized = false;

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
        // This needs to be implemented based on your app's activity management
        // For Uno Platform, you can access it via:
        return Microsoft.UI.Xaml.Window.Current?.Content?.XamlRoot?.Content as Android.App.Activity;
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

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _textToSpeech?.Stop();
            _textToSpeech?.Shutdown();
            _textToSpeech?.Dispose();
        }
        base.Dispose(disposing);
    }
}
#endif
