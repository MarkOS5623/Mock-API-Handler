namespace BugTriageWorkflow.Models;

/// <summary>
/// Indicates how a routing decision was made.
/// </summary>
public enum RouteSource {
    Automatic,
    Human
}

/// <summary>
/// Represents the final routing decision produced by the router.
/// May contain either an automatic routing decision or a
/// human-reviewed routing decision.
/// </summary>
public class RouteResult {
    /// <summary>
    /// Team or destination selected for the bug report.
    /// </summary>
    public string Route { get; set; } = "";

    /// <summary>
    /// Indicates whether the route was chosen automatically
    /// or by a human reviewer.
    /// </summary>
    public RouteSource RoutedBy { get; set; }

    /// <summary>
    /// Explanation of why the routing decision was made.
    /// </summary>
    public string Reason { get; set; } = "";

    /// <summary>
    /// Urgency selected by a human reviewer during escalation.
    /// Empty when routing was performed automatically.
    /// </summary>
    public string HumanSelectedUrgency { get; set; } = "";

    /// <summary>
    /// Human assessment of whether the report is false.
    /// Null when no human review was performed.
    /// </summary>
    public bool? HumanMarkedFalseReport { get; set; }
}