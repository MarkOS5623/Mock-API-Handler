namespace BugTriageWorkflow.Helpers;

/// <summary>
/// Categories of errors that can occur during workflow execution.
/// Used for clear error reporting and handling.
/// </summary>
public enum ErrorCategory {
    /// <summary>
    /// Missing or invalid configuration (API key, model, environment variables).
    /// </summary>
    Configuration,

    /// <summary>
    /// API provider errors (authentication, rate limits, service unavailable).
    /// </summary>
    ApiProvider,

    /// <summary>
    /// Network connectivity issues (timeouts, DNS resolution).
    /// </summary>
    Network,

    /// <summary>
    /// Business logic validation failures (semantic rule violations).
    /// </summary>
    Validation,

    /// <summary>
    /// Unexpected errors not fitting other categories.
    /// </summary>
    Unexpected
}

/// <summary>
/// Classifies exceptions into error categories for appropriate handling and logging.
/// </summary>
public static class ErrorClassifier {
    /// <summary>
    /// Determines the error category for an exception.
    /// </summary>
    public static ErrorCategory Classify(Exception ex) {
        var exceptionType = ex.GetType().Name;
        var message = ex.Message?.ToLowerInvariant() ?? "";

        // API authentication and provider errors
        if (message.Contains("401") || message.Contains("unauthorized") || message.Contains("authentication")) {
            return ErrorCategory.ApiProvider;
        }

        // Rate limiting
        if (message.Contains("429") || message.Contains("rate limit") || message.Contains("too many requests")) {
            return ErrorCategory.ApiProvider;
        }

        // Service availability
        if (message.Contains("503") || message.Contains("service unavailable") || message.Contains("provider returned error")) {
            return ErrorCategory.ApiProvider;
        }

        // Network issues
        if (exceptionType.Contains("Http") || exceptionType.Contains("Network") ||
            message.Contains("timeout") || message.Contains("connection") || message.Contains("dns")) {
            return ErrorCategory.Network;
        }

        // SDK or API client errors
        if (exceptionType.Contains("ClientResult") || exceptionType.Contains("ApiException")) {
            return ErrorCategory.ApiProvider;
        }

        return ErrorCategory.Unexpected;
    }

    /// <summary>
    /// Returns a user-friendly error category description.
    /// </summary>
    public static string GetCategoryDescription(ErrorCategory category) {
        return category switch {
            ErrorCategory.Configuration => "Configuration Error",
            ErrorCategory.ApiProvider => "API Provider Error",
            ErrorCategory.Network => "Network Error",
            ErrorCategory.Validation => "Validation Error",
            ErrorCategory.Unexpected => "Unexpected Error",
            _ => "Unknown Error"
        };
    }

    /// <summary>
    /// Returns guidance for the user based on error category.
    /// </summary>
    public static string GetRecoveryGuidance(ErrorCategory category) {
        return category switch {
            ErrorCategory.Configuration => "Check environment variables and configuration files.",
            ErrorCategory.ApiProvider => "Check API key, model name, and provider service status. May require retry.",
            ErrorCategory.Network => "Check network connectivity and firewall settings. May require retry.",
            ErrorCategory.Validation => "Review classification output and validation rules.",
            ErrorCategory.Unexpected => "Review error details and logs for diagnosis.",
            _ => "Review error details."
        };
    }
}
