namespace BugTriageWorkflow.Prompts;

/// <summary>
/// Represents the prompt variants used during classifier evaluation.
/// The variants differ in the amount of instruction and guidance
/// provided to the language model.
/// </summary>
public enum PromptType {
    /// <summary>
    /// Full prompt containing detailed classification,
    /// routing, verification, and scoring instructions.
    /// </summary>
    Detailed,

    /// <summary>
    /// Simplified prompt containing only core categories,
    /// output requirements, and minimal guidance.
    /// </summary>
    Medium,

    /// <summary>
    /// Minimal prompt containing only the report data
    /// and required output schema.
    /// </summary>
    Vague
}