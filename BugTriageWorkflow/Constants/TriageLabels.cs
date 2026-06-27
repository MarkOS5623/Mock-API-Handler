namespace BugTriageWorkflow.Constants;

// Central place for all classifier labels.
// This avoids repeating strings like "backend", "frontend_team",
// "supported_by_evidence", etc. across validators, evaluators, prompts,
// and expected results.
public static class TriageLabels {
    public static class Category {
        public const string Frontend = "frontend";
        public const string Backend = "backend";
        public const string Infrastructure = "infrastructure";
        public const string Unknown = "unknown";

        // Order matters.
        // This must match the LLM's category_vector order.
        public static readonly string[] All = [
            Frontend,
            Backend,
            Infrastructure,
            Unknown
        ];

        // Enum-based access for type safety
        public static readonly CategoryEnum[] AllEnums = [
            CategoryEnum.Frontend,
            CategoryEnum.Backend,
            CategoryEnum.Infrastructure,
            CategoryEnum.Unknown
        ];
    }

    public static class Route {
        public const string FrontendTeam = "frontend_team";
        public const string BackendTeam = "backend_team";
        public const string InfrastructureTeam = "infrastructure_team";
        public const string HumanReview = "human_review";

        // Order matters.
        // This must match the LLM's recommended_route_vector order.
        public static readonly string[] All = [
            FrontendTeam,
            BackendTeam,
            InfrastructureTeam,
            HumanReview
        ];

        // Enum-based access for type safety
        public static readonly RouteEnum[] AllEnums = [
            RouteEnum.FrontendTeam,
            RouteEnum.BackendTeam,
            RouteEnum.InfrastructureTeam,
            RouteEnum.HumanReview
        ];
    }

    public static class Verification {
        public const string Supported = "supported_by_evidence";
        public const string Contradicted = "contradicted_by_evidence";
        public const string Inconclusive = "inconclusive";

        // Order matters.
        // This must match the LLM's verification_status_vector order.
        public static readonly string[] All = [
            Supported,
            Contradicted,
            Inconclusive
        ];

        // Enum-based access for type safety
        public static readonly VerificationEnum[] AllEnums = [
            VerificationEnum.Supported,
            VerificationEnum.Contradicted,
            VerificationEnum.Inconclusive
        ];
    }

    public static class Level {
        public const string Low = "low";
        public const string Medium = "medium";
        public const string High = "high";

        public static readonly string[] All = [
            Low,
            Medium,
            High
        ];

        // Enum-based access for type safety (used for Urgency and FalseReportRisk)
        public static readonly UrgencyEnum[] AllUrgencyEnums = [
            UrgencyEnum.Low,
            UrgencyEnum.Medium,
            UrgencyEnum.High
        ];

        public static readonly FalseReportRiskEnum[] AllRiskEnums = [
            FalseReportRiskEnum.Low,
            FalseReportRiskEnum.Medium,
            FalseReportRiskEnum.High
        ];
    }

    // Shared helper for converting a label into its vector index.
    // Case-insensitive so small casing differences do not break validation.
    public static int IndexOf(string value, string[] options) {
        return Array.FindIndex(
            options,
            option => string.Equals(
                option,
                value.Trim(),
                StringComparison.OrdinalIgnoreCase));
    }
    
    public static string AsPipeList(string[] options) {
        return string.Join(" | ", options);
    }

    public static string AsVectorOrder(string[] options) {
        return string.Join(", ", options);
    }
}