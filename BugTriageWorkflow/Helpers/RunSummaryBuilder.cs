using BugTriageWorkflow.Models;

namespace BugTriageWorkflow.Helpers;

/// <summary>
/// Collects per-case evaluation results and builds the final run summary.
/// Keeps Program.cs focused on workflow orchestration instead of score math.
/// </summary>
public class RunSummaryBuilder {
    private double totalConfidenceScore;
    private double totalAccuracyScore;
    private double totalQualityScore;

    private int totalCorrectPredictions;
    private int totalPredictions;
    private int evaluationCount;

    /// <summary>
    /// Adds one evaluated bug report to the run totals.
    /// </summary>
    public void Add(EvaluationResult evaluation) {
        totalConfidenceScore += evaluation.ConfidenceScore;
        totalAccuracyScore += evaluation.AccuracyScore;
        totalQualityScore += evaluation.QualityScore;

        totalCorrectPredictions += evaluation.TotalCorrectPredictions;
        totalPredictions += evaluation.TotalPredictions;

        evaluationCount++;
    }

    /// <summary>
    /// Builds the final summary object after the run completes.
    /// </summary>
    public RunSummary Build(string model, int keywordCount, string promptType, string scenario, bool manualEscalationHandling, TimeSpan runDuration) {

        var averageConfidenceScore = evaluationCount > 0 ? totalConfidenceScore / evaluationCount : 0.0;

        var averageAccuracyScore = evaluationCount > 0 ? totalAccuracyScore / evaluationCount : 0.0;

        var averageQualityScore = evaluationCount > 0 ? totalQualityScore / evaluationCount : 0.0;

        return new RunSummary {
            Model = model,
            KeywordCount = keywordCount,
            PromptType = promptType,
            Scenario = scenario,
            ManualEscalationHandling = manualEscalationHandling,

            CasesEvaluated = evaluationCount,

            AverageConfidenceScore = averageConfidenceScore,
            AverageAccuracyScore = averageAccuracyScore,
            AverageQualityScore = averageQualityScore,

            TotalCorrectPredictions = totalCorrectPredictions,
            TotalPredictions = totalPredictions,

            RunTimeSeconds = runDuration.TotalSeconds,
        };
    }
}