namespace BugTriageWorkflow.Models;

/// <summary>
/// Represents aggregated metrics for a full workflow run.
/// </summary>
public class RunSummary {
    // -------------------------
    // Run configuration
    // -------------------------

    public string Model { get; set; } = "";

    public int KeywordCount { get; set; }

    public string PromptType { get; set; } = "";

    public string Scenario { get; set; } = "";

    public bool ManualEscalationHandling { get; set; }

    // -------------------------
    // Run results
    // -------------------------

    public int CasesEvaluated { get; set; }

    public double AverageConfidenceScore { get; set; }

    public double AverageAccuracyScore { get; set; }

    public double AverageQualityScore { get; set; }

    // -------------------------
    // Exact-match totals
    // -------------------------
    //
    // Example:
    // 68 correct predictions out of 90 total prediction fields.
    //

    public int TotalCorrectPredictions { get; set; }

    public int TotalPredictions { get; set; }

    public double RunTimeSeconds { get; set; }

}