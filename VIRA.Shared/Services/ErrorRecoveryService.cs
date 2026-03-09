using VIRA.Shared.Models;

namespace VIRA.Shared.Services;

/// <summary>
/// Service for handling errors and implementing recovery mechanisms
/// </summary>
public class ErrorRecoveryService
{
    private readonly Dictionary<string, int> _retryAttempts = new();
    private readonly Dictionary<string, DateTime> _lastRetryTime = new();
    private const int MaxRetryAttempts = 3;
    private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(2);

    /// <summary>
    /// Check if retry is allowed for a specific operation
    /// </summary>
    public bool CanRetry(string operationId)
    {
        if (!_retryAttempts.ContainsKey(operationId))
        {
            return true;
        }

        var attempts = _retryAttempts[operationId];
        if (attempts >= MaxRetryAttempts)
        {
            return false;
        }

        // Check if enough time has passed since last retry
        if (_lastRetryTime.TryGetValue(operationId, out var lastTime))
        {
            var timeSinceLastRetry = DateTime.Now - lastTime;
            return timeSinceLastRetry >= GetBackoffDelay(attempts);
        }

        return true;
    }

    /// <summary>
    /// Get exponential backoff delay based on attempt number
    /// </summary>
    private TimeSpan GetBackoffDelay(int attemptNumber)
    {
        // Exponential backoff: 2s, 4s, 8s
        return TimeSpan.FromSeconds(Math.Pow(2, attemptNumber));
    }

    /// <summary>
    /// Record a retry attempt
    /// </summary>
    public void RecordRetry(string operationId)
    {
        if (!_retryAttempts.ContainsKey(operationId))
        {
            _retryAttempts[operationId] = 0;
        }

        _retryAttempts[operationId]++;
        _lastRetryTime[operationId] = DateTime.Now;
    }

    /// <summary>
    /// Reset retry counter for an operation (call on success)
    /// </summary>
    public void ResetRetries(string operationId)
    {
        _retryAttempts.Remove(operationId);
        _lastRetryTime.Remove(operationId);
    }

    /// <summary>
    /// Get remaining retry attempts
    /// </summary>
    public int GetRemainingAttempts(string operationId)
    {
        if (!_retryAttempts.ContainsKey(operationId))
        {
            return MaxRetryAttempts;
        }

        return Math.Max(0, MaxRetryAttempts - _retryAttempts[operationId]);
    }

    /// <summary>
    /// Handle error with automatic recovery strategy
    /// </summary>
    public async Task<T?> ExecuteWithRetryAsync<T>(
        string operationId,
        Func<Task<T>> operation,
        Func<Exception, bool>? shouldRetry = null)
    {
        while (CanRetry(operationId))
        {
            try
            {
                var result = await operation();
                ResetRetries(operationId);
                return result;
            }
            catch (Exception ex)
            {
                // Check if we should retry this error
                if (shouldRetry != null && !shouldRetry(ex))
                {
                    throw;
                }

                RecordRetry(operationId);

                if (!CanRetry(operationId))
                {
                    throw;
                }

                // Wait before retrying
                var delay = GetBackoffDelay(_retryAttempts[operationId] - 1);
                await Task.Delay(delay);
            }
        }

        return default;
    }

    /// <summary>
    /// Create user-friendly error response
    /// </summary>
    public string CreateErrorResponse(ViraError error)
    {
        var message = ErrorMessages.GetMessage(error);
        var suggestion = ErrorMessages.GetActionSuggestion(error);

        return $"{message}\n\n💡 {suggestion}";
    }

    /// <summary>
    /// Determine if error is retryable
    /// </summary>
    public bool IsRetryableError(ViraError error)
    {
        return error switch
        {
            NetworkError ne => ne.Type is NetworkErrorType.Timeout or NetworkErrorType.ServerError,
            ProcessingError pe => pe.Type is ProcessingErrorType.TimeoutError or ProcessingErrorType.AIRequestFailed,
            VoiceError ve => ve.Type is VoiceErrorType.RecognitionFailed or VoiceErrorType.NoSpeechDetected,
            _ => false
        };
    }

    /// <summary>
    /// Get fallback action for error
    /// </summary>
    public string? GetFallbackAction(ViraError error)
    {
        return error switch
        {
            NetworkError => "offline_mode",
            ProcessingError pe when pe.Type == ProcessingErrorType.AINotConfigured => "rule_based_only",
            ProcessingError pe when pe.Type == ProcessingErrorType.AIRequestFailed => "rule_based_fallback",
            PermissionError => "request_permission",
            _ => null
        };
    }
}

/// <summary>
/// Extension methods for error handling
/// </summary>
public static class ErrorHandlingExtensions
{
    /// <summary>
    /// Convert exception to ViraError
    /// </summary>
    public static ViraError ToViraError(this Exception exception)
    {
        return exception switch
        {
            HttpRequestException httpEx => new NetworkError(
                httpEx.StatusCode.HasValue ? NetworkErrorType.ServerError : NetworkErrorType.NoConnection,
                httpEx.Message
            ),
            TaskCanceledException => new NetworkError(NetworkErrorType.Timeout, "Request timeout"),
            UnauthorizedAccessException => new PermissionError(
                PermissionErrorType.Unknown,
                "Access denied"
            ),
            _ => new ProcessingError(ProcessingErrorType.Unknown, exception.Message)
        };
    }

    /// <summary>
    /// Check if exception is network-related
    /// </summary>
    public static bool IsNetworkError(this Exception exception)
    {
        return exception is HttpRequestException or TaskCanceledException;
    }

    /// <summary>
    /// Check if exception is permission-related
    /// </summary>
    public static bool IsPermissionError(this Exception exception)
    {
        return exception is UnauthorizedAccessException ||
               exception.Message.Contains("permission", StringComparison.OrdinalIgnoreCase);
    }
}
