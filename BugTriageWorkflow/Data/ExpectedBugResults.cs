using BugTriageWorkflow.Constants;
using BugTriageWorkflow.Models;

namespace BugTriageWorkflow.Data;

public static class ExpectedBugResults {
    public static Dictionary<string, ExpectedBugClassification> Create() {
        return new Dictionary<string, ExpectedBugClassification> {
            // -------------------------
            // Real / supported reports
            // -------------------------

            ["BUG-001"] = new() {
                Category = CategoryEnum.Backend,
                Urgency = UrgencyEnum.Medium,
                RecommendedRoute = RouteEnum.BackendTeam,
                EscalateToHuman = false,
                VerificationStatus = VerificationEnum.Supported,
                FalseReportRisk = FalseReportRiskEnum.Low
            },

            ["BUG-002"] = new() {
                Category = CategoryEnum.Frontend,
                Urgency = UrgencyEnum.Medium,
                RecommendedRoute = RouteEnum.FrontendTeam,
                EscalateToHuman = false,
                VerificationStatus = VerificationEnum.Supported,
                FalseReportRisk = FalseReportRiskEnum.Low
            },

            ["BUG-003"] = new() {
                Category = CategoryEnum.Backend,
                Urgency = UrgencyEnum.Medium,
                RecommendedRoute = RouteEnum.BackendTeam,
                EscalateToHuman = false,
                VerificationStatus = VerificationEnum.Supported,
                FalseReportRisk = FalseReportRiskEnum.Low
            },

            ["BUG-004"] = new() {
                Category = CategoryEnum.Backend,
                Urgency = UrgencyEnum.Medium,
                RecommendedRoute = RouteEnum.BackendTeam,
                EscalateToHuman = false,
                VerificationStatus = VerificationEnum.Supported,
                FalseReportRisk = FalseReportRiskEnum.Low
            },

            ["BUG-005"] = new() {
                Category = CategoryEnum.Infrastructure,
                Urgency = UrgencyEnum.Medium,
                RecommendedRoute = RouteEnum.InfrastructureTeam,
                EscalateToHuman = false,
                VerificationStatus = VerificationEnum.Supported,
                FalseReportRisk = FalseReportRiskEnum.Low
            },

            // -------------------------
            // Suspicious / contradicted reports
            // These are expected to escalate to human review.
            // -------------------------

            ["BUG-006"] = new() {
                Category = CategoryEnum.Frontend,
                Urgency = UrgencyEnum.High,
                RecommendedRoute = RouteEnum.HumanReview,
                EscalateToHuman = true,
                VerificationStatus = VerificationEnum.Contradicted,
                FalseReportRisk = FalseReportRiskEnum.High
            },

            ["BUG-007"] = new() {
                Category = CategoryEnum.Frontend,
                Urgency = UrgencyEnum.Medium,
                RecommendedRoute = RouteEnum.HumanReview,
                EscalateToHuman = true,
                VerificationStatus = VerificationEnum.Contradicted,
                FalseReportRisk = FalseReportRiskEnum.High
            },

            ["BUG-008"] = new() {
                Category = CategoryEnum.Frontend,
                Urgency = UrgencyEnum.Medium,
                RecommendedRoute = RouteEnum.HumanReview,
                EscalateToHuman = true,
                VerificationStatus = VerificationEnum.Contradicted,
                FalseReportRisk = FalseReportRiskEnum.High
            },

            ["BUG-009"] = new() {
                Category = CategoryEnum.Infrastructure,
                Urgency = UrgencyEnum.Medium,
                RecommendedRoute = RouteEnum.HumanReview,
                EscalateToHuman = true,
                VerificationStatus = VerificationEnum.Contradicted,
                FalseReportRisk = FalseReportRiskEnum.High
            },

            ["BUG-010"] = new() {
                Category = CategoryEnum.Frontend,
                Urgency = UrgencyEnum.Medium,
                RecommendedRoute = RouteEnum.HumanReview,
                EscalateToHuman = true,
                VerificationStatus = VerificationEnum.Contradicted,
                FalseReportRisk = FalseReportRiskEnum.High
            },

            // -------------------------
            // Ambiguous / incomplete reports
            // These usually have inconclusive evidence.
            // -------------------------

            ["BUG-011"] = new() {
                Category = CategoryEnum.Frontend,
                Urgency = UrgencyEnum.Low,
                RecommendedRoute = RouteEnum.FrontendTeam,
                EscalateToHuman = false,
                VerificationStatus = VerificationEnum.Inconclusive,
                FalseReportRisk = FalseReportRiskEnum.Medium
            },

            ["BUG-012"] = new() {
                Category = CategoryEnum.Frontend,
                Urgency = UrgencyEnum.Low,
                RecommendedRoute = RouteEnum.FrontendTeam,
                EscalateToHuman = false,
                VerificationStatus = VerificationEnum.Inconclusive,
                FalseReportRisk = FalseReportRiskEnum.Medium
            },

            ["BUG-013"] = new() {
                Category = CategoryEnum.Frontend,
                Urgency = UrgencyEnum.Low,
                RecommendedRoute = RouteEnum.FrontendTeam,
                EscalateToHuman = false,
                VerificationStatus = VerificationEnum.Supported,
                FalseReportRisk = FalseReportRiskEnum.Low
            },

            ["BUG-014"] = new() {
                Category = CategoryEnum.Infrastructure,
                Urgency = UrgencyEnum.Low,
                RecommendedRoute = RouteEnum.InfrastructureTeam,
                EscalateToHuman = false,
                VerificationStatus = VerificationEnum.Inconclusive,
                FalseReportRisk = FalseReportRiskEnum.Medium
            },

            ["BUG-015"] = new() {
                Category = CategoryEnum.Backend,
                Urgency = UrgencyEnum.Low,
                RecommendedRoute = RouteEnum.BackendTeam,
                EscalateToHuman = false,
                VerificationStatus = VerificationEnum.Inconclusive,
                FalseReportRisk = FalseReportRiskEnum.Medium
            }
        };
    }
}