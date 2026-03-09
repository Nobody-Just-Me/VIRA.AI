namespace VIRA.Shared.Services;

public interface ITTSService
{
    void SetApiKey(string apiKey);
    Task<byte[]> SynthesizeSpeechAsync(string text);
    Task<bool> TestConnectionAsync();
}
