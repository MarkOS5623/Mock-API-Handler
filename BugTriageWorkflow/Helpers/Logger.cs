using BugTriageWorkflow.Models;

namespace BugTriageWorkflow.Helpers;

public static class Logger {
    private static readonly string LogFileName =
        Path.Combine(ProjectPaths.Logs, $"run_{DateTime.Now:yyyyMMdd_HHmmss}.txt");

    static Logger() { Directory.CreateDirectory(ProjectPaths.Logs); }

    public static string CurrentLogFile => LogFileName;

    public static void Section(string title) {
        Info("");
        Info("==================================================");
        Info(title);
        Info("==================================================");
    }

    public static void Case(string title) {
        Info("");
        Info("##################################################");
        Info($"CASE: {title}");
        Info("##################################################");
    }

    public static void Info(string message) {
        Console.WriteLine(message);
        File.AppendAllText(LogFileName, message + Environment.NewLine);
    }

    /// <summary>
    /// Logs the configuration used for the current run.
    /// This makes it easier to compare different prompt types,
    /// keyword counts, models, and scenarios later.
    /// </summary>
    public static void RunConfiguration(
        string model,
        int keywordCount,
        string promptType,
        string scenario) {

        Info($"Model: {model}");
        Info($"Keyword Count: {keywordCount}");
        Info($"Prompt Type: {promptType}");
        Info($"Scenario: {scenario}");
        Info($"Log File: {LogFileName}");
    }

    public static void RawBugReport(BugReport report) {
        Info($"Id: {report.Id}");
        Info($"Reporter: {report.Reporter}");
        Info($"Description: {report.RawText.Trim()}");

        if (!string.IsNullOrWhiteSpace(report.Evidence)) Info($"Evidence: {report.Evidence.Trim()}");
    }

    public static void PreprocessedReport(PreprocessedBugReport report) {
        Info($"Id: {report.Id}");
        Info($"Reporter: {report.Reporter}");
        Info($"Clean Text: {report.CleanText}");
        Info($"Evidence: {report.Evidence}");
        Info($"Keywords: {string.Join(", ", report.Keywords)}");
    }

    public static void Classification(BugClassification classification) {
        Info($"Category: {classification.Category}");
        Info($"Category Vector: {FormatVector(classification.CategoryVector)}");

        Info($"Urgency: {classification.Urgency}");
        Info($"Urgency Value: {classification.UrgencyValue:0.00}");

        Info($"Missing Info: {string.Join(", ", classification.MissingInfo)}");

        Info($"Recommended Route: {classification.RecommendedRoute}");
        Info($"Route Vector: {FormatVector(classification.RecommendedRouteVector)}");

        Info($"Escalate To Human: {classification.EscalateToHuman}");
        Info($"Escalation Value: {classification.EscalateToHumanValue:0.00}");

        Info($"Verification Status: {classification.VerificationStatus}");
        Info($"Verification Vector: {FormatVector(classification.VerificationStatusVector)}");

        Info($"False Report Risk: {classification.FalseReportRisk}");
        Info($"False Report Risk Value: {classification.FalseReportRiskValue:0.00}");

        Info($"Verification Reason: {classification.VerificationReason}");
    }

    public static void Expected(ExpectedBugClassification expected) {
        Info($"Expected Category: {expected.Category}");
        Info($"Expected Urgency: {expected.Urgency}");
        Info($"Expected Route: {expected.RecommendedRoute}");
        Info($"Expected Escalation: {expected.EscalateToHuman}");
        Info($"Expected Verification: {expected.VerificationStatus}");
        Info($"Expected False Report Risk: {expected.FalseReportRisk}");
    }

    /// <summary>
    /// Logs evaluation metrics for a single bug report.
    ///
    /// Confidence scores measure how much confidence the classifier
    /// assigned to the expected answer.
    ///
    /// Correct flags measure exact-match accuracy.
    ///
    /// Quality score combines confidence and accuracy into one metric.
    /// </summary>
    public static void Evaluation(EvaluationResult evaluation) {
        Info($"Category Confidence Score: {evaluation.CategoryConfidenceScore:0.00}");
        Info($"Category Correct: {evaluation.CategoryCorrect}");

        Info($"Urgency Confidence Score: {evaluation.UrgencyConfidenceScore:0.00}");
        Info($"Urgency Correct: {evaluation.UrgencyCorrect}");

        Info($"Route Confidence Score: {evaluation.RouteConfidenceScore:0.00}");
        Info($"Route Correct: {evaluation.RouteCorrect}");

        Info($"Escalation Confidence Score: {evaluation.EscalationConfidenceScore:0.00}");
        Info($"Escalation Correct: {evaluation.EscalationCorrect}");

        Info($"Verification Confidence Score: {evaluation.VerificationConfidenceScore:0.00}");
        Info($"Verification Correct: {evaluation.VerificationCorrect}");

        Info($"False Report Risk Confidence Score: {evaluation.FalseReportRiskConfidenceScore:0.00}");
        Info($"False Report Risk Correct: {evaluation.FalseReportRiskCorrect}");

        Info("");

        Info($"Confidence Score: {evaluation.ConfidenceScore:0.00}");
        Info($"Accuracy Score: {evaluation.AccuracyScore:0.00}");
        Info($"Quality Score: {evaluation.QualityScore:0.00}");

        Info(
            $"Correct Predictions: {evaluation.TotalCorrectPredictions}/{evaluation.TotalPredictions}");
    }

    public static void Route(RouteResult route) {
        Info($"Assigned Route: {route.Route}");
        Info($"Routed By: {route.RoutedBy}");
        Info($"Routing Reason: {route.Reason}");

        if (!string.IsNullOrWhiteSpace(route.HumanSelectedUrgency)) Info($"Human Selected Urgency: {route.HumanSelectedUrgency}");

        if (route.HumanMarkedFalseReport.HasValue) Info($"Human Marked False Report: {route.HumanMarkedFalseReport.Value}");
    }

    /// <summary>
    /// Logs the final run summary.
    /// This provides a single place to compare different runs.
    /// </summary>
    public static void RunSummary(RunSummary summary) {
        Section("RUN SUMMARY");

        Info("RUN CONFIGURATION");
        Info($"Model: {summary.Model}");
        Info($"Keyword Count: {summary.KeywordCount}");
        Info($"Prompt Type: {summary.PromptType}");
        Info($"Scenario: {summary.Scenario}");
        Info($"Manual Escalation Handling: {summary.ManualEscalationHandling}");

        Info("");

        Info("RUN RESULTS");
        Info($"Cases Evaluated: {summary.CasesEvaluated}");

        Info($"Average Confidence Score: {summary.AverageConfidenceScore:0.00}");
        Info($"Average Accuracy Score: {summary.AverageAccuracyScore:0.00}");
        Info($"Average Quality Score: {summary.AverageQualityScore:0.00}");

        Info($"Correct Predictions: {summary.TotalCorrectPredictions}/{summary.TotalPredictions}");

        Info("");

        Info("RUNTIME");
        Info($"Run Time Seconds: {summary.RunTimeSeconds:0.00}");
    }

    private static string FormatVector(List<double> vector) { return $"[{string.Join(", ", vector.Select(v => v.ToString("0.00")))}]";}
}