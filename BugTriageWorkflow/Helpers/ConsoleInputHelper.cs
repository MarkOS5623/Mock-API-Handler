using BugTriageWorkflow.Prompts;

namespace BugTriageWorkflow.Helpers;

/// <summary>
/// Reads workflow configuration choices from the console.
/// Keeps user interaction logic separate from the main workflow.
/// </summary>
public static class ConsoleInputHelper {
    /// <summary>
    /// Reads the number of keywords to extract during preprocessing.
    /// Falls back to the default value when the input is empty or invalid.
    /// </summary>
    public static int ReadKeywordCount(int defaultKeywordCount) {
        Console.Write($"Enter keyword count [{defaultKeywordCount}]: ");
        var input = Console.ReadLine();

        return int.TryParse(input, out var keywordCount) && keywordCount > 0
            ? keywordCount
            : defaultKeywordCount;
    }

    /// <summary>
    /// Reads the prompt type used by the classifier.
    /// Defaults to the detailed prompt when no valid choice is entered.
    /// </summary>
    public static PromptType ReadPromptType() {
        Console.WriteLine("Choose prompt type:");
        Console.WriteLine("1 - Detailed");
        Console.WriteLine("2 - Medium");
        Console.WriteLine("3 - Vague");
        Console.Write("Choice [1]: ");

        var input = Console.ReadLine();

        return input switch {
            "2" => PromptType.Medium,
            "3" => PromptType.Vague,
            _ => PromptType.Detailed
        };
    }

    /// <summary>
    /// Reads which scenario group should be evaluated.
    /// Defaults to all scenarios when no valid choice is entered.
    /// </summary>
    public static string ReadScenarioName() {
        Console.WriteLine("Choose scenario:");
        Console.WriteLine("1 - Evidence Supports Bug");
        Console.WriteLine("2 - Evidence Contradicts Bug");
        Console.WriteLine("3 - Evidence Mixed Or Inconclusive");
        Console.WriteLine("4 - All");
        Console.Write("Choice [4]: ");

        var input = Console.ReadLine();

        return input switch {
            "1" => "Evidence Supports Bug",
            "2" => "Evidence Contradicts Bug",
            "3" => "Evidence Mixed Or Inconclusive",
            _ => "All"
        };
    }

    /// <summary>
    /// Reads whether escalate-to-human cases should pause for manual review.
    /// Defaults to automatic handling when the input is empty or invalid.
    /// </summary>
    public static bool ReadManualEscalationMode() {
        Console.Write("Handle escalate-to-human cases manually? [y/N]: ");
        var input = Console.ReadLine();

        return input?.Trim().Equals("y", StringComparison.OrdinalIgnoreCase) == true;
    }
}