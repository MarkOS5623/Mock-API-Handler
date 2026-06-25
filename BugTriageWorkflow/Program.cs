using BugTriageWorkflow.Data;
using BugTriageWorkflow.Executors;
using BugTriageWorkflow.Helpers;
using BugTriageWorkflow.Models;
using DotNetEnv;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using System.ClientModel.Primitives;

Env.Load(ProjectPaths.EnvFile);

var apiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY");

if (string.IsNullOrWhiteSpace(apiKey)) {
    Logger.Info("OPENROUTER_API_KEY is missing.");
    return;
}

var model = Environment.GetEnvironmentVariable("MODEL");

if (string.IsNullOrWhiteSpace(model)) {
    Logger.Info("MODEL is missing.");
    return;
}

const int DefaultKeywordCount = 5;

var keywordCount = ConsoleInputHelper.ReadKeywordCount(DefaultKeywordCount);
var promptType = ConsoleInputHelper.ReadPromptType();
var selectedScenario = ConsoleInputHelper.ReadScenarioName();
var handleEscalationsManually = ConsoleInputHelper.ReadManualEscalationMode();

Logger.Section("RUN CONFIGURATION");
Logger.RunConfiguration(model, keywordCount, promptType.ToString(), selectedScenario);
Logger.Info($"Manual Escalation Handling: {handleEscalationsManually}");

var openAiOptions = new OpenAIClientOptions { Endpoint = new Uri("https://openrouter.ai/api/v1") };

openAiOptions.AddPolicy(
    new OpenRouterHeadersPolicy(referer: "http://localhost", title: "HW2 Bug Triage Workflow"),
    PipelinePosition.PerCall);

var openAiClient = new OpenAIClient(new System.ClientModel.ApiKeyCredential(apiKey), openAiOptions);

var chatClient = openAiClient.GetChatClient(model).AsIChatClient();

var classifierAgent = chatClient.AsAIAgent(
    instructions:
    """
    You are a bug triage classifier.
    Follow the user's classification instructions and return only valid JSON.
    """,
    name: "BugClassifierAgent"
);

var cases = BugReportDataStore.LoadOrCreateBugReports();
var expectedResults = BugReportDataStore.LoadOrCreateExpectedResults();

var selectedCases = FilterCases(cases, selectedScenario);

var runStartTime = DateTime.UtcNow;
var summaryBuilder = new RunSummaryBuilder();

foreach (var testCase in selectedCases) {
    Logger.Case(testCase.Key);

    foreach (var report in testCase.Value) {
        Logger.Section("RAW BUG REPORT");
        Logger.RawBugReport(report);

        Logger.Section("PREPROCESS EXECUTOR");

        var preprocessed = PreprocessExecutor.Execute(report, keywordCount);

        Logger.PreprocessedReport(preprocessed);

        Logger.Section("CLASSIFIER EXECUTOR");

        var classification = await RetryHelper.ExecuteAsync(() => ClassifierExecutor.ExecuteAsync(classifierAgent, preprocessed, promptType), maxAttempts: 3);

        if (classification == null) {
            Logger.Info("Classification failed.");
            continue;
        }

        Logger.Classification(classification);

        Logger.Section("ROUTER EXECUTOR");

        var route = RouterExecutor.Execute(classification, report, handleEscalationsManually);

        Logger.Route(route);

        if (!expectedResults.TryGetValue(report.Id, out var expected)) {
            Logger.Section("EXPECTED RESULT");
            Logger.Info($"No expected result found for report id: {report.Id}");
            continue;
        }

        Logger.Section("EXPECTED RESULT");
        Logger.Expected(expected);

        Logger.Section("EVALUATION");

        var evaluation = EvaluationExecutor.Execute(expected, classification);

        Logger.Evaluation(evaluation);

        summaryBuilder.Add(evaluation);
    }
}

var runDuration = DateTime.UtcNow - runStartTime;

var runSummary = summaryBuilder.Build(model, keywordCount, promptType.ToString(), selectedScenario, handleEscalationsManually, runDuration);

Logger.RunSummary(runSummary);

Logger.Section("RUN COMPLETE");
Logger.Info($"Log saved to: {Logger.CurrentLogFile}");

/// <summary>
/// Filters the loaded test cases based on the selected scenario.
/// Returns all cases when the selected scenario is All.
/// </summary>
static Dictionary<string, List<BugReport>> FilterCases(
    Dictionary<string, List<BugReport>> cases,
    string selectedScenario) {

    if (selectedScenario == "All") return cases;

    return cases.Where(testCase => testCase.Key == selectedScenario).ToDictionary(testCase => testCase.Key, testCase => testCase.Value);
}