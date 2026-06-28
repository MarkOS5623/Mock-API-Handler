using Xunit;
using BugTriageWorkflow.Constants;

namespace BugTriageWorkflow.Tests.Unit;

public class TriageLabelsTests
{
    // Array length invariants (4 tests)

    [Fact]
    public void CategoryAll_Length4()
    {
        // Assert
        Assert.Equal(4, TriageLabels.Category.All.Length);
        Assert.Equal(4, TriageLabels.Category.AllEnums.Length);
    }

    [Fact]
    public void RouteAll_Length4()
    {
        // Assert
        Assert.Equal(4, TriageLabels.Route.All.Length);
        Assert.Equal(4, TriageLabels.Route.AllEnums.Length);
    }

    [Fact]
    public void VerificationAll_Length3()
    {
        // Assert
        Assert.Equal(3, TriageLabels.Verification.All.Length);
        Assert.Equal(3, TriageLabels.Verification.AllEnums.Length);
    }

    [Fact]
    public void LevelAll_Length3()
    {
        // Assert
        Assert.Equal(3, TriageLabels.Level.All.Length);
        Assert.Equal(3, TriageLabels.Level.AllUrgencyEnums.Length);
        Assert.Equal(3, TriageLabels.Level.AllRiskEnums.Length);
    }

    // Order consistency (2 tests)

    [Fact]
    public void CategoryAll_MatchesAllEnums_SameOrder()
    {
        // Arrange
        var expectedOrder = new[]
        {
            CategoryEnum.Frontend,
            CategoryEnum.Backend,
            CategoryEnum.Infrastructure,
            CategoryEnum.Unknown
        };

        // Assert - enums array matches expected order
        Assert.Equal(expectedOrder, TriageLabels.Category.AllEnums);

        // Assert - string array corresponds to enum array
        for (int i = 0; i < TriageLabels.Category.AllEnums.Length; i++)
        {
            var enumValue = TriageLabels.Category.AllEnums[i];
            var stringValue = TriageLabels.Category.All[i];

            // Verify the enum and string at the same index correspond
            Assert.Equal(i, Array.IndexOf(TriageLabels.Category.AllEnums, enumValue));
        }
    }

    [Fact]
    public void RouteAll_MatchesAllEnums_SameOrder()
    {
        // Arrange
        var expectedOrder = new[]
        {
            RouteEnum.FrontendTeam,
            RouteEnum.BackendTeam,
            RouteEnum.InfrastructureTeam,
            RouteEnum.HumanReview
        };

        // Assert - enums array matches expected order
        Assert.Equal(expectedOrder, TriageLabels.Route.AllEnums);

        // Assert - string array corresponds to enum array
        for (int i = 0; i < TriageLabels.Route.AllEnums.Length; i++)
        {
            var enumValue = TriageLabels.Route.AllEnums[i];
            var stringValue = TriageLabels.Route.All[i];

            // Verify the enum and string at the same index correspond
            Assert.Equal(i, Array.IndexOf(TriageLabels.Route.AllEnums, enumValue));
        }
    }

    // Lookup behavior (2 tests)

    [Fact]
    public void IndexOf_ValidLabel_ReturnsCorrectIndex()
    {
        // Test Category
        Assert.Equal(0, TriageLabels.IndexOf("frontend", TriageLabels.Category.All));
        Assert.Equal(1, TriageLabels.IndexOf("backend", TriageLabels.Category.All));
        Assert.Equal(2, TriageLabels.IndexOf("infrastructure", TriageLabels.Category.All));
        Assert.Equal(3, TriageLabels.IndexOf("unknown", TriageLabels.Category.All));

        // Test Route
        Assert.Equal(0, TriageLabels.IndexOf("frontend_team", TriageLabels.Route.All));
        Assert.Equal(1, TriageLabels.IndexOf("backend_team", TriageLabels.Route.All));
        Assert.Equal(2, TriageLabels.IndexOf("infrastructure_team", TriageLabels.Route.All));
        Assert.Equal(3, TriageLabels.IndexOf("human_review", TriageLabels.Route.All));

        // Test Verification
        Assert.Equal(0, TriageLabels.IndexOf("supported_by_evidence", TriageLabels.Verification.All));
        Assert.Equal(1, TriageLabels.IndexOf("contradicted_by_evidence", TriageLabels.Verification.All));
        Assert.Equal(2, TriageLabels.IndexOf("inconclusive", TriageLabels.Verification.All));

        // Test Level
        Assert.Equal(0, TriageLabels.IndexOf("low", TriageLabels.Level.All));
        Assert.Equal(1, TriageLabels.IndexOf("medium", TriageLabels.Level.All));
        Assert.Equal(2, TriageLabels.IndexOf("high", TriageLabels.Level.All));

        // Test case-insensitive
        Assert.Equal(0, TriageLabels.IndexOf("FRONTEND", TriageLabels.Category.All));
        Assert.Equal(1, TriageLabels.IndexOf("Backend", TriageLabels.Category.All));
        Assert.Equal(0, TriageLabels.IndexOf(" frontend ", TriageLabels.Category.All)); // With whitespace
    }

    [Fact]
    public void IndexOf_InvalidLabel_ReturnsNegative1()
    {
        // Test invalid labels
        Assert.Equal(-1, TriageLabels.IndexOf("invalid", TriageLabels.Category.All));
        Assert.Equal(-1, TriageLabels.IndexOf("nonexistent", TriageLabels.Route.All));
        Assert.Equal(-1, TriageLabels.IndexOf("unknown_status", TriageLabels.Verification.All));
        Assert.Equal(-1, TriageLabels.IndexOf("critical", TriageLabels.Level.All));
        Assert.Equal(-1, TriageLabels.IndexOf("", TriageLabels.Category.All));
    }
}
