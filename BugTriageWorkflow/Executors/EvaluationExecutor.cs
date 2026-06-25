using BugTriageWorkflow.Constants;
using BugTriageWorkflow.Models;

namespace BugTriageWorkflow.Executors;

public static class EvaluationExecutor {
    public static EvaluationResult Execute(ExpectedBugClassification expected, BugClassification actual) {
        // -------------------------
        // Confidence scoring
        // -------------------------
        //
        // These scores use the classifier's numeric confidence values.
        //

        var categoryConfidenceScore = VectorScore(TriageLabels.IndexOf(expected.Category, TriageLabels.Category.All), actual.CategoryVector);

        var urgencyConfidenceScore = DistanceScore(LabelToValue(expected.Urgency), actual.UrgencyValue);

        var routeConfidenceScore = VectorScore(TriageLabels.IndexOf(expected.RecommendedRoute, TriageLabels.Route.All), actual.RecommendedRouteVector);

        var escalationConfidenceScore = DistanceScore(BoolToValue(expected.EscalateToHuman), actual.EscalateToHumanValue);

        var verificationConfidenceScore = VectorScore(TriageLabels.IndexOf(expected.VerificationStatus, TriageLabels.Verification.All), actual.VerificationStatusVector);

        var falseReportRiskConfidenceScore = DistanceScore(LabelToValue(expected.FalseReportRisk), actual.FalseReportRiskValue);

        var confidenceScore =
            (categoryConfidenceScore
             + urgencyConfidenceScore
             + routeConfidenceScore
             + escalationConfidenceScore
             + verificationConfidenceScore
             + falseReportRiskConfidenceScore) / 6.0;

        // -------------------------
        // Exact-match accuracy
        // -------------------------
        //
        // These check whether the classifier selected the exact expected value.
        //

        var categoryCorrect = SameLabel(expected.Category, actual.Category);
        var urgencyCorrect = SameLabel(expected.Urgency, actual.Urgency);
        var routeCorrect = SameLabel(expected.RecommendedRoute, actual.RecommendedRoute);
        var escalationCorrect = expected.EscalateToHuman == actual.EscalateToHuman;
        var verificationCorrect = SameLabel(expected.VerificationStatus, actual.VerificationStatus);
        var falseReportRiskCorrect = SameLabel(expected.FalseReportRisk, actual.FalseReportRisk);

        var totalCorrectPredictions =
            BoolScore(categoryCorrect)
            + BoolScore(urgencyCorrect)
            + BoolScore(routeCorrect)
            + BoolScore(escalationCorrect)
            + BoolScore(verificationCorrect)
            + BoolScore(falseReportRiskCorrect);

        const int totalPredictions = 6;

        var accuracyScore = totalCorrectPredictions / (double)totalPredictions;

        // -------------------------
        // Quality score
        // -------------------------
        //
        // Quality balances confidence and accuracy.
        //

        var qualityScore = (confidenceScore + accuracyScore) / 2.0;

        return new EvaluationResult {
            CategoryConfidenceScore = categoryConfidenceScore,
            UrgencyConfidenceScore = urgencyConfidenceScore,
            RouteConfidenceScore = routeConfidenceScore,
            EscalationConfidenceScore = escalationConfidenceScore,
            VerificationConfidenceScore = verificationConfidenceScore,
            FalseReportRiskConfidenceScore = falseReportRiskConfidenceScore,
            ConfidenceScore = confidenceScore,

            CategoryCorrect = categoryCorrect,
            UrgencyCorrect = urgencyCorrect,
            RouteCorrect = routeCorrect,
            EscalationCorrect = escalationCorrect,
            VerificationCorrect = verificationCorrect,
            FalseReportRiskCorrect = falseReportRiskCorrect,
            AccuracyScore = accuracyScore,

            QualityScore = qualityScore,

            TotalCorrectPredictions = totalCorrectPredictions,
            TotalPredictions = totalPredictions
        };
    }

    private static double VectorScore(int expectedIndex, List<double>? actualVector) {
        if (actualVector == null) return 0.0;

        if (expectedIndex < 0 || expectedIndex >= actualVector.Count) return 0.0;

        return Clamp(actualVector[expectedIndex]);
    }

    private static double DistanceScore(double expected, double actual) { return Clamp(1.0 - Math.Abs(expected - actual)); }

    private static double LabelToValue(string value) {
        return value.Trim().ToLowerInvariant() switch {
            TriageLabels.Level.Low => 0.0,
            TriageLabels.Level.Medium => 0.5,
            TriageLabels.Level.High => 1.0,
            _ => 0.0
        };
    }

    private static double BoolToValue(bool value) { return value ? 1.0 : 0.0;}

    private static int BoolScore(bool value) { return value ? 1 : 0; }

    private static bool SameLabel(string expected, string actual) {
        return string.Equals(
            expected.Trim(),
            actual.Trim(),
            StringComparison.OrdinalIgnoreCase);
    }

    private static double Clamp(double value) { return Math.Max(0.0, Math.Min(1.0, value)); }
}