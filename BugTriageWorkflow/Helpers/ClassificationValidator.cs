using BugTriageWorkflow.Constants;
using BugTriageWorkflow.Models;

namespace BugTriageWorkflow.Helpers;

public static class ClassificationValidator {
    // Allows tiny floating-point / LLM formatting error.
    // Example: 0.33 + 0.33 + 0.34 = 1.00 is fine.
    private const double VectorSumTolerance = 0.05;

    public static bool IsValid(BugClassification? classification, out string error) {

        if (classification == null) return Fail("Classification is null.", out error);

        // -------------------------
        // Category validation
        // -------------------------

        if (string.IsNullOrWhiteSpace(classification.Category)) return Fail("Category is missing.", out error);

        var categoryIndex = TriageLabels.IndexOf(classification.Category, TriageLabels.Category.All);

        if (categoryIndex == -1) return Fail($"Unknown category: {classification.Category}", out error);

        if (!IsValidVector(classification.CategoryVector, TriageLabels.Category.All.Length, categoryIndex, "Category vector", out error))
            return false;

        // -------------------------
        // Urgency validation
        // -------------------------

        if (string.IsNullOrWhiteSpace(classification.Urgency)) return Fail("Urgency is missing.", out error);

        if (TriageLabels.IndexOf(classification.Urgency, TriageLabels.Level.All) == -1) return Fail($"Unknown urgency: {classification.Urgency}", out error);

        if (!IsProbability(classification.UrgencyValue, "Urgency value", out error)) return false;

        // Makes sure the label and value agree.
        // Example invalid:
        // urgency = "high", urgency_value = 0.10
        if (!IsValidLevelValue(classification.Urgency, classification.UrgencyValue, "Urgency", out error)) return false;

        // -------------------------
        // Missing info validation
        // -------------------------

        if (classification.MissingInfo == null) return Fail("Missing info list is null.", out error);

        // -------------------------
        // Route validation
        // -------------------------

        if (string.IsNullOrWhiteSpace(classification.RecommendedRoute)) return Fail("Recommended route is missing.", out error);

        var routeIndex = TriageLabels.IndexOf(classification.RecommendedRoute, TriageLabels.Route.All);

        if (routeIndex == -1) return Fail($"Unknown route: {classification.RecommendedRoute}", out error);

        if (!IsValidVector(classification.RecommendedRouteVector, TriageLabels.Route.All.Length, routeIndex, "Recommended route vector", out error))
            return false;

        // -------------------------
        // Escalation validation
        // -------------------------

        if (!IsProbability(classification.EscalateToHumanValue, "Escalation value", out error))
            return false;

        // Makes sure the boolean and value agree.
        // Example invalid:
        // escalate_to_human = true, escalate_to_human_value = 0.20
        if (!IsValidEscalationValue(classification.EscalateToHuman, classification.EscalateToHumanValue, out error))
            return false;

        // -------------------------
        // Verification validation
        // -------------------------

        if (string.IsNullOrWhiteSpace(classification.VerificationStatus)) return Fail("Verification status is missing.", out error);

        var verificationIndex = TriageLabels.IndexOf(classification.VerificationStatus, TriageLabels.Verification.All);

        if (verificationIndex == -1)
            return Fail($"Unknown verification status: {classification.VerificationStatus}", out error);

        if (!IsValidVector(classification.VerificationStatusVector, TriageLabels.Verification.All.Length, verificationIndex, "Verification status vector", out error))
            return false;

        // -------------------------
        // False report risk validation
        // -------------------------

        if (string.IsNullOrWhiteSpace(classification.FalseReportRisk)) return Fail("False report risk is missing.", out error);

        if (TriageLabels.IndexOf(classification.FalseReportRisk, TriageLabels.Level.All) == -1)
            return Fail($"Unknown false report risk: {classification.FalseReportRisk}", out error);

        if (!IsProbability(classification.FalseReportRiskValue, "False report risk value", out error)) return false;

        // Makes sure the label and value agree.
        // Example invalid:
        // false_report_risk = "low", false_report_risk_value = 0.90
        if (!IsValidLevelValue(classification.FalseReportRisk, classification.FalseReportRiskValue, "False report risk", out error))
            return false;

        // -------------------------
        // Cross-field consistency validation
        // -------------------------

        if (!IsValidCrossFieldConsistency(classification, out error)) return false;

        // -------------------------
        // Reason validation
        // -------------------------

        if (string.IsNullOrWhiteSpace(classification.VerificationReason)) return Fail("Verification reason is missing.", out error);

        return Success(out error);
    }

    private static bool IsValidVector(List<double>? vector, int expectedLength, int selectedIndex, string vectorName, out string error) {

        if (vector == null) return Fail($"{vectorName} is null.", out error);

        if (vector.Count != expectedLength) return Fail($"{vectorName} must contain {expectedLength} values.", out error);

        if (vector.Any(value => value < 0 || value > 1)) return Fail($"{vectorName} values must be between 0 and 1.", out error);

        var sum = vector.Sum();

        if (Math.Abs(sum - 1.0) > VectorSumTolerance) return Fail($"{vectorName} must sum to approximately 1.0. Actual sum: {sum:0.00}.", out error);

        var maxIndex = vector.IndexOf(vector.Max());

        if (maxIndex != selectedIndex) return Fail($"{vectorName} highest-confidence index ({maxIndex}) does not match selected label index ({selectedIndex}).", out error);

        return Success(out error);
    }

    private static bool IsProbability(double value, string valueName, out string error) {

        return value switch {
            < 0 or > 1 => Fail($"{valueName} must be between 0 and 1.", out error),
            _ => Success(out error)
        };
    }

    private static bool IsValidLevelValue(string label, double value, string fieldName, out string error) {

        var normalized = Normalize(label);

        return normalized switch {
            TriageLabels.Level.Low when value is >= TriageRanges.LowMin and <= TriageRanges.LowMax => Success(out error),

            TriageLabels.Level.Medium when value is >= TriageRanges.MediumMin and <= TriageRanges.MediumMax => Success(out error),

            TriageLabels.Level.High when value is >= TriageRanges.HighMin and <= TriageRanges.HighMax => Success(out error),

            TriageLabels.Level.Low =>
                Fail(
                    $"{fieldName} value {value:0.00} does not match label '{TriageLabels.Level.Low}'. Expected range: {TriageRanges.Format(TriageRanges.LowMin, TriageRanges.LowMax)}.",
                    out error),

            TriageLabels.Level.Medium =>
                Fail(
                    $"{fieldName} value {value:0.00} does not match label '{TriageLabels.Level.Medium}'. Expected range: {TriageRanges.Format(TriageRanges.MediumMin, TriageRanges.MediumMax)}.",
                    out error),

            TriageLabels.Level.High =>
                Fail(
                    $"{fieldName} value {value:0.00} does not match label '{TriageLabels.Level.High}'. Expected range: {TriageRanges.Format(TriageRanges.HighMin, TriageRanges.HighMax)}.",
                    out error),

            _ =>
                Fail($"Unknown {fieldName.ToLowerInvariant()} label: {label}", out error)
        };
    }

    private static bool IsValidEscalationValue(bool escalateToHuman, double value, out string error) {

        if (!escalateToHuman && value is >= TriageRanges.FalseEscalationMin and <= TriageRanges.FalseEscalationMax) return Success(out error);

        if (escalateToHuman && value is >= TriageRanges.TrueEscalationMin and <= TriageRanges.TrueEscalationMax) return Success(out error);

        var expectedRange = escalateToHuman
            ? TriageRanges.Format(TriageRanges.TrueEscalationMin, TriageRanges.TrueEscalationMax)
            : TriageRanges.Format(TriageRanges.FalseEscalationMin, TriageRanges.FalseEscalationMax);

        return Fail($"Escalation value {value:0.00} does not match escalate_to_human = {escalateToHuman}. Expected range: {expectedRange}.", out error);
    }

    private static bool IsValidCrossFieldConsistency(BugClassification classification, out string error) {

        var category = Normalize(classification.Category);
        var route = Normalize(classification.RecommendedRoute);
        var verification = Normalize(classification.VerificationStatus);
        var falseReportRisk = Normalize(classification.FalseReportRisk);

        // Contradicted evidence should always go to human review.
        if (verification == TriageLabels.Verification.Contradicted) {
            if (!classification.EscalateToHuman) return Fail("Contradicted evidence must set escalate_to_human = true.", out error);

            if (route != TriageLabels.Route.HumanReview) return Fail("Contradicted evidence must route to human_review.", out error);

            if (falseReportRisk != TriageLabels.Level.High)  return Fail("Contradicted evidence must use false_report_risk = high.", out error);
        }

        // High false-report risk should always go to human review.
        if (falseReportRisk == TriageLabels.Level.High) {
            if (!classification.EscalateToHuman) return Fail("High false report risk must set escalate_to_human = true.", out error);

            if (route != TriageLabels.Route.HumanReview) return Fail("High false report risk must route to human_review.", out error);
        }

        // Unknown category should not be assigned directly to a product team.
        if (category == TriageLabels.Category.Unknown) {
            if (!classification.EscalateToHuman) return Fail("Unknown category must set escalate_to_human = true.", out error);

            if (route != TriageLabels.Route.HumanReview) return Fail("Unknown category must route to human_review.", out error);
        }

        // Supported evidence and high false-report risk contradict each other.
        if (verification == TriageLabels.Verification.Supported && falseReportRisk == TriageLabels.Level.High) {
            return Fail("Supported evidence cannot have false_report_risk = high.", out error);
        }

        // Contradicted evidence and low false-report risk contradict each other.
        if (verification == TriageLabels.Verification.Contradicted && falseReportRisk == TriageLabels.Level.Low) {
            return Fail("Contradicted evidence cannot have false_report_risk = low.", out error);
        }

        return Success(out error);
    }

    private static string Normalize(string value) { return value.Trim().ToLowerInvariant();}

    private static bool Success(out string error) {
        error = "";
        return true;
    }

    private static bool Fail(string message, out string error) {
        error = message;
        return false;
    }
}