namespace VIRA.Shared.Services;

public interface IVoiceService
{
    Task<string> RecognizeSpeechAsync();
    Task SpeakAsync(string text);
    Task PlayAudioAsync(byte[] audioData);
    void StopAudio();
    bool IsAvailable { get; }
}
