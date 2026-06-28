using BugTriageWorkflow.Constants;
using BugTriageWorkflow.Models;

namespace BugTriageWorkflow.Helpers;

/// <summary>
/// Deterministic routing policy that applies business rules to classifier proposals.
/// Classifier proposes, policy decides.
/// </summary>
public static class RoutingPolicy
{
    /// <summary>
    /// Determines the final routing decision by applying business rules to the classifier's proposal.
    /// </summary>
    /// <param name="classification">The AI classifier's proposal containing classification data.</param>
    /// <returns>Tuple of (FinalRoute, Rationale) explaining the routing decision.</returns>
    public static (RouteEnum FinalRoute, string Rationale) DetermineRoute(BugClassification classification)
    {
        // Rule 1: Escalation flag override (highest priority)
        // Business requirement: EscalateToHuman flag forces human review
        if (classification.EscalateToHuman)
        {
            return (RouteEnum.HumanReview, "Escalation flag set by classifier");
        }

        // Rule 2: Contradicted verification (business requirement)
        // Business requirement: Contradicted evidence requires human review
        if (classification.VerificationStatus == VerificationEnum.Contradicted)
        {
            return (RouteEnum.HumanReview, "Contradicted evidence requires human review");
        }

        // Rule 3: Unknown category (triage requirement)
        // Business requirement: Unknown categories require human triage
        if (classification.Category == CategoryEnum.Unknown)
        {
            return (RouteEnum.HumanReview, "Unknown category requires human triage");
        }

        // Rule 4: Low confidence escalation (future enhancement)
        // Reserved for future implementation: confidence-based routing
        // Example: if (GetMaxConfidence(classification.RecommendedRouteVector) < 0.60)
        //     return (RouteEnum.HumanReview, "Low confidence route, escalating to human");

        // Rule 5: Default - accept classifier recommendation
        // No business rules triggered, use classifier's proposal
        return (classification.RecommendedRoute, "Classifier recommendation");
    }

    /// <summary>
    /// Helper method for future confidence-based routing.
    /// Extracts the maximum confidence value from the route probability vector.
    /// </summary>
    /// <param name="routeVector">Probability distribution over route options.</param>
    /// <returns>Maximum confidence value in the vector.</returns>
    private static double GetMaxConfidence(List<double> routeVector)
    {
        if (routeVector == null || routeVector.Count == 0)
        {
            return 0.0;
        }

        return routeVector.Max();
    }
}
