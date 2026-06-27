using BugTriageWorkflow.Helpers;
using BugTriageWorkflow.Models;
using BugTriageWorkflow.Constants;

namespace BugTriageWorkflow.Executors;

public static class RouterExecutor {
    // Human-facing team names.
    // These are display values, not LLM JSON labels.
    private const string FrontendTeamDisplay = "Frontend Team";
    private const string BackendTeamDisplay = "Backend Team";
    private const string InfrastructureTeamDisplay = "Infrastructure Team";
    private const string HumanReviewDisplay = "Human Review";
    private const string UnknownTeamDisplay = "Unknown Team";
    private const string UnknownRouteDisplay = "Unknown Route";

    // Options shown when a human manually chooses a team.
    // We keep these separate from TriageLabels.Route because those labels are
    // machine/JSON values like "frontend_team", while these are readable names.
    private static readonly string[] AssignableTeams = [
        FrontendTeamDisplay,
        BackendTeamDisplay,
        InfrastructureTeamDisplay,
        UnknownTeamDisplay
    ];

    // Human-facing urgency names.
    // This list is for manual console selection, so it uses display casing.
    // "Critical" is a manual-only option and is not part of the LLM output labels.
    private static readonly string[] UrgencyOptions = [
        "Low",
        "Medium",
        "High",
        "Critical"
    ];

    public static RouteResult Execute(
        BugClassification classification,
        BugReport report,
        bool handleEscalationsManually) {

        // If the classifier recommends escalation and the app is configured
        // for manual handling, show the report to a human and collect decisions.
        if (classification.EscalateToHuman && handleEscalationsManually)  return HandleHumanEscalation(classification, report);

        // Otherwise, trust the classifier's recommended route.
        return RouteAutomatically(classification);
    }

    private static RouteResult HandleHumanEscalation(
        BugClassification classification,
        BugReport report) {

        Logger.Info("Bug marked for human escalation.");
        Logger.Info("");
        Logger.Info("RAW BUG:");
        Logger.Info(report.RawText.Trim());

        if (!string.IsNullOrWhiteSpace(report.Evidence)) {
            Logger.Info("");
            Logger.Info("EVIDENCE:");
            Logger.Info(report.Evidence.Trim());
        }

        Logger.Info("");
        Logger.Info("CLASSIFIER SUMMARY:");
        Logger.Info($"Recommended Route: {classification.RecommendedRoute}");
        Logger.Info($"Urgency: {classification.Urgency}");
        Logger.Info($"False Report Risk: {classification.FalseReportRisk}");
        Logger.Info($"Verification Status: {classification.VerificationStatus}");
        Logger.Info($"Reason: {classification.VerificationReason}");

        // Convert the agent's machine-readable route label into the same
        // display format used by the human menu.
        var recommendedTeam = CategoryToTeamDisplay(classification.Category);

        // Convert the agent's urgency label into the same display format
        // used by the human urgency menu.
        var recommendedUrgency = UrgencyToDisplayLabel(classification.Urgency);

        // Human overrides / confirms routing information.
        // The recommended values are shown beside the choices.
        var selectedTeam = ReadTeamChoice(recommendedTeam);
        var selectedUrgency = ReadUrgencyChoice(recommendedUrgency);
        var isFalseReport = ReadFalseReportChoice(classification.FalseReportRisk);

        return new RouteResult {
            Route = selectedTeam,
            RoutedBy = RouteSource.Human,
            Reason = "Classification required human escalation.",
            HumanSelectedUrgency = selectedUrgency,
            HumanMarkedFalseReport = isFalseReport
        };
    }

    private static RouteResult RouteAutomatically(
        BugClassification classification) {

        // Convert machine label like "backend_team" into display text like
        // "Backend Team" before storing/logging the route result.
        var route = RouteToDisplay(classification.RecommendedRoute);

        return new RouteResult {
            Route = route,
            RoutedBy = RouteSource.Automatic,
            Reason = "Classification did not require manual human routing.",
            HumanSelectedUrgency = "",
            HumanMarkedFalseReport = null
        };
    }

    private static string ReadTeamChoice(string recommendedTeam) {
        Logger.Info("");
        Logger.Info($"Choose team: Agent recommends {recommendedTeam}");

        for (var i = 0; i < AssignableTeams.Length; i++) {
            var marker = AssignableTeams[i] == recommendedTeam ? " <- recommended" : "";
            Logger.Info($"{i + 1} - {AssignableTeams[i]}{marker}");
        }

        var defaultIndex = GetOptionIndexOrDefault(AssignableTeams, recommendedTeam, defaultIndex: 3);

        return ReadChoice(prompt: $"Choice [default: {defaultIndex + 1}]: ", options: AssignableTeams, defaultIndex: defaultIndex);
    }

    private static string ReadUrgencyChoice(string recommendedUrgency) {
        Logger.Info("");
        Logger.Info($"Choose urgency: Agent recommends {recommendedUrgency}");

        for (var i = 0; i < UrgencyOptions.Length; i++) {
            var marker = UrgencyOptions[i] == recommendedUrgency ? " <- recommended" : "";
            Logger.Info($"{i + 1} - {UrgencyOptions[i]}{marker}");
        }

        var defaultIndex = GetOptionIndexOrDefault(UrgencyOptions, recommendedUrgency, defaultIndex: 1);

        return ReadChoice(prompt: $"Choice [default: {defaultIndex + 1}]: ", options: UrgencyOptions, defaultIndex: defaultIndex);
    }

    private static bool ReadFalseReportChoice(FalseReportRiskEnum falseReportRisk) {
        var recommendedFalseReport = falseReportRisk == FalseReportRiskEnum.High;

        Logger.Info("");
        Logger.Info($"Agent false-report recommendation: {(recommendedFalseReport ? "Yes" : "No")}");

        while (true) {
            Console.Write($"Is this a false report? [default: {(recommendedFalseReport ? "Y" : "N")}]: ");

            var input = Console.ReadLine()?.Trim().ToLowerInvariant();

            // Empty input chooses the agent's recommendation.
            if (string.IsNullOrWhiteSpace(input)) return recommendedFalseReport;

            if (input is "y" or "yes") return true;

            if (input is "n" or "no") return false;

            Logger.Info("Invalid choice. Please enter y or n.");
        }
    }

    private static string ReadChoice(string prompt, string[] options, int defaultIndex) {
        while (true) {
            Console.Write(prompt);
            var input = Console.ReadLine()?.Trim();

            // Empty input chooses the configured default option.
            if (string.IsNullOrWhiteSpace(input)) {
                Logger.Info($"No choice entered. Defaulting to {options[defaultIndex]}.");
                return options[defaultIndex];
            }

            if (
                int.TryParse(input, out var choice) &&
                choice >= 1 &&
                choice <= options.Length) {
                return options[choice - 1];
            }

            Logger.Info($"Invalid choice. Please enter a number between 1 and {options.Length}.");
        }
    }

    private static string RouteToDisplay(RouteEnum route) {
        return route switch {
            RouteEnum.FrontendTeam => FrontendTeamDisplay,
            RouteEnum.BackendTeam => BackendTeamDisplay,
            RouteEnum.InfrastructureTeam => InfrastructureTeamDisplay,
            RouteEnum.HumanReview => HumanReviewDisplay,
            _ => UnknownRouteDisplay
        };
    }

    private static string CategoryToTeamDisplay(CategoryEnum category) {
        return category switch {
            CategoryEnum.Frontend => FrontendTeamDisplay,
            CategoryEnum.Backend => BackendTeamDisplay,
            CategoryEnum.Infrastructure => InfrastructureTeamDisplay,
            CategoryEnum.Unknown => UnknownTeamDisplay,
            _ => UnknownTeamDisplay
        };
    }

    private static string UrgencyToDisplayLabel(UrgencyEnum urgency) {
        return urgency switch {
            UrgencyEnum.Low => "Low",
            UrgencyEnum.Medium => "Medium",
            UrgencyEnum.High => "High",
            _ => ""
        };
    }

    private static int GetOptionIndexOrDefault(string[] options, string value, int defaultIndex) {
        var index = Array.FindIndex( options, option => string.Equals(option, value, StringComparison.OrdinalIgnoreCase));
        return index >= 0 ? index : defaultIndex;
    }
}