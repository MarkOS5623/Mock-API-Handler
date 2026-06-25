namespace BugTriageWorkflow.Models;

/// <summary>
/// Represents the evaluation result for one classified bug report.
/// Includes confidence-based scoring, exact-match accuracy,
/// and a combined quality score.
/// </summary>
public class EvaluationResult {
    // -------------------------
    // Confidence scores
    // -------------------------
    //
    // These measure how much confidence the classifier assigned
    // to the expected answer.
    //
    // Example:
    // Expected category = backend
    // Actual vector = [0.10, 0.80, 0.05, 0.05]
    // Category confidence score = 0.80
    //

    public double CategoryConfidenceScore { get; set; }

    public double UrgencyConfidenceScore { get; set; }

    public double RouteConfidenceScore { get; set; }

    public double EscalationConfidenceScore { get; set; }

    public double VerificationConfidenceScore { get; set; }

    public double FalseReportRiskConfidenceScore { get; set; }

    /// <summary>
    /// Average confidence score across all evaluation fields.
    /// </summary>
    public double ConfidenceScore { get; set; }

    // -------------------------
    // Accuracy checks
    // -------------------------
    //
    // These measure whether the selected label/value exactly matched
    // the expected result.
    //

    public bool CategoryCorrect { get; set; }

    public bool UrgencyCorrect { get; set; }

    public bool RouteCorrect { get; set; }

    public bool EscalationCorrect { get; set; }

    public bool VerificationCorrect { get; set; }

    public bool FalseReportRiskCorrect { get; set; }

    /// <summary>
    /// Exact-match accuracy across all evaluation fields.
    /// Range: 0.0 - 1.0
    /// </summary>
    public double AccuracyScore { get; set; }

    // -------------------------
    // Combined quality score
    // -------------------------
    //
    // This balances confidence and exact-match accuracy.
    //

    public double QualityScore { get; set; }

    // -------------------------
    // Exact-match counters
    // -------------------------
    //
    // These are useful for run summaries.
    // For one bug report, TotalPredictions should usually be 6.
    //

    public int TotalCorrectPredictions { get; set; }

    public int TotalPredictions { get; set; }
}