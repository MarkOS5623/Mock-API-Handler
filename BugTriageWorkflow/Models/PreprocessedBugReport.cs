namespace BugTriageWorkflow.Models;

/// <summary>
/// Represents a bug report after preprocessing.
/// The preprocessing step cleans the text and extracts useful keywords.
/// </summary>
public class PreprocessedBugReport
{
    /// <summary>
    /// Unique identifier for the bug report.
    /// </summary>
    public string Id { get; set; } = "";

    /// <summary>
    /// Name or identifier of the user who submitted the bug report.
    /// </summary>
    public string Reporter { get; set; } = "";

    /// <summary>
    /// Sanitized version of the original report text used
    /// for downstream classification and analysis.
    /// </summary>
    public string CleanText { get; set; } = "";

    /// <summary>
    /// Supporting evidence associated with the report,
    /// such as logs, test results, screenshots, or monitoring data.
    /// </summary>
    public string Evidence { get; set; } = "";

    /// <summary>
    /// Keywords extracted from the cleaned report text.
    /// These provide a simplified representation of the report
    /// and help guide classification.
    /// </summary>
    public List<string> Keywords { get; set; } = [];
}