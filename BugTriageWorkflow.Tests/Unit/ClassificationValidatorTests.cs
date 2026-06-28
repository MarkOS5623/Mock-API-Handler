using Xunit;
using BugTriageWorkflow.Helpers;
using BugTriageWorkflow.Models;
using BugTriageWorkflow.Constants;

namespace BugTriageWorkflow.Tests.Unit;

public class ClassificationValidatorTests
{
    // Taxonomy validation (4 tests)

    [Fact]
    public void IsValid_InvalidCategory_ReturnsFalse()
    {
        // Arrange
        var classification = CreateValidClassification();
        classification.Category = (CategoryEnum)999; // Invalid enum value

        // Act
        var result = ClassificationValidator.IsValid(classification, out var error);

        // Assert
        Assert.False(result);
        Assert.Contains("Unknown category", error);
    }

    [Fact]
    public void IsValid_InvalidRoute_ReturnsFalse()
    {
        // Arrange
        var classification = CreateValidClassification();
        classification.RecommendedRoute = (RouteEnum)999; // Invalid enum value

        // Act
        var result = ClassificationValidator.IsValid(classification, out var error);

        // Assert
        Assert.False(result);
        Assert.Contains("Unknown route", error);
    }

    [Fact]
    public void IsValid_InvalidVerification_ReturnsFalse()
    {
        // Arrange
        var classification = CreateValidClassification();
        classification.VerificationStatus = (VerificationEnum)999; // Invalid enum value

        // Act
        var result = ClassificationValidator.IsValid(classification, out var error);

        // Assert
        Assert.False(result);
        Assert.Contains("Unknown verification status", error);
    }

    [Fact]
    public void IsValid_InvalidUrgency_ReturnsFalse()
    {
        // Arrange
        var classification = CreateValidClassification();
        classification.Urgency = (UrgencyEnum)999; // Invalid enum value

        // Act
        var result = ClassificationValidator.IsValid(classification, out var error);

        // Assert
        Assert.False(result);
        Assert.Contains("Unknown urgency", error);
    }

    // Vector validation (5 tests)

    [Fact]
    public void IsValid_CategoryVectorNullOrWrongLength_ReturnsFalse()
    {
        // Arrange - null vector
        var classification = CreateValidClassification();
        classification.CategoryVector = null!;

        // Act
        var result = ClassificationValidator.IsValid(classification, out var error);

        // Assert
        Assert.False(result);
        Assert.Contains("Category vector is null", error);
    }

    [Fact]
    public void IsValid_CategoryVectorSumNot1_ReturnsFalse()
    {
        // Arrange - sum = 0.5, not 1.0
        var classification = CreateValidClassification();
        classification.CategoryVector = [0.1, 0.2, 0.1, 0.1]; // Sum = 0.5

        // Act
        var result = ClassificationValidator.IsValid(classification, out var error);

        // Assert
        Assert.False(result);
        Assert.Contains("must sum to approximately 1.0", error);
    }

    [Fact]
    public void IsValid_CategoryVectorNegativeValue_ReturnsFalse()
    {
        // Arrange
        var classification = CreateValidClassification();
        classification.CategoryVector = [0.1, 0.7, -0.1, 0.3]; // Negative value

        // Act
        var result = ClassificationValidator.IsValid(classification, out var error);

        // Assert
        Assert.False(result);
        Assert.Contains("must be between 0 and 1", error);
    }

    [Fact]
    public void IsValid_CategoryVectorHighestValueMismatchesLabel_ReturnsFalse()
    {
        // Arrange - category = Backend (index 1), but highest value is index 0
        var classification = CreateValidClassification();
        classification.Category = CategoryEnum.Backend;
        classification.CategoryVector = [0.8, 0.1, 0.05, 0.05]; // Highest at index 0, not 1

        // Act
        var result = ClassificationValidator.IsValid(classification, out var error);

        // Assert
        Assert.False(result);
        Assert.Contains("does not match selected label index", error);
    }

    [Fact]
    public void IsValid_CategoryVectorWithinTolerance_ReturnsTrue()
    {
        // Arrange - sum = 1.01, within 0.05 tolerance
        var classification = CreateValidClassification();
        classification.CategoryVector = [0.1, 0.67, 0.14, 0.1]; // Sum = 1.01

        // Act
        var result = ClassificationValidator.IsValid(classification, out var error);

        // Assert
        Assert.True(result);
        Assert.Empty(error);
    }

    // Value-label alignment (3 tests using Theory)

    [Theory]
    [InlineData(UrgencyEnum.Low, 0.0, true)]
    [InlineData(UrgencyEnum.Low, 0.2, true)]
    [InlineData(UrgencyEnum.Low, 0.5, false)]  // Out of range for Low (max 0.25)
    [InlineData(UrgencyEnum.Medium, 0.3, true)]
    [InlineData(UrgencyEnum.Medium, 0.5, true)]
    [InlineData(UrgencyEnum.Medium, 0.7, true)]
    [InlineData(UrgencyEnum.Medium, 0.1, false)]  // Out of range for Medium (min 0.26)
    [InlineData(UrgencyEnum.High, 0.76, true)]  // High starts at 0.76
    [InlineData(UrgencyEnum.High, 1.0, true)]
    [InlineData(UrgencyEnum.High, 0.4, false)]  // Out of range for High
    public void IsValid_UrgencyValueMatchesLabel(UrgencyEnum urgency, double value, bool expectedValid)
    {
        // Arrange
        var classification = CreateValidClassification();
        classification.Urgency = urgency;
        classification.UrgencyValue = value;

        // Act
        var result = ClassificationValidator.IsValid(classification, out var error);

        // Assert
        Assert.Equal(expectedValid, result);
    }

    [Theory]
    [InlineData(false, 0.0, true)]
    [InlineData(false, 0.2, true)]
    [InlineData(false, 0.5, true)]  // 0.5 is still in false range (<=0.50)
    [InlineData(false, 0.51, false)]  // Out of range for false
    [InlineData(true, 0.51, true)]
    [InlineData(true, 1.0, true)]
    [InlineData(true, 0.5, false)]  // Out of range for true (needs >0.50)
    [InlineData(true, 0.2, false)]  // Out of range for true
    public void IsValid_EscalationValueMatchesBool(bool escalate, double value, bool expectedValid)
    {
        // Arrange
        var classification = CreateValidClassification();
        classification.EscalateToHuman = escalate;
        classification.EscalateToHumanValue = value;

        // Act
        var result = ClassificationValidator.IsValid(classification, out var error);

        // Assert
        Assert.Equal(expectedValid, result);
    }

    [Theory]
    [InlineData(FalseReportRiskEnum.Low, 0.0, true)]
    [InlineData(FalseReportRiskEnum.Low, 0.2, true)]
    [InlineData(FalseReportRiskEnum.Low, 0.5, false)]  // Out of range for Low (max 0.25)
    [InlineData(FalseReportRiskEnum.Medium, 0.3, true)]
    [InlineData(FalseReportRiskEnum.Medium, 0.5, true)]
    [InlineData(FalseReportRiskEnum.Medium, 0.7, true)]
    [InlineData(FalseReportRiskEnum.Medium, 0.1, false)]  // Out of range for Medium (min 0.26)
    [InlineData(FalseReportRiskEnum.High, 0.76, true)]  // High starts at 0.76
    [InlineData(FalseReportRiskEnum.High, 1.0, true)]
    [InlineData(FalseReportRiskEnum.High, 0.4, false)]  // Out of range for High
    public void IsValid_RiskValueMatchesLabel(FalseReportRiskEnum risk, double value, bool expectedValid)
    {
        // Arrange
        var classification = CreateValidClassification();
        classification.FalseReportRisk = risk;
        classification.FalseReportRiskValue = value;

        // High risk requires escalation, human review route, and cannot be Supported
        if (risk == FalseReportRiskEnum.High)
        {
            classification.EscalateToHuman = true;
            classification.EscalateToHumanValue = 1.0;
            classification.RecommendedRoute = RouteEnum.HumanReview;
            classification.RecommendedRouteVector = [0.1, 0.1, 0.1, 0.7];
            classification.VerificationStatus = VerificationEnum.Inconclusive;  // Can't be Supported with High risk
            classification.VerificationStatusVector = [0.1, 0.1, 0.8];
        }

        // Act
        var result = ClassificationValidator.IsValid(classification, out var error);

        // Assert
        Assert.Equal(expectedValid, result);
    }

    // Cross-field consistency (8 tests - CRITICAL)

    [Fact]
    public void IsValid_Contradicted_MustEscalate_Fails()
    {
        // Arrange
        var classification = CreateValidClassification();
        classification.VerificationStatus = VerificationEnum.Contradicted;
        classification.VerificationStatusVector = [0.1, 0.8, 0.1];
        classification.FalseReportRisk = FalseReportRiskEnum.High;
        classification.FalseReportRiskValue = 1.0;
        classification.RecommendedRoute = RouteEnum.HumanReview;
        classification.RecommendedRouteVector = [0.1, 0.1, 0.1, 0.7];
        classification.EscalateToHuman = false;  // Invalid!
        classification.EscalateToHumanValue = 0.0;

        // Act
        var result = ClassificationValidator.IsValid(classification, out var error);

        // Assert
        Assert.False(result);
        Assert.Contains("Contradicted evidence must set escalate_to_human = true", error);
    }

    [Fact]
    public void IsValid_Contradicted_MustRouteHumanReview_Fails()
    {
        // Arrange
        var classification = CreateValidClassification();
        classification.VerificationStatus = VerificationEnum.Contradicted;
        classification.VerificationStatusVector = [0.1, 0.8, 0.1];
        classification.FalseReportRisk = FalseReportRiskEnum.High;
        classification.FalseReportRiskValue = 1.0;
        classification.EscalateToHuman = true;
        classification.EscalateToHumanValue = 1.0;
        classification.RecommendedRoute = RouteEnum.BackendTeam;  // Invalid!
        classification.RecommendedRouteVector = [0.1, 0.7, 0.1, 0.1];

        // Act
        var result = ClassificationValidator.IsValid(classification, out var error);

        // Assert
        Assert.False(result);
        Assert.Contains("Contradicted evidence must route to human_review", error);
    }

    [Fact]
    public void IsValid_Contradicted_MustHaveHighRisk_Fails()
    {
        // Arrange
        var classification = CreateValidClassification();
        classification.VerificationStatus = VerificationEnum.Contradicted;
        classification.VerificationStatusVector = [0.1, 0.8, 0.1];
        classification.EscalateToHuman = true;
        classification.EscalateToHumanValue = 1.0;
        classification.RecommendedRoute = RouteEnum.HumanReview;
        classification.RecommendedRouteVector = [0.1, 0.1, 0.1, 0.7];
        classification.FalseReportRisk = FalseReportRiskEnum.Low;  // Invalid!
        classification.FalseReportRiskValue = 0.0;

        // Act
        var result = ClassificationValidator.IsValid(classification, out var error);

        // Assert
        Assert.False(result);
        Assert.Contains("Contradicted evidence must use false_report_risk = high", error);
    }

    [Fact]
    public void IsValid_HighRisk_MustEscalate_Fails()
    {
        // Arrange
        var classification = CreateValidClassification();
        classification.FalseReportRisk = FalseReportRiskEnum.High;
        classification.FalseReportRiskValue = 1.0;
        classification.RecommendedRoute = RouteEnum.HumanReview;
        classification.RecommendedRouteVector = [0.1, 0.1, 0.1, 0.7];
        classification.EscalateToHuman = false;  // Invalid!
        classification.EscalateToHumanValue = 0.0;

        // Act
        var result = ClassificationValidator.IsValid(classification, out var error);

        // Assert
        Assert.False(result);
        Assert.Contains("High false report risk must set escalate_to_human = true", error);
    }

    [Fact]
    public void IsValid_UnknownCategory_MustEscalate_Fails()
    {
        // Arrange
        var classification = CreateValidClassification();
        classification.Category = CategoryEnum.Unknown;
        classification.CategoryVector = [0.1, 0.1, 0.1, 0.7];
        classification.RecommendedRoute = RouteEnum.HumanReview;
        classification.RecommendedRouteVector = [0.1, 0.1, 0.1, 0.7];
        classification.EscalateToHuman = false;  // Invalid!
        classification.EscalateToHumanValue = 0.0;

        // Act
        var result = ClassificationValidator.IsValid(classification, out var error);

        // Assert
        Assert.False(result);
        Assert.Contains("Unknown category must set escalate_to_human = true", error);
    }

    [Fact]
    public void IsValid_Supported_CannotBeHighRisk_Fails()
    {
        // Arrange
        var classification = CreateValidClassification();
        classification.VerificationStatus = VerificationEnum.Supported;
        classification.VerificationStatusVector = [0.8, 0.1, 0.1];
        classification.FalseReportRisk = FalseReportRiskEnum.High;  // Invalid combo!
        classification.FalseReportRiskValue = 1.0;
        classification.EscalateToHuman = true;
        classification.EscalateToHumanValue = 1.0;
        classification.RecommendedRoute = RouteEnum.HumanReview;
        classification.RecommendedRouteVector = [0.1, 0.1, 0.1, 0.7];

        // Act
        var result = ClassificationValidator.IsValid(classification, out var error);

        // Assert
        Assert.False(result);
        Assert.Contains("Supported evidence cannot have false_report_risk = high", error);
    }

    [Fact]
    public void IsValid_Contradicted_CannotBeLowRisk_Fails()
    {
        // Arrange - already tested in MustHaveHighRisk, but explicit test
        var classification = CreateValidClassification();
        classification.VerificationStatus = VerificationEnum.Contradicted;
        classification.VerificationStatusVector = [0.1, 0.8, 0.1];
        classification.FalseReportRisk = FalseReportRiskEnum.Low;  // Invalid!
        classification.FalseReportRiskValue = 0.0;
        classification.EscalateToHuman = true;
        classification.EscalateToHumanValue = 1.0;
        classification.RecommendedRoute = RouteEnum.HumanReview;
        classification.RecommendedRouteVector = [0.1, 0.1, 0.1, 0.7];

        // Act
        var result = ClassificationValidator.IsValid(classification, out var error);

        // Assert
        Assert.False(result);
        Assert.Contains("Contradicted evidence must use false_report_risk = high", error);
    }

    [Fact]
    public void IsValid_ValidCrossFieldCombinations_Passes()
    {
        // Test various valid combinations

        // Valid: Supported + Low Risk + No Escalation
        var supported = CreateValidClassification();
        supported.VerificationStatus = VerificationEnum.Supported;
        supported.VerificationStatusVector = [0.8, 0.1, 0.1];
        supported.FalseReportRisk = FalseReportRiskEnum.Low;
        supported.FalseReportRiskValue = 0.1;
        supported.EscalateToHuman = false;
        supported.EscalateToHumanValue = 0.1;
        Assert.True(ClassificationValidator.IsValid(supported, out _));

        // Valid: Contradicted + High Risk + Escalation
        var contradicted = CreateValidClassification();
        contradicted.VerificationStatus = VerificationEnum.Contradicted;
        contradicted.VerificationStatusVector = [0.1, 0.8, 0.1];
        contradicted.FalseReportRisk = FalseReportRiskEnum.High;
        contradicted.FalseReportRiskValue = 1.0;
        contradicted.EscalateToHuman = true;
        contradicted.EscalateToHumanValue = 1.0;
        contradicted.RecommendedRoute = RouteEnum.HumanReview;
        contradicted.RecommendedRouteVector = [0.1, 0.1, 0.1, 0.7];
        Assert.True(ClassificationValidator.IsValid(contradicted, out _));

        // Valid: Inconclusive + Medium Risk + No Escalation
        var inconclusive = CreateValidClassification();
        inconclusive.VerificationStatus = VerificationEnum.Inconclusive;
        inconclusive.VerificationStatusVector = [0.1, 0.1, 0.8];
        inconclusive.FalseReportRisk = FalseReportRiskEnum.Medium;
        inconclusive.FalseReportRiskValue = 0.5;
        inconclusive.EscalateToHuman = false;
        inconclusive.EscalateToHumanValue = 0.1;
        Assert.True(ClassificationValidator.IsValid(inconclusive, out _));
    }

    // Null handling (2 tests)

    [Fact]
    public void IsValid_NullClassification_ReturnsFalse()
    {
        // Act
        var result = ClassificationValidator.IsValid(null, out var error);

        // Assert
        Assert.False(result);
        Assert.Contains("Classification is null", error);
    }

    [Fact]
    public void IsValid_NullMissingInfoList_ReturnsFalse()
    {
        // Arrange
        var classification = CreateValidClassification();
        classification.MissingInfo = null!;

        // Act
        var result = ClassificationValidator.IsValid(classification, out var error);

        // Assert
        Assert.False(result);
        Assert.Contains("Missing info list is null", error);
    }

    // Valid examples (3 tests)

    [Fact]
    public void IsValid_SupportedBackendMedium_ReturnsTrue()
    {
        // Arrange
        var classification = new BugClassification
        {
            Category = CategoryEnum.Backend,
            CategoryVector = [0.1, 0.7, 0.1, 0.1],
            Urgency = UrgencyEnum.Medium,
            UrgencyValue = 0.5,
            RecommendedRoute = RouteEnum.BackendTeam,
            RecommendedRouteVector = [0.1, 0.7, 0.1, 0.1],
            EscalateToHuman = false,
            EscalateToHumanValue = 0.1,
            VerificationStatus = VerificationEnum.Supported,
            VerificationStatusVector = [0.8, 0.1, 0.1],
            FalseReportRisk = FalseReportRiskEnum.Low,
            FalseReportRiskValue = 0.1,
            VerificationReason = "Evidence supports the bug report.",
            MissingInfo = []
        };

        // Act
        var result = ClassificationValidator.IsValid(classification, out var error);

        // Assert
        Assert.True(result);
        Assert.Empty(error);
    }

    [Fact]
    public void IsValid_ContradictedFrontendHigh_ReturnsTrue()
    {
        // Arrange
        var classification = new BugClassification
        {
            Category = CategoryEnum.Frontend,
            CategoryVector = [0.7, 0.1, 0.1, 0.1],
            Urgency = UrgencyEnum.High,
            UrgencyValue = 1.0,
            RecommendedRoute = RouteEnum.HumanReview,
            RecommendedRouteVector = [0.1, 0.1, 0.1, 0.7],
            EscalateToHuman = true,
            EscalateToHumanValue = 1.0,
            VerificationStatus = VerificationEnum.Contradicted,
            VerificationStatusVector = [0.1, 0.8, 0.1],
            FalseReportRisk = FalseReportRiskEnum.High,
            FalseReportRiskValue = 1.0,
            VerificationReason = "Evidence contradicts the bug report.",
            MissingInfo = []
        };

        // Act
        var result = ClassificationValidator.IsValid(classification, out var error);

        // Assert
        Assert.True(result);
        Assert.Empty(error);
    }

    [Fact]
    public void IsValid_InconclusiveInfrastructureLow_ReturnsTrue()
    {
        // Arrange
        var classification = new BugClassification
        {
            Category = CategoryEnum.Infrastructure,
            CategoryVector = [0.1, 0.1, 0.7, 0.1],
            Urgency = UrgencyEnum.Low,
            UrgencyValue = 0.1,
            RecommendedRoute = RouteEnum.InfrastructureTeam,
            RecommendedRouteVector = [0.1, 0.1, 0.7, 0.1],
            EscalateToHuman = false,
            EscalateToHumanValue = 0.1,
            VerificationStatus = VerificationEnum.Inconclusive,
            VerificationStatusVector = [0.1, 0.1, 0.8],
            FalseReportRisk = FalseReportRiskEnum.Medium,
            FalseReportRiskValue = 0.5,
            VerificationReason = "Evidence is inconclusive.",
            MissingInfo = []
        };

        // Act
        var result = ClassificationValidator.IsValid(classification, out var error);

        // Assert
        Assert.True(result);
        Assert.Empty(error);
    }

    // Helper method to create a valid baseline classification
    private static BugClassification CreateValidClassification()
    {
        return new BugClassification
        {
            Category = CategoryEnum.Backend,
            CategoryVector = [0.1, 0.7, 0.1, 0.1],
            Urgency = UrgencyEnum.Medium,
            UrgencyValue = 0.5,
            RecommendedRoute = RouteEnum.BackendTeam,
            RecommendedRouteVector = [0.1, 0.7, 0.1, 0.1],
            EscalateToHuman = false,
            EscalateToHumanValue = 0.1,
            VerificationStatus = VerificationEnum.Supported,
            VerificationStatusVector = [0.8, 0.1, 0.1],
            FalseReportRisk = FalseReportRiskEnum.Low,
            FalseReportRiskValue = 0.1,
            VerificationReason = "Test reason",
            MissingInfo = []
        };
    }
}
