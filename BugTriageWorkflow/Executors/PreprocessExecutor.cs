using System.Text.RegularExpressions;
using BugTriageWorkflow.Models;

namespace BugTriageWorkflow.Executors;

public static class PreprocessExecutor {
    private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase) {
        "a", "an", "the", "that", "this", "these", "those",
        "is", "are", "was", "were", "be", "been", "being",
        "to", "of", "and", "or", "but", "in", "on", "at", "for",
        "from", "with", "without", "by", "as", "into", "during",
        "after", "before", "near", "over", "under", "through",

        "it", "its", "they", "them", "their", "we", "our", "you", "your",
        "user", "users", "report", "reported", "reporter", "says", "claim",
        "claims", "claimed", "issue", "problem", "bug",

        "can", "cannot", "could", "should", "would", "will", "may", "might",
        "must", "does", "did", "do", "done",

        "not", "no", "yes", "same", "other", "both", "all", "some",
        "several", "many", "one", "multiple",

        "show", "shows", "showed", "visible", "appears", "happens",
        "successfully", "successful", "failed", "failure",

        "page", "screen", "button", "click", "clicked",
        "logs", "log", "evidence", "test", "tests"
    };

    public static PreprocessedBugReport Execute(BugReport report, int keywordCount) {
        var cleanText = NormalizeText(report.RawText);
        var evidence = NormalizeText(report.Evidence ?? "");

        var combinedText = $"{cleanText} {evidence}";

        var keywords = ExtractKeywordsByFrequency(
            combinedText,
            keywordCount);

        return new PreprocessedBugReport {
            Id = report.Id,
            Reporter = report.Reporter,
            CleanText = cleanText,
            Evidence = evidence,
            Keywords = keywords
        };
    }

    private static string NormalizeText(string text) {
        return Regex.Replace(text.Trim(), @"\s+", " ");
    }

    private static List<string> ExtractKeywordsByFrequency(
        string text,
        int keywordCount) {

        return Regex.Matches(text.ToLowerInvariant(), @"\b[a-z][a-z0-9']*\b")
            .Select(match => match.Value.Trim('\''))
            .Where(word => word.Length >= 3)
            .Where(word => !StopWords.Contains(word))
            .GroupBy(word => word)
            .OrderByDescending(group => group.Count())
            .ThenBy(group => group.Key)
            .Take(keywordCount)
            .Select(group => group.Key)
            .ToList();
    }
}