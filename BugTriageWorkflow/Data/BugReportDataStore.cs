using System.Text.Json;
using BugTriageWorkflow.Helpers;
using BugTriageWorkflow.Models;

namespace BugTriageWorkflow.Data;

public static class BugReportDataStore {
    private static readonly string BugReportsFilePath = ProjectPaths.BugReports;

    private static readonly string ExpectedResultsFilePath = ProjectPaths.ExpectedResults;

    private static readonly JsonSerializerOptions JsonOptions = new() {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public static Dictionary<string, List<BugReport>> LoadOrCreateBugReports() {
        if (File.Exists(BugReportsFilePath)) {
            var json = File.ReadAllText(BugReportsFilePath);

            return JsonSerializer.Deserialize<Dictionary<string, List<BugReport>>>(json, JsonOptions) ?? [];
        }

        var reports = SampleBugReports.CreateCases();

        Directory.CreateDirectory(Path.GetDirectoryName(BugReportsFilePath)!);

        File.WriteAllText(BugReportsFilePath, JsonSerializer.Serialize(reports, JsonOptions));

        return reports;
    }

    public static Dictionary<string, ExpectedBugClassification>
        LoadOrCreateExpectedResults() {
        if (File.Exists(ExpectedResultsFilePath)) {
            var json = File.ReadAllText(ExpectedResultsFilePath);

            return JsonSerializer.Deserialize<
                Dictionary<string, ExpectedBugClassification>>(
                    json,
                    JsonOptions) ?? [];
        }

        var expectedResults = ExpectedBugResults.Create();

        Directory.CreateDirectory(Path.GetDirectoryName(ExpectedResultsFilePath)!);

        File.WriteAllText(ExpectedResultsFilePath, JsonSerializer.Serialize(expectedResults, JsonOptions));

        return expectedResults;
    }
}