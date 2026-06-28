using System.Text.Json.Serialization;

namespace BugTriageWorkflow.Models;

/// <summary>
/// Complete JSON schema model for structured benchmark output.
/// Enables programmatic analysis, regression detection, and CI quality gates.
/// </summary>
public class BenchmarkRunData
{
    /// <summary>Schema version for backwards compatibility tracking.</summary>
    [JsonPropertyName("schema_version")]
    public string SchemaVersion { get; set; } = "1.0";

    /// <summary>Run metadata (timestamp, file paths).</summary>
    [JsonPropertyName("run_metadata")]
    public RunMetadata RunMetadata { get; set; } = new();

    /// <summary>Configuration used for this benchmark run.</summary>
    [JsonPropertyName("configuration")]
    public Configuration Configuration { get; set; } = new();

    /// <summary>Test cases and their results.</summary>
    [JsonPropertyName("test_cases")]
    public List<TestCaseData> TestCases { get; set; } = [];

    /// <summary>Aggregated summary metrics.</summary>
    [JsonPropertyName("summary")]
    public RunSummary Summary { get; set; } = new();

    /// <summary>Failures encountered during the run.</summary>
    [JsonPropertyName("failures")]
    public List<FailureInfo> Failures { get; set; } = [];
}

/// <summary>
/// Metadata about the benchmark run (timing, file paths).
/// </summary>
public class RunMetadata
{
    /// <summary>When the run started (UTC).</summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    /// <summary>Duration of the entire run.</summary>
    [JsonPropertyName("duration_seconds")]
    public double DurationSeconds { get; set; }

    /// <summary>Path to the human-readable text log file.</summary>
    [JsonPropertyName("log_file")]
    public string LogFile { get; set; } = "";

    /// <summary>Path to this JSON file.</summary>
    [JsonPropertyName("json_file")]
    public string JsonFile { get; set; } = "";
}

/// <summary>
/// Configuration parameters for the benchmark run.
/// </summary>
public class Configuration
{
    /// <summary>Model used for classification.</summary>
    [JsonPropertyName("model")]
    public string Model { get; set; } = "";

    /// <summary>Number of keywords extracted during preprocessing.</summary>
    [JsonPropertyName("keyword_count")]
    public int KeywordCount { get; set; }

    /// <summary>Prompt type (Detailed, Medium, or Vague).</summary>
    [JsonPropertyName("prompt_type")]
    public string PromptType { get; set; } = "";

    /// <summary>Scenario filter (All, Category, Urgency, etc.).</summary>
    [JsonPropertyName("scenario")]
    public string Scenario { get; set; } = "";

    /// <summary>Whether manual escalation handling was enabled.</summary>
    [JsonPropertyName("manual_escalation_handling")]
    public bool ManualEscalationHandling { get; set; }
}

/// <summary>
/// Results for a single test case (scenario).
/// </summary>
public class TestCaseData
{
    /// <summary>Name of the scenario.</summary>
    [JsonPropertyName("scenario_name")]
    public string ScenarioName { get; set; } = "";

    /// <summary>Bug reports processed in this scenario.</summary>
    [JsonPropertyName("reports")]
    public List<ReportResult> Reports { get; set; } = [];
}

/// <summary>
/// Complete result for a single bug report through the entire pipeline.
/// </summary>
public class ReportResult
{
    /// <summary>Unique report identifier.</summary>
    [JsonPropertyName("report_id")]
    public string ReportId { get; set; } = "";

    /// <summary>Reporter name/identifier.</summary>
    [JsonPropertyName("reporter")]
    public string Reporter { get; set; } = "";

    /// <summary>Processing status (success or failed).</summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = "success";

    /// <summary>Failure details if status is failed.</summary>
    [JsonPropertyName("failure_reason")]
    public FailureInfo? FailureReason { get; set; }

    /// <summary>Preprocessing results.</summary>
    [JsonPropertyName("preprocessing")]
    public PreprocessingData? Preprocessing { get; set; }

    /// <summary>Classification results from AI model.</summary>
    [JsonPropertyName("classification")]
    public BugClassification? Classification { get; set; }

    /// <summary>Routing decision results.</summary>
    [JsonPropertyName("routing")]
    public RouteResult? Routing { get; set; }

    /// <summary>Expected classification for evaluation.</summary>
    [JsonPropertyName("expected")]
    public ExpectedBugClassification Expected { get; set; } = new();

    /// <summary>Evaluation metrics (accuracy, confidence, quality).</summary>
    [JsonPropertyName("evaluation")]
    public EvaluationData? Evaluation { get; set; }
}

/// <summary>
/// Failure information for a specific stage.
/// </summary>
public class FailureInfo
{
    /// <summary>Report ID where failure occurred.</summary>
    [JsonPropertyName("report_id")]
    public string ReportId { get; set; } = "";

    /// <summary>Pipeline stage where failure occurred (classifier, router, evaluation).</summary>
    [JsonPropertyName("stage")]
    public string Stage { get; set; } = "";

    /// <summary>Error category (unexpected_error, api_provider_error, etc.).</summary>
    [JsonPropertyName("category")]
    public string Category { get; set; } = "";

    /// <summary>Human-readable error message.</summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = "";

    /// <summary>Number of retry attempts before failure.</summary>
    [JsonPropertyName("attempts")]
    public int Attempts { get; set; }
}

/// <summary>
/// Preprocessing stage data.
/// </summary>
public class PreprocessingData
{
    /// <summary>Length of cleaned text.</summary>
    [JsonPropertyName("clean_text_length")]
    public int CleanTextLength { get; set; }

    /// <summary>Length of extracted evidence.</summary>
    [JsonPropertyName("evidence_length")]
    public int EvidenceLength { get; set; }

    /// <summary>Extracted keywords.</summary>
    [JsonPropertyName("keywords")]
    public List<string> Keywords { get; set; } = [];
}

/// <summary>
/// Evaluation metrics for a single report.
/// </summary>
public class EvaluationData
{
    /// <summary>Confidence scores by field.</summary>
    [JsonPropertyName("confidence")]
    public Dictionary<string, double> Confidence { get; set; } = new();

    /// <summary>Accuracy flags by field.</summary>
    [JsonPropertyName("accuracy")]
    public Dictionary<string, bool> Accuracy { get; set; } = new();

    /// <summary>Number of correct predictions.</summary>
    [JsonPropertyName("correct_predictions")]
    public int CorrectPredictions { get; set; }

    /// <summary>Total number of predictions.</summary>
    [JsonPropertyName("total_predictions")]
    public int TotalPredictions { get; set; }

    /// <summary>Accuracy score (0.0 to 1.0).</summary>
    [JsonPropertyName("accuracy_score")]
    public double AccuracyScore { get; set; }

    /// <summary>Quality score (0.0 to 1.0).</summary>
    [JsonPropertyName("quality_score")]
    public double QualityScore { get; set; }
}
