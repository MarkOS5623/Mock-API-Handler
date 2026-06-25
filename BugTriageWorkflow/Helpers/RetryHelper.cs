namespace BugTriageWorkflow.Helpers;

/// <summary>
/// Provides retry logic for operations that may fail temporarily,
/// such as AI model calls, rate limits, or network requests.
/// </summary>
public static class RetryHelper {
    private const int DefaultMaxAttempts = 3;
    private const int DefaultInitialDelayMs = 1000;
    private const int DefaultMaxDelayMs = 10_000;

    /// <summary>
    /// Executes an asynchronous operation with exponential backoff retries.
    ///
    /// </summary>
    /// <typeparam name="T">
    /// Type returned by the operation.
    /// </typeparam>
    /// <param name="action">
    /// Operation to execute.
    /// </param>
    /// <param name="maxAttempts">
    /// Maximum number of total attempts, including the first attempt.
    /// </param>
    /// <param name="initialDelayMs">
    /// Delay in milliseconds before the first retry.
    /// Subsequent retries use exponential backoff.
    /// </param>
    /// <param name="maxDelayMs">
    /// Maximum retry delay in milliseconds.
    /// Prevents very large waits if maxAttempts increases later.
    /// </param>
    /// <param name="shouldRetry">
    /// Optional predicate used to decide whether an exception should be retried.
    /// If omitted, all exceptions are considered retryable.
    /// </param>
    public static async Task<T?> ExecuteAsync<T>(
        Func<Task<T?>> action,
        int maxAttempts = DefaultMaxAttempts,
        int initialDelayMs = DefaultInitialDelayMs,
        int maxDelayMs = DefaultMaxDelayMs,
        Func<Exception, bool>? shouldRetry = null) {

        if (maxAttempts <= 0) throw new ArgumentOutOfRangeException(nameof(maxAttempts), "Max attempts must be greater than zero.");

        if (initialDelayMs <= 0) throw new ArgumentOutOfRangeException(nameof(initialDelayMs), "Initial delay must be greater than zero.");

        if (maxDelayMs < initialDelayMs) throw new ArgumentOutOfRangeException(nameof(maxDelayMs), "Max delay must be greater than or equal to the initial delay.");

        shouldRetry ??= _ => true;

        for (var attempt = 1; attempt <= maxAttempts; attempt++) {
            try {
                Logger.Info($"Attempt {attempt}/{maxAttempts}...");

                var result = await action();

                if (result != null) return result;

                Logger.Info($"Attempt {attempt}/{maxAttempts} returned no result.");
            } catch (Exception ex) {
                Logger.Info($"Attempt {attempt}/{maxAttempts} failed: {ex.GetType().Name}: {ex.Message}");

                if (!shouldRetry(ex)) {
                    Logger.Info("Failure is not retryable. Stopping retries.");
                    return default;
                }
            }

            if (attempt < maxAttempts) {
                var delay = CalculateDelayMs(attempt, initialDelayMs, maxDelayMs);

                Logger.Info($"Retrying in {delay}ms...");

                await Task.Delay(delay);
            }
        }

        Logger.Info($"All {maxAttempts} attempts failed.");

        return default;
    }

    /// <summary>
    /// Calculates exponential backoff delay with jitter.
    ///
    /// Base delay:
    /// attempt 1 -> initialDelayMs
    /// attempt 2 -> initialDelayMs * 2
    /// attempt 3 -> initialDelayMs * 4
    ///
    /// Jitter:
    /// Adds a small random delay so repeated calls do not retry at the
    /// exact same millisecond.
    /// </summary>
    private static int CalculateDelayMs(int attempt, int initialDelayMs, int maxDelayMs) {

        var exponentialDelay = initialDelayMs * Math.Pow(2, attempt - 1);
        var cappedDelay = Math.Min(exponentialDelay, maxDelayMs);

        // Adds 0-250ms jitter.
        var jitter = Random.Shared.Next(0, 251);

        return (int)cappedDelay + jitter;
    }
}