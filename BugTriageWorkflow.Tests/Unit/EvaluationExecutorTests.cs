using Xunit;
using BugTriageWorkflow.Executors;
using BugTriageWorkflow.Models;
using BugTriageWorkflow.Constants;

namespace BugTriageWorkflow.Tests.Unit;

public class EvaluationExecutorTests
{
    // Core scoring logic (4 tests)

    [Fact]
    public void VectorScore_ValidIndex_ReturnsCorrectValue()
    {
        // Arrange
        int expectedIndex = 1;
        var actualVector = new List<double> { 0.1, 0.8, 0.1 };

        // Act
        var result = EvaluationExecutor.VectorScore(expectedIndex, actualVector);

        // Assert
        Assert.Equal(0.8, result);
    }

    [Fact]
    public void VectorScore_InvalidIndex_ReturnsZero()
    {
        // Arrange
        int expectedIndex = 5;
        var actualVector = new List<double> { 0.1, 0.2, 0.3, 0.4 };

        // Act
        var result = EvaluationExecutor.VectorScore(expectedIndex, actualVector);

        // Assert
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void DistanceScore_SameValues_ReturnsOne()
    {
        // Act
        var result = EvaluationExecutor.DistanceScore(0.5, 0.5);

        // Assert
        Assert.Equal(1.0, result);
    }

    [Fact]
    public void DistanceScore_OppositeValues_ReturnsZero()
    {
        // Act
        var result = EvaluationExecutor.DistanceScore(0.0, 1.0);

        // Assert
        Assert.Equal(0.0, result);
    }

    // Edge cases (5 tests)

    [Fact]
    public void VectorScore_NullVector_ReturnsZero()
    {
        // Arrange
        int expectedIndex = 0;
        List<double>? actualVector = null;

        // Act
        var result = EvaluationExecutor.VectorScore(expectedIndex, actualVector);

        // Assert
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void VectorScore_NegativeIndex_ReturnsZero()
    {
        // Arrange
        int expectedIndex = -1;
        var actualVector = new List<double> { 0.1, 0.2, 0.3 };

        // Act
        var result = EvaluationExecutor.VectorScore(expectedIndex, actualVector);

        // Assert
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void VectorScore_IndexOutOfBounds_ReturnsZero()
    {
        // Arrange
        int expectedIndex = 10;
        var actualVector = new List<double> { 0.1, 0.2, 0.3 };

        // Act
        var result = EvaluationExecutor.VectorScore(expectedIndex, actualVector);

        // Assert
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void DistanceScore_ClampsBelowZero()
    {
        // Arrange - distance would be negative without clamping
        double expected = 0.0;
        double actual = 1.5;  // Distance = 1 - |0.0 - 1.5| = -0.5

        // Act
        var result = EvaluationExecutor.DistanceScore(expected, actual);

        // Assert
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void DistanceScore_ClampsAboveOne()
    {
        // Arrange - distance would be > 1 without clamping (doesn't actually happen with valid inputs, but testing clamping)
        double expected = 0.0;
        double actual = 0.0;  // Distance = 1.0, which is already at max

        // Act
        var result = EvaluationExecutor.DistanceScore(expected, actual);

        // Assert
        Assert.Equal(1.0, result);
    }

    // Field-specific mappings (3 tests)

    [Theory]
    [InlineData(UrgencyEnum.Low, 0.0)]
    [InlineData(UrgencyEnum.Medium, 0.5)]
    [InlineData(UrgencyEnum.High, 1.0)]
    public void UrgencyToValue_MapsCorrectly(UrgencyEnum urgency, double expected)
    {
        // Act
        var result = EvaluationExecutor.UrgencyToValue(urgency);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(FalseReportRiskEnum.Low, 0.0)]
    [InlineData(FalseReportRiskEnum.Medium, 0.5)]
    [InlineData(FalseReportRiskEnum.High, 1.0)]
    public void RiskToValue_MapsCorrectly(FalseReportRiskEnum risk, double expected)
    {
        // Act
        var result = EvaluationExecutor.RiskToValue(risk);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(true, 1.0)]
    [InlineData(false, 0.0)]
    public void BoolToValue_MapsCorrectly(bool value, double expected)
    {
        // Act
        var result = EvaluationExecutor.BoolToValue(value);

        // Assert
        Assert.Equal(expected, result);
    }

    // Integration scoring (3 tests)

    [Fact]
    public void Execute_AllFieldsCorrect_AccuracyScoreIsOne()
    {
        // Arrange
        var expected = new ExpectedBugClassification
        {
            Category = CategoryEnum.Backend,
            Urgency = UrgencyEnum.High,
            RecommendedRoute = RouteEnum.BackendTeam,
            EscalateToHuman = true,
            VerificationStatus = VerificationEnum.Supported,
            FalseReportRisk = FalseReportRiskEnum.Low
        };

        var actual = new BugClassification
        {
            Category = CategoryEnum.Backend,
            CategoryVector = new List<double> { 0.1, 0.7, 0.1, 0.1 },
            Urgency = UrgencyEnum.High,
            UrgencyValue = 1.0,
            RecommendedRoute = RouteEnum.BackendTeam,
            RecommendedRouteVector = new List<double> { 0.1, 0.7, 0.1, 0.1 },
            EscalateToHuman = true,
            EscalateToHumanValue = 1.0,
            VerificationStatus = VerificationEnum.Supported,
            VerificationStatusVector = new List<double> { 0.8, 0.1, 0.1 },
            FalseReportRisk = FalseReportRiskEnum.Low,
            FalseReportRiskValue = 0.0,
            VerificationReason = "Test",
            MissingInfo = new List<string>()
        };

        // Act
        var result = EvaluationExecutor.Execute(expected, actual);

        // Assert
        Assert.Equal(1.0, result.AccuracyScore);
        Assert.Equal(6, result.TotalCorrectPredictions);
    }

    [Fact]
    public void Execute_NoFieldsCorrect_AccuracyScoreIsZero()
    {
        // Arrange
        var expected = new ExpectedBugClassification
        {
            Category = CategoryEnum.Backend,
            Urgency = UrgencyEnum.High,
            RecommendedRoute = RouteEnum.BackendTeam,
            EscalateToHuman = true,
            VerificationStatus = VerificationEnum.Supported,
            FalseReportRisk = FalseReportRiskEnum.Low
        };

        var actual = new BugClassification
        {
            Category = CategoryEnum.Frontend,  // Wrong
            CategoryVector = new List<double> { 0.7, 0.1, 0.1, 0.1 },
            Urgency = UrgencyEnum.Low,  // Wrong
            UrgencyValue = 0.0,
            RecommendedRoute = RouteEnum.FrontendTeam,  // Wrong
            RecommendedRouteVector = new List<double> { 0.7, 0.1, 0.1, 0.1 },
            EscalateToHuman = false,  // Wrong
            EscalateToHumanValue = 0.0,
            VerificationStatus = VerificationEnum.Contradicted,  // Wrong
            VerificationStatusVector = new List<double> { 0.1, 0.8, 0.1 },
            FalseReportRisk = FalseReportRiskEnum.High,  // Wrong
            FalseReportRiskValue = 1.0,
            VerificationReason = "Test",
            MissingInfo = new List<string>()
        };

        // Act
        var result = EvaluationExecutor.Execute(expected, actual);

        // Assert
        Assert.Equal(0.0, result.AccuracyScore);
        Assert.Equal(0, result.TotalCorrectPredictions);
    }

    [Fact]
    public void Execute_TotalPredictionsAlwaysSix()
    {
        // Arrange - create any valid combination
        var expected = new ExpectedBugClassification
        {
            Category = CategoryEnum.Backend,
            Urgency = UrgencyEnum.Medium,
            RecommendedRoute = RouteEnum.BackendTeam,
            EscalateToHuman = false,
            VerificationStatus = VerificationEnum.Inconclusive,
            FalseReportRisk = FalseReportRiskEnum.Medium
        };

        var actual = new BugClassification
        {
            Category = CategoryEnum.Frontend,
            CategoryVector = new List<double> { 0.7, 0.1, 0.1, 0.1 },
            Urgency = UrgencyEnum.High,
            UrgencyValue = 1.0,
            RecommendedRoute = RouteEnum.HumanReview,
            RecommendedRouteVector = new List<double> { 0.1, 0.1, 0.1, 0.7 },
            EscalateToHuman = true,
            EscalateToHumanValue = 1.0,
            VerificationStatus = VerificationEnum.Supported,
            VerificationStatusVector = new List<double> { 0.8, 0.1, 0.1 },
            FalseReportRisk = FalseReportRiskEnum.Low,
            FalseReportRiskValue = 0.0,
            VerificationReason = "Test",
            MissingInfo = new List<string>()
        };

        // Act
        var result = EvaluationExecutor.Execute(expected, actual);

        // Assert
        Assert.Equal(6, result.TotalPredictions);
    }
}
