using VIRA.Mobile.SharedModels;

namespace VIRA.Mobile.SharedServices;

/// <summary>
/// Rule-based message processor using pattern matching
/// Implements the first stage of hybrid processing (rule-based → AI fallback)
/// </summary>
public class RuleBasedProcessor : IMessageProcessor
{
    private readonly PatternRegistry _patternRegistry;
    private const float CONFIDENCE_THRESHOLD = 0.8f;

    public RuleBasedProcessor(PatternRegistry patternRegistry)
    {
        _patternRegistry = patternRegistry;
    }

    /// <summary>
    /// Process a message using rule-based pattern matching
    /// </summary>
    /// <param name="message">User input message</param>
    /// <param name="context">Conversation context</param>
    /// <returns>Processing result with response, action, and confidence</returns>
    public async Task<ProcessingResult> ProcessMessageAsync(string message, ConversationContext context)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(message))
        {
            return new ErrorResult(
                errorMessage: "Empty message",
                fallbackResponse: "Maaf, saya tidak menangkap pesan Anda. Silakan coba lagi."
            );
        }

        // Try to find a matching pattern
        var patternMatch = _patternRegistry.FindMatch(message);

        if (patternMatch == null)
        {
            // No pattern matched - return low confidence result
            return new RuleBasedResult(
                response: "Maaf, saya tidak mengerti perintah tersebut. Coba tanyakan 'bantuan' untuk melihat perintah yang tersedia.",
                action: null,
                confidence: 0.0f
            );
        }

        try
        {
            // Execute the matched pattern's handler
            var commandResult = await patternMatch.Pattern.Handler.HandleAsync(
                patternMatch.Match,
                context
            );

            // Convert CommandResult to RuleBasedResult
            return new RuleBasedResult(
                response: commandResult.Response,
                action: commandResult.Action,
                confidence: commandResult.Confidence
            );
        }
        catch (Exception ex)
        {
            // Handler execution failed
            return new ErrorResult(
                errorMessage: $"Handler execution failed: {ex.Message}",
                fallbackResponse: "Maaf, terjadi kesalahan saat memproses perintah Anda. Silakan coba lagi."
            );
        }
    }

    /// <summary>
    /// Check if the confidence score meets the threshold for rule-based processing
    /// </summary>
    /// <param name="confidence">Confidence score from pattern matching</param>
    /// <returns>True if confidence is above threshold</returns>
    public static bool MeetsConfidenceThreshold(float confidence)
    {
        return confidence >= CONFIDENCE_THRESHOLD;
    }

    /// <summary>
    /// Get the confidence threshold value
    /// </summary>
    public static float GetConfidenceThreshold()
    {
        return CONFIDENCE_THRESHOLD;
    }
}
