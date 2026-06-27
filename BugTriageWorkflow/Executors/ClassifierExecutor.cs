using System.Text.Json;
using BugTriageWorkflow.Helpers;
using BugTriageWorkflow.Models;
using BugTriageWorkflow.Prompts;
using Microsoft.Extensions.AI;

namespace BugTriageWorkflow.Executors;

/// <summary>
/// Uses an AI chat client to classify a preprocessed bug report.
/// Produces structured output used by the router and evaluation workflow.
/// </summary>
public static class ClassifierExecutor {
    private static readonly JsonSerializerOptions JsonOptions = new() {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Runs the classifier using the selected prompt variant with structured output.
    /// Uses JSON schema validation to enforce response format.
    /// </summary>
    public static async Task<BugClassification?> ExecuteAsync(IChatClient chatClient, PreprocessedBugReport report, PromptType promptType) {
        var prompt = ClassifierPrompts.Create(promptType, report);

        var chatOptions = new ChatOptions {
            ResponseFormat = ChatResponseFormat.ForJsonSchema<BugClassification>(
                schemaName: "BugClassification",
                schemaDescription: "Structured classification result for a bug report"
            )
        };

        var messages = new List<ChatMessage> {
            new(ChatRole.System, "You are a bug triage classifier. Follow the user's classification instructions and return only valid JSON."),
            new(ChatRole.User, prompt)
        };

        try {
            var response = await chatClient.GetResponseAsync(messages, chatOptions);

            // Extract text content from the response
            var messageContent = response.Text;

            if (string.IsNullOrWhiteSpace(messageContent)) {
                Logger.Info("Classifier returned empty output.");
                return null;
            }

            var classification = JsonSerializer.Deserialize<BugClassification>(messageContent, JsonOptions);

            if (!ClassificationValidator.IsValid(classification, out var validationError)) {
                Logger.Info($"Invalid classification output: {validationError}");
                Logger.Info(messageContent);
                return null;
            }

            return classification;
        } catch (JsonException ex) {
            Logger.Info("Failed to deserialize classification.");
            Logger.Info($"JSON error: {ex.Message}");
            Logger.Info("Error Category: Validation Error");
            return null;
        } catch (Exception ex) {
            var errorCategory = ErrorClassifier.Classify(ex);
            var categoryDescription = ErrorClassifier.GetCategoryDescription(errorCategory);

            Logger.Info($"Classification failed: {ex.GetType().Name}: {ex.Message}");
            Logger.Info($"Error Category: {categoryDescription}");

            // Rethrow to allow RetryHelper to handle retries
            throw;
        }
    }
}