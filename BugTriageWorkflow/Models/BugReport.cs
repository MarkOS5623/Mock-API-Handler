namespace BugTriageWorkflow.Models;

/// <summary>
/// Represents the raw bug report submitted by a user.
/// This is the initial input to the workflow.
/// </summary>
public class BugReport {
    /// <summary>
    /// Unique identifier for the bug report.
    /// </summary>
    public string Id { get; set; } = "";

    /// <summary>
    /// Name or identifier of the user who created the report.
    /// </summary>
    public string Reporter { get; set; } = "";

    /// <summary>
    /// Original bug report text submitted by the user.
    /// </summary>
    public string RawText { get; set; } = "";

    /// <summary>
    /// Optional test, monitoring, or reproduction evidence related to the report.
    /// This may support or contradict the user's claim.
    /// </summary>
    public string Evidence { get; set; } = "";
}