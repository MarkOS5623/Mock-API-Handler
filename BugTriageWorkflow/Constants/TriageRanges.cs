namespace BugTriageWorkflow.Constants;

/// <summary>
/// Centralized numeric ranges used by the bug triage workflow.
///
/// These ranges are referenced by:
/// - Prompt generation (telling the LLM which values correspond to low/medium/high)
/// - Classification validation (ensuring labels match numeric values)
///
/// </summary>
public static class TriageRanges {

    // ---------------------------------------------------------
    // Severity / Risk ranges
    // ---------------------------------------------------------
    //
    // Used for:
    // - urgency_value
    // - false_report_risk_value
    //
    // is valid because 0.15 falls within the Low range.
    //

    /// <summary>
    /// Minimum numeric value for a "low" classification.
    /// </summary>
    public const double LowMin = 0.00;

    /// <summary>
    /// Maximum numeric value for a "low" classification.
    /// </summary>
    public const double LowMax = 0.25;

    /// <summary>
    /// Minimum numeric value for a "medium" classification.
    /// </summary>
    public const double MediumMin = 0.26;

    /// <summary>
    /// Maximum numeric value for a "medium" classification.
    /// </summary>
    public const double MediumMax = 0.75;

    /// <summary>
    /// Minimum numeric value for a "high" classification.
    /// </summary>
    public const double HighMin = 0.76;

    /// <summary>
    /// Maximum numeric value for a "high" classification.
    /// </summary>
    public const double HighMax = 1.00;

    // ---------------------------------------------------------
    // Escalation ranges
    // ---------------------------------------------------------
    //
    // Used for:
    // - escalate_to_human
    // - escalate_to_human_value
    //
    // Example:
    // escalate_to_human = false
    // escalate_to_human_value = 0.20
    //
    // is valid because 0.20 falls within the
    // non-escalation range.
    //
    // Example:
    // escalate_to_human = true
    // escalate_to_human_value = 0.80
    //
    // is valid because 0.80 falls within the
    // escalation range.
    //

    /// <summary>
    /// Minimum value for a "false" escalation decision.
    /// </summary>
    public const double FalseEscalationMin = 0.00;

    /// <summary>
    /// Maximum value for a "false" escalation decision.
    /// </summary>
    public const double FalseEscalationMax = 0.50;

    /// <summary>
    /// Minimum value for a "true" escalation decision.
    /// </summary>
    public const double TrueEscalationMin = 0.51;

    /// <summary>
    /// Maximum value for a "true" escalation decision.
    /// </summary>
    public const double TrueEscalationMax = 1.00;

    /// <summary>
    /// Formats a numeric range for prompt generation.
    /// </summary>
    public static string Format(double min, double max) {
        return $"{min:0.00} to {max:0.00}";
    }
}