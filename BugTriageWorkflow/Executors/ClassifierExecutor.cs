using System.Text.Json;
using BugTriageWorkflow.Helpers;
using BugTriageWorkflow.Models;
using BugTriageWorkflow.Prompts;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace BugTriageWorkflow.Executors;

/// <summary>
/// Uses an AI agent to classify a preprocessed bug report.
/// Produces structured output used by the router and evaluation workflow.
/// </summary>
public static class ClassifierExecutor {
    private static readonly JsonSerializerOptions JsonOptions = new() {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Runs the classifier agent using the selected prompt variant.
    /// Cleans, deserializes, and validates the model response before returning it.
    /// </summary>
    public static async Task<BugClassification?> ExecuteAsync(AIAgent agent, PreprocessedBugReport report, PromptType promptType) {
        var prompt = ClassifierPrompts.Create(promptType, report);

        var result = await agent.RunAsync(prompt);

        var finalText = result.Messages
            .LastOrDefault()?
            .Contents
            .OfType<TextContent>()
            .FirstOrDefault()?
            .Text;

        if (string.IsNullOrWhiteSpace(finalText)) {
            Logger.Info("Classifier returned empty output.");
            return null;
        }

        var cleanedJson = CleanJsonResponse(finalText);

        try {
            var classification = JsonSerializer.Deserialize<BugClassification>(cleanedJson, JsonOptions);

            if (!ClassificationValidator.IsValid(classification, out var validationError)) {
                Logger.Info($"Invalid classification output: {validationError}");
                Logger.Info(cleanedJson);
                return null;
            }

            return classification;
        } catch (JsonException ex) {
            Logger.Info("Failed to deserialize classification.");
            Logger.Info($"JSON error: {ex.Message}");
            Logger.Info(cleanedJson);
            return null;
        }
    }

    /// <summary>
    /// Removes markdown formatting and extra text around the JSON object.
    /// This makes the classifier more tolerant of imperfect model formatting.
    /// </summary>
    private static string CleanJsonResponse(string text) {
        var cleaned = text.Trim();

        cleaned = RemoveCodeFence(cleaned);
        cleaned = ExtractJsonObject(cleaned);

        return cleaned;
    }

    /// <summary>
    /// Removes markdown code fences from a model response.
    /// Handles both ```json and plain ``` fences.
    /// </summary>
    private static string RemoveCodeFence(string text) {
        var cleaned = text.Trim();

        if (cleaned.StartsWith("```json", StringComparison.OrdinalIgnoreCase)) cleaned = cleaned["```json".Length..].Trim();
        
        else if (cleaned.StartsWith("```"))  cleaned = cleaned["```".Length..].Trim();

        if (cleaned.EndsWith("```")) cleaned = cleaned[..^3].Trim();
        

        return cleaned;
    }

    /// <summary>
    /// Extracts the first JSON object from a text response.
    /// Keeps the workflow resilient if the model adds text before or after JSON.
    /// </summary>
    private static string ExtractJsonObject(string text) {
        var firstBrace = text.IndexOf('{');
        var lastBrace = text.LastIndexOf('}');

        if (firstBrace >= 0 && lastBrace > firstBrace) {
            return text[firstBrace..(lastBrace + 1)];
        }

        return text;
    }
}