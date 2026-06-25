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
                Category = TriageLabels.Category.Backend,
                Urgency = TriageLabels.Level.Medium,
                RecommendedRoute = TriageLabels.Route.BackendTeam,
                EscalateToHuman = false,
                VerificationStatus = TriageLabels.Verification.Supported,
                FalseReportRisk = TriageLabels.Level.Low
            },

            ["BUG-002"] = new() {
                Category = TriageLabels.Category.Frontend,
                Urgency = TriageLabels.Level.Medium,
                RecommendedRoute = TriageLabels.Route.FrontendTeam,
                EscalateToHuman = false,
                VerificationStatus = TriageLabels.Verification.Supported,
                FalseReportRisk = TriageLabels.Level.Low
            },

            ["BUG-003"] = new() {
                Category = TriageLabels.Category.Backend,
                Urgency = TriageLabels.Level.Medium,
                RecommendedRoute = TriageLabels.Route.BackendTeam,
                EscalateToHuman = false,
                VerificationStatus = TriageLabels.Verification.Supported,
                FalseReportRisk = TriageLabels.Level.Low
            },

            ["BUG-004"] = new() {
                Category = TriageLabels.Category.Backend,
                Urgency = TriageLabels.Level.Medium,
                RecommendedRoute = TriageLabels.Route.BackendTeam,
                EscalateToHuman = false,
                VerificationStatus = TriageLabels.Verification.Supported,
                FalseReportRisk = TriageLabels.Level.Low
            },

            ["BUG-005"] = new() {
                Category = TriageLabels.Category.Infrastructure,
                Urgency = TriageLabels.Level.Medium,
                RecommendedRoute = TriageLabels.Route.InfrastructureTeam,
                EscalateToHuman = false,
                VerificationStatus = TriageLabels.Verification.Supported,
                FalseReportRisk = TriageLabels.Level.Low
            },

            // -------------------------
            // Suspicious / contradicted reports
            // These are expected to escalate to human review.
            // -------------------------

            ["BUG-006"] = new() {
                Category = TriageLabels.Category.Frontend,
                Urgency = TriageLabels.Level.High,
                RecommendedRoute = TriageLabels.Route.HumanReview,
                EscalateToHuman = true,
                VerificationStatus = TriageLabels.Verification.Contradicted,
                FalseReportRisk = TriageLabels.Level.High
            },

            ["BUG-007"] = new() {
                Category = TriageLabels.Category.Frontend,
                Urgency = TriageLabels.Level.Medium,
                RecommendedRoute = TriageLabels.Route.HumanReview,
                EscalateToHuman = true,
                VerificationStatus = TriageLabels.Verification.Contradicted,
                FalseReportRisk = TriageLabels.Level.High
            },

            ["BUG-008"] = new() {
                Category = TriageLabels.Category.Frontend,
                Urgency = TriageLabels.Level.Medium,
                RecommendedRoute = TriageLabels.Route.HumanReview,
                EscalateToHuman = true,
                VerificationStatus = TriageLabels.Verification.Contradicted,
                FalseReportRisk = TriageLabels.Level.High
            },

            ["BUG-009"] = new() {
                Category = TriageLabels.Category.Infrastructure,
                Urgency = TriageLabels.Level.Medium,
                RecommendedRoute = TriageLabels.Route.HumanReview,
                EscalateToHuman = true,
                VerificationStatus = TriageLabels.Verification.Contradicted,
                FalseReportRisk = TriageLabels.Level.High
            },

            ["BUG-010"] = new() {
                Category = TriageLabels.Category.Frontend,
                Urgency = TriageLabels.Level.Medium,
                RecommendedRoute = TriageLabels.Route.HumanReview,
                EscalateToHuman = true,
                VerificationStatus = TriageLabels.Verification.Contradicted,
                FalseReportRisk = TriageLabels.Level.High
            },

            // -------------------------
            // Ambiguous / incomplete reports
            // These usually have inconclusive evidence.
            // -------------------------

            ["BUG-011"] = new() {
                Category = TriageLabels.Category.Frontend,
                Urgency = TriageLabels.Level.Low,
                RecommendedRoute = TriageLabels.Route.FrontendTeam,
                EscalateToHuman = false,
                VerificationStatus = TriageLabels.Verification.Inconclusive,
                FalseReportRisk = TriageLabels.Level.Medium
            },

            ["BUG-012"] = new() {
                Category = TriageLabels.Category.Frontend,
                Urgency = TriageLabels.Level.Low,
                RecommendedRoute = TriageLabels.Route.FrontendTeam,
                EscalateToHuman = false,
                VerificationStatus = TriageLabels.Verification.Inconclusive,
                FalseReportRisk = TriageLabels.Level.Medium
            },

            ["BUG-013"] = new() {
                Category = TriageLabels.Category.Frontend,
                Urgency = TriageLabels.Level.Low,
                RecommendedRoute = TriageLabels.Route.FrontendTeam,
                EscalateToHuman = false,
                VerificationStatus = TriageLabels.Verification.Supported,
                FalseReportRisk = TriageLabels.Level.Low
            },

            ["BUG-014"] = new() {
                Category = TriageLabels.Category.Infrastructure,
                Urgency = TriageLabels.Level.Low,
                RecommendedRoute = TriageLabels.Route.InfrastructureTeam,
                EscalateToHuman = false,
                VerificationStatus = TriageLabels.Verification.Inconclusive,
                FalseReportRisk = TriageLabels.Level.Medium
            },

            ["BUG-015"] = new() {
                Category = TriageLabels.Category.Backend,
                Urgency = TriageLabels.Level.Low,
                RecommendedRoute = TriageLabels.Route.BackendTeam,
                EscalateToHuman = false,
                VerificationStatus = TriageLabels.Verification.Inconclusive,
                FalseReportRisk = TriageLabels.Level.Medium
            }
        };
    }
}