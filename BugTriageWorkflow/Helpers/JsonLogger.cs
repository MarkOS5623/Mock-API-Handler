using System.Text.Json;
using System.Text.Json.Serialization;
using BugTriageWorkflow.Models;

namespace BugTriageWorkflow.Helpers;

/// <summary>
/// Parallel structured JSON logging alongside text logs.
/// Builds in-memory BenchmarkRunData during run, persists to JSON on finalization.
/// </summary>
public static class JsonLogger
{
    private static BenchmarkRunData _runData = new();
    private static TestCaseData? _currentTestCase;
    private static DateTime _runStartTime;
    private static string _timestamp = "";

    /// <summary>
    /// Initialize JSON logger with run configuration.
    /// Call this once at the start of the benchmark run.
    /// </summary>
    public static void Initialize(
        string model,
        int keywordCount,
        string promptType,
        string scenario,
        bool manualEscalation)
    {
        _runStartTime = DateTime.UtcNow;
        _timestamp = _runStartTime.ToString("yyyyMMdd_HHmmss");

        _runData = new BenchmarkRunData
        {
            RunMetadata = new RunMetadata
            {
                Timestamp = _runStartTime,
                LogFile = $"logs/run_{_timestamp}.txt",
                JsonFile = $"logs/run_{_timestamp}.json"
            },
            Configuration = new Configuration
            {
                Model = model,
                KeywordCount = keywordCount,
                PromptType = promptType,
                Scenario = scenario,
                ManualEscalationHandling = manualEscalation
            }
        };
    }

    /// <summary>
    /// Start a new test case (scenario).
    /// Call this before processing each scenario's bug reports.
    /// </summary>
    public static void StartTestCase(string scenarioName)
    {
        _currentTestCase = new TestCaseData { ScenarioName = scenarioName };
        _runData.TestCases.Add(_currentTestCase);
    }

    /// <summary>
    /// Add a successfully processed report to the current test case.
    /// Call this after evaluation completes for a report.
    /// </summary>
    public static void AddReport(
        BugReport report,
        PreprocessedBugReport preprocessed,
        BugClassification? classification,
        RouteResult? route,
        ExpectedBugClassification expected,
        EvaluationResult? evaluation)
    {
        if (_currentTestCase == null)
        {
            throw new InvalidOperationException("StartTestCase must be called before AddReport.");
        }

        var reportResult = new ReportResult
        {
            ReportId = report.Id,
            Reporter = report.Reporter,
            Status = "success",
            Preprocessing = new PreprocessingData
            {
                CleanTextLength = preprocessed.CleanText.Length,
                EvidenceLength = preprocessed.Evidence.Length,
                Keywords = [..preprocessed.Keywords]
            },
            Classification = classification,
            Routing = route,
            Expected = expected
        };

        // Add evaluation data if present
        if (evaluation != null)
        {
            reportResult.Evaluation = new EvaluationData
            {
                Confidence = new Dictionary<string, double>
                {
                    ["Category"] = evaluation.CategoryConfidenceScore,
                    ["Urgency"] = evaluation.UrgencyConfidenceScore,
                    ["Route"] = evaluation.RouteConfidenceScore,
                    ["Verification"] = evaluation.VerificationConfidenceScore,
                    ["FalseReportRisk"] = evaluation.FalseReportRiskConfidenceScore
                },
                Accuracy = new Dictionary<string, bool>
                {
                    ["Category"] = evaluation.CategoryCorrect,
                    ["Urgency"] = evaluation.UrgencyCorrect,
                    ["Route"] = evaluation.RouteCorrect,
                    ["Verification"] = evaluation.VerificationCorrect,
                    ["FalseReportRisk"] = evaluation.FalseReportRiskCorrect
                },
                CorrectPredictions = evaluation.TotalCorrectPredictions,
                TotalPredictions = evaluation.TotalPredictions,
                AccuracyScore = evaluation.AccuracyScore,
                QualityScore = evaluation.QualityScore
            };
        }

        _currentTestCase.Reports.Add(reportResult);
    }

    /// <summary>
    /// Add a failure record for a report that failed at some stage.
    /// Call this immediately when a failure occurs.
    /// </summary>
    public static void AddFailure(
        string reportId,
        string stage,
        string category,
        string message,
        int attempts)
    {
        var failure = new FailureInfo
        {
            ReportId = reportId,
            Stage = stage,
            Category = category,
            Message = message,
            Attempts = attempts
        };

        _runData.Failures.Add(failure);

        // Also add to current test case as a failed report
        if (_currentTestCase != null)
        {
            var reportResult = new ReportResult
            {
                ReportId = reportId,
                Status = "failed",
                FailureReason = failure
            };
            _currentTestCase.Reports.Add(reportResult);
        }
    }

    /// <summary>
    /// Finalize the run and persist JSON to disk.
    /// Call this once at the end of the benchmark run.
    /// </summary>
    public static void Finalize(RunSummary summary, TimeSpan runtime)
    {
        _runData.RunMetadata.DurationSeconds = runtime.TotalSeconds;
        _runData.Summary = summary;

        WriteJsonFile();
    }

    /// <summary>
    /// Write the complete JSON file to disk.
    /// </summary>
    private static void WriteJsonFile()
    {
        var jsonPath = Path.Combine(ProjectPaths.Logs, $"run_{_timestamp}.json");

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };

        var json = JsonSerializer.Serialize(_runData, options);
        File.WriteAllText(jsonPath, json);
    }
}
