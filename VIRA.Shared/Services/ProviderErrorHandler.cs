using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace VIRA.Shared.Services
{
    /// <summary>
    /// Handles errors from AI providers with user-friendly messages
    /// </summary>
    public class ProviderErrorHandler
    {
        /// <summary>
        /// Handles provider errors and returns user-friendly message
        /// </summary>
        public static async Task<string> HandleProviderError(Exception ex, string providerName)
        {
            if (ex is HttpRequestException)
            {
                // Network error
                return "Unable to connect to AI service. Please check your internet connection.";
            }
            else if (ex is UnauthorizedAccessException || ex.Message.Contains("401") || ex.Message.Contains("unauthorized"))
            {
                // Invalid API key
                return $"Invalid API key for {providerName}. Please check your settings.";
            }
            else if (ex is TaskCanceledException || ex is TimeoutException)
            {
                // Timeout
                return "Request timed out. Please try again.";
            }
            else if (ex.Message.Contains("429") || ex.Message.Contains("rate limit"))
            {
                // Rate limit
                return $"{providerName} rate limit exceeded. Please try again later.";
            }
            else if (ex.Message.Contains("500") || ex.Message.Contains("503"))
            {
                // Server error
                return $"{providerName} is experiencing issues. Please try again later.";
            }
            else
            {
                // Generic error
                return $"An error occurred with {providerName}. Please try again later.";
            }
        }

        /// <summary>
        /// Determines if an error is recoverable
        /// </summary>
        public static bool IsRecoverableError(Exception ex)
        {
            // Network errors, timeouts, and rate limits are recoverable
            return ex is HttpRequestException ||
                   ex is TaskCanceledException ||
                   ex is TimeoutException ||
                   ex.Message.Contains("429") ||
                   ex.Message.Contains("503");
        }

        /// <summary>
        /// Determines if an error requires navigation to settings
        /// </summary>
        public static bool RequiresSettingsNavigation(Exception ex)
        {
            // API key errors require settings navigation
            return ex is UnauthorizedAccessException ||
                   ex.Message.Contains("401") ||
                   ex.Message.Contains("unauthorized") ||
                   ex.Message.Contains("invalid api key");
        }
    }
}
