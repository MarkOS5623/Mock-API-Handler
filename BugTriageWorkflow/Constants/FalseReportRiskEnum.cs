using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using BugTriageWorkflow.Helpers;

namespace BugTriageWorkflow.Constants;

/// <summary>
/// Estimated likelihood that the report is inaccurate or unsupported.
/// </summary>
[JsonConverter(typeof(EnumMemberJsonConverter<FalseReportRiskEnum>))]
public enum FalseReportRiskEnum {
    [EnumMember(Value = "low")]
    Low,

    [EnumMember(Value = "medium")]
    Medium,

    [EnumMember(Value = "high")]
    High
}
