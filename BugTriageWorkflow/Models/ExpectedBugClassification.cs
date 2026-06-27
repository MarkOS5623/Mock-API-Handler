using BugTriageWorkflow.Constants;

namespace BugTriageWorkflow.Models;

/// <summary>
/// Represents the expected correct classification for a bug report.
/// Used by the evaluation workflow to measure classifier accuracy.
/// These values are never exposed to the classifier agent.
/// </summary>
public class ExpectedBugClassification {
    /// <summary>
    /// Expected bug category.
    /// Examples: frontend, backend, infrastructure, unknown.
    /// </summary>
    public CategoryEnum Category { get; set; }

    /// <summary>
    /// Expected urgency level.
    /// Examples: low, medium, high.
    /// </summary>
    public UrgencyEnum Urgency { get; set; }

    /// <summary>
    /// Expected routing destination.
    /// Examples: frontend_team, backend_team,
    /// infrastructure_team, human_review.
    /// </summary>
    public RouteEnum RecommendedRoute { get; set; }

    /// <summary>
    /// Indicates whether the report is expected to require
    /// escalation for manual human review.
    /// </summary>
    public bool EscalateToHuman { get; set; }

    /// <summary>
    /// Expected evidence assessment.
    /// Examples:
    /// supported_by_evidence,
    /// contradicted_by_evidence,
    /// inconclusive.
    /// </summary>
    public VerificationEnum VerificationStatus { get; set; }

    /// <summary>
    /// Expected likelihood that the report is inaccurate
    /// or unsupported by the available evidence.
    /// Examples: low, medium, high.
    /// </summary>
    public FalseReportRiskEnum FalseReportRisk { get; set; }
}