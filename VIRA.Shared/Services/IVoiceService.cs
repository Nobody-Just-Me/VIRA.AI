namespace VIRA.Shared.Services;

public interface IVoiceService
{
    Task<string> RecognizeSpeechAsync();
    Task SpeakAsync(string text);
    bool IsAvailable { get; }
}
