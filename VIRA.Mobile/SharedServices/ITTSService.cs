namespace VIRA.Mobile.SharedServices;

public interface ITTSService
{
    void SetApiKey(string apiKey);
    void SetVoiceId(string voiceId);
    Task<byte[]> SynthesizeSpeechAsync(string text);
    Task<bool> TestConnectionAsync();
}
