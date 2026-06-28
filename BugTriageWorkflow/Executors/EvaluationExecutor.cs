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

        var categoryConfidenceScore = VectorScore(Array.IndexOf(TriageLabels.Category.AllEnums, expected.Category), actual.CategoryVector);

        var urgencyConfidenceScore = DistanceScore(UrgencyToValue(expected.Urgency), actual.UrgencyValue);

        var routeConfidenceScore = VectorScore(Array.IndexOf(TriageLabels.Route.AllEnums, expected.RecommendedRoute), actual.RecommendedRouteVector);

        var escalationConfidenceScore = DistanceScore(BoolToValue(expected.EscalateToHuman), actual.EscalateToHumanValue);

        var verificationConfidenceScore = VectorScore(Array.IndexOf(TriageLabels.Verification.AllEnums, expected.VerificationStatus), actual.VerificationStatusVector);

        var falseReportRiskConfidenceScore = DistanceScore(RiskToValue(expected.FalseReportRisk), actual.FalseReportRiskValue);

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

        var categoryCorrect = expected.Category == actual.Category;
        var urgencyCorrect = expected.Urgency == actual.Urgency;
        var routeCorrect = expected.RecommendedRoute == actual.RecommendedRoute;
        var escalationCorrect = expected.EscalateToHuman == actual.EscalateToHuman;
        var verificationCorrect = expected.VerificationStatus == actual.VerificationStatus;
        var falseReportRiskCorrect = expected.FalseReportRisk == actual.FalseReportRisk;

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

    internal static double VectorScore(int expectedIndex, List<double>? actualVector) {
        if (actualVector == null) return 0.0;

        if (expectedIndex < 0 || expectedIndex >= actualVector.Count) return 0.0;

        return Clamp(actualVector[expectedIndex]);
    }

    internal static double DistanceScore(double expected, double actual) { return Clamp(1.0 - Math.Abs(expected - actual)); }

    internal static double UrgencyToValue(UrgencyEnum value) {
        return value switch {
            UrgencyEnum.Low => 0.0,
            UrgencyEnum.Medium => 0.5,
            UrgencyEnum.High => 1.0,
            _ => 0.0
        };
    }

    internal static double RiskToValue(FalseReportRiskEnum value) {
        return value switch {
            FalseReportRiskEnum.Low => 0.0,
            FalseReportRiskEnum.Medium => 0.5,
            FalseReportRiskEnum.High => 1.0,
            _ => 0.0
        };
    }

    internal static double BoolToValue(bool value) { return value ? 1.0 : 0.0;}

    internal static int BoolScore(bool value) { return value ? 1 : 0; }

    internal static double Clamp(double value) { return Math.Max(0.0, Math.Min(1.0, value)); }
}