using System.ClientModel.Primitives;

namespace BugTriageWorkflow.Helpers;

/// <summary>
/// Adds OpenRouter attribution headers to outgoing API requests.
/// These headers help identify the application making requests to OpenRouter.
/// </summary>
public sealed class OpenRouterHeadersPolicy : PipelinePolicy {
    private readonly string _referer;
    private readonly string _title;

    /// <summary>
    /// Creates a new OpenRouter header policy.
    /// </summary>
    /// <param name="referer">
    /// URL identifying the application or website making requests.
    /// </param>
    /// <param name="title">
    /// Human-readable name of the application.
    /// </param>
    public OpenRouterHeadersPolicy(string referer, string title) {
        _referer = referer;
        _title = title;
    }

    /// <summary>
    /// Adds OpenRouter headers to a synchronous request.
    /// </summary>
    public override void Process(
        PipelineMessage message,
        IReadOnlyList<PipelinePolicy> pipeline,
        int currentIndex) {

        AddHeaders(message);
        ProcessNext(message, pipeline, currentIndex);
    }

    /// <summary>
    /// Adds OpenRouter headers to an asynchronous request.
    /// </summary>
    public override ValueTask ProcessAsync(
        PipelineMessage message,
        IReadOnlyList<PipelinePolicy> pipeline,
        int currentIndex) {

        AddHeaders(message);
        return ProcessNextAsync(message, pipeline, currentIndex);
    }

    /// <summary>
    /// Applies the configured OpenRouter attribution headers to a request.
    /// </summary>
    private void AddHeaders(PipelineMessage message) {
        message.Request.Headers.Set("HTTP-Referer", _referer);
        message.Request.Headers.Set("X-Title", _title);
    }
}