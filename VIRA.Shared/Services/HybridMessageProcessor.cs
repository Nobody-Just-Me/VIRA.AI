using VIRA.Shared.Models;

namespace VIRA.Shared.Services;

/// <summary>
/// Hybrid message processor that combines rule-based and AI-enhanced processing
/// Implements two-stage processing: rule-based first, then AI fallback for complex queries
/// Requirements: AC-16.10, Hybrid Processing Flow
/// </summary>
public class HybridMessageProcessor : IMessageProcessor
{
    private readonly RuleBasedProcessor _ruleBasedProcessor;
    private readonly IGeminiService _geminiService;
    private readonly IGeminiService _groqService;
    private readonly IGeminiService _openaiService;
    private readonly IPreferencesService _preferencesService;
    private const float CONFIDENCE_THRESHOLD = 0.8f;

    public HybridMessageProcessor(
        RuleBasedProcessor ruleBasedProcessor,
        IGeminiService geminiService,
        IGeminiService groqService,
        IGeminiService openaiService,
        IPreferencesService preferencesService)
    {
        _ruleBasedProcessor = ruleBasedProcessor;
        _geminiService = geminiService;
        _groqService = groqService;
        _openaiService = openaiService;
        _preferencesService = preferencesService;
    }

    /// <summary>
    /// Process a message using hybrid two-stage approach:
    /// 1. Try rule-based processing first (fast, offline-capable)
    /// 2. If confidence < 0.8, fallback to AI processing (if configured)
    /// 3. If AI fails or not configured, return helpful error message
    /// </summary>
    public async Task<ProcessingResult> ProcessMessageAsync(string message, ConversationContext context)
    {
        #if __ANDROID__
        Android.Util.Log.Info("VIRA_Hybrid", $"Processing message: {message}");
        #endif
        
        // Step 1: Try rule-based processing first
        #if __ANDROID__
        Android.Util.Log.Info("VIRA_Hybrid", "Step 1: Attempting rule-based processing...");
        #endif
        var ruleResult = await _ruleBasedProcessor.ProcessMessageAsync(message, context);
        
        // Check if rule-based processing was successful with high confidence
        if (ruleResult is RuleBasedResult ruleBasedResult)
        {
            #if __ANDROID__
            Android.Util.Log.Info("VIRA_Hybrid", $"Rule-based confidence: {ruleBasedResult.Confidence}");
            #endif
            
            if (ruleBasedResult.Confidence >= CONFIDENCE_THRESHOLD)
            {
                #if __ANDROID__
                Android.Util.Log.Info("VIRA_Hybrid", "✅ Rule-based processing successful (high confidence)");
                #endif
                return ruleBasedResult;
            }
            
            #if __ANDROID__
            Android.Util.Log.Info("VIRA_Hybrid", "⚠️ Rule-based confidence below threshold, attempting AI fallback...");
            #endif
        }
        
        // Step 2: Check if AI is configured
        #if __ANDROID__
        Android.Util.Log.Info("VIRA_Hybrid", "Step 2: Checking AI configuration...");
        #endif
        var aiConfig = GetAIConfiguration();
        
        if (!aiConfig.IsConfigured)
        {
            #if __ANDROID__
            Android.Util.Log.Warn("VIRA_Hybrid", "❌ AI not configured, returning error with fallback");
            #endif
            return new ErrorResult(
                errorMessage: "AI not configured",
                fallbackResponse: GenerateHelpMessage(ruleResult)
            );
        }
        
        #if __ANDROID__
        Android.Util.Log.Info("VIRA_Hybrid", $"✅ AI configured: Provider={aiConfig.Provider}");
        #endif
        
        // Step 3: Use AI for complex queries
        #if __ANDROID__
        Android.Util.Log.Info("VIRA_Hybrid", "Step 3: Attempting AI processing...");
        #endif
        try
        {
            var aiResult = await ProcessWithAI(message, context, aiConfig);
            #if __ANDROID__
            Android.Util.Log.Info("VIRA_Hybrid", "✅ AI processing successful");
            #endif
            return aiResult;
        }
        catch (Exception ex)
        {
            #if __ANDROID__
            Android.Util.Log.Error("VIRA_Hybrid", $"❌ AI processing failed: {ex.Message}");
            #endif
            
            // Fallback to rule-based response if available
            if (ruleResult is RuleBasedResult fallbackResult && !string.IsNullOrEmpty(fallbackResult.Response))
            {
                #if __ANDROID__
                Android.Util.Log.Info("VIRA_Hybrid", "Using rule-based response as fallback");
                #endif
                return new ErrorResult(
                    errorMessage: $"AI processing failed: {ex.Message}",
                    fallbackResponse: fallbackResult.Response
                );
            }
            
            return new ErrorResult(
                errorMessage: $"AI processing failed: {ex.Message}",
                fallbackResponse: "Maaf, saya tidak dapat memproses permintaan Anda saat ini. Silakan coba lagi atau periksa koneksi internet Anda."
            );
        }
    }

    /// <summary>
    /// Get AI configuration from preferences
    /// </summary>
    private AIConfiguration GetAIConfiguration()
    {
        try
        {
            // Get AI provider from Android SharedPreferences
#if __ANDROID__
            var context = Android.App.Application.Context;
            var prefs = context?.GetSharedPreferences("vira_settings", Android.Content.FileCreationMode.Private);
            
            var provider = prefs?.GetString("ai_provider", "gemini") ?? "gemini";
            var apiKey = provider.ToLower() switch
            {
                "groq" => prefs?.GetString("groq_api_key", null),
                "openai" => prefs?.GetString("openai_api_key", null),
                "gemini" => prefs?.GetString("gemini_api_key", null),
                _ => null
            };
            
            var isConfigured = !string.IsNullOrEmpty(apiKey);
            
            Android.Util.Log.Info("VIRA_Hybrid", $"AI Config: Provider={provider}, IsConfigured={isConfigured}");
            
            return new AIConfiguration
            {
                Provider = provider,
                ApiKey = apiKey ?? string.Empty,
                IsConfigured = isConfigured
            };
#else
            return new AIConfiguration
            {
                Provider = "none",
                ApiKey = string.Empty,
                IsConfigured = false
            };
#endif
        }
        catch (Exception ex)
        {
            #if __ANDROID__
            Android.Util.Log.Error("VIRA_Hybrid", $"Error getting AI configuration: {ex.Message}");
            #endif
            return new AIConfiguration
            {
                Provider = "none",
                ApiKey = string.Empty,
                IsConfigured = false
            };
        }
    }

    /// <summary>
    /// Process message with AI provider (Groq or Gemini)
    /// </summary>
    private async Task<ProcessingResult> ProcessWithAI(
        string message, 
        ConversationContext context, 
        AIConfiguration config)
    {
        // Select the appropriate AI service
        var aiService = config.Provider.ToLower() switch
        {
            "groq" => _groqService,
            "openai" => _openaiService,
            "gemini" => _geminiService,
            _ => _geminiService // Default to Gemini
        };
        
        #if __ANDROID__
        Android.Util.Log.Info("VIRA_Hybrid", $"Using AI service: {config.Provider}");
        #endif
        
        // Convert context to chat history
        var conversationHistory = context.PreviousMessages ?? new List<ChatMessage>();
        
        // Send message to AI
        var aiResponse = await aiService.SendMessageAsync(message, conversationHistory);
        
        // Check if response is an error message
        if (aiResponse.Content.Contains("❌") || aiResponse.Content.Contains("⚠️"))
        {
            // AI returned an error, treat as error result
            return new ErrorResult(
                errorMessage: "AI API error",
                fallbackResponse: aiResponse.Content
            );
        }
        
        // Return AI-enhanced result
        return new AIEnhancedResult(
            response: aiResponse.Content,
            action: null, // TODO: Extract actions from AI response
            provider: config.Provider
        );
    }

    /// <summary>
    /// Generate helpful message when AI is not configured
    /// </summary>
    private string GenerateHelpMessage(ProcessingResult ruleResult)
    {
        var baseMessage = "Maaf, saya tidak mengerti perintah tersebut.";
        
        // If rule-based had a response, use it
        if (ruleResult is RuleBasedResult ruleBasedResult && !string.IsNullOrEmpty(ruleBasedResult.Response))
        {
            baseMessage = ruleBasedResult.Response;
        }
        
        return $"{baseMessage}\n\n" +
               "💡 Untuk pertanyaan yang lebih kompleks, Anda dapat mengaktifkan AI di Settings:\n" +
               "1. Buka Settings\n" +
               "2. Pilih AI Provider (Groq atau Gemini)\n" +
               "3. Masukkan API Key Anda\n\n" +
               "Atau coba perintah seperti:\n" +
               "• \"cuaca hari ini\"\n" +
               "• \"berita terkini\"\n" +
               "• \"tambah task beli susu\"\n" +
               "• \"daftar task\"";
    }

    /// <summary>
    /// AI configuration data
    /// </summary>
    private class AIConfiguration
    {
        public string Provider { get; set; } = "none";
        public string ApiKey { get; set; } = string.Empty;
        public bool IsConfigured { get; set; } = false;
    }
}
