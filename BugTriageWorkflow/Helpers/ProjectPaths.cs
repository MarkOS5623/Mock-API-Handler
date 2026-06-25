namespace BugTriageWorkflow.Helpers;

/// <summary>
/// Centralized project paths used throughout the workflow.
/// Keeps file and directory locations in a single place.
/// </summary>
public static class ProjectPaths {
    /// <summary>
    /// Root directory of the project.
    /// Resolves correctly when running from Debug or Release builds.
    /// </summary>
    public static readonly string ProjectRoot = AppContext.BaseDirectory
        .Split(Path.Combine("bin", "Debug"))[0]
        .Split(Path.Combine("bin", "Release"))[0]
        .TrimEnd(Path.DirectorySeparatorChar);

    /// <summary>
    /// Root data directory used by the workflow.
    /// </summary>
    public static readonly string Data = Path.Combine(ProjectRoot, "data");

    /// <summary>
    /// Directory containing training datasets used during evaluation.
    /// </summary>
    public static readonly string TrainingData = Path.Combine(Data, "TrainingData");

    /// <summary>
    /// Directory used for workflow execution logs.
    /// </summary>
    public static readonly string Logs = Path.Combine(ProjectRoot, "logs");

    /// <summary>
    /// Training dataset containing raw bug reports.
    /// </summary>
    public static readonly string BugReports = Path.Combine(TrainingData, "bug_reports.json");

    /// <summary>
    /// Expected classifications used to evaluate model performance.
    /// </summary>
    public static readonly string ExpectedResults = Path.Combine(TrainingData, "expected_results.json");

    /// <summary>
    /// Default workflow log file.
    /// </summary>
    public static readonly string LogFile = Path.Combine(Logs, "workflow_log.txt");

    /// <summary>
    /// Environment variable configuration file.
    /// Contains secrets such as API keys.
    /// </summary>
    public static readonly string EnvFile = Path.Combine(ProjectRoot, ".env");
}