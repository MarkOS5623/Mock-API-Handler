using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using BugTriageWorkflow.Helpers;

namespace BugTriageWorkflow.Constants;

/// <summary>
/// Urgency level indicating estimated impact and severity.
/// </summary>
[JsonConverter(typeof(EnumMemberJsonConverter<UrgencyEnum>))]
public enum UrgencyEnum {
    [EnumMember(Value = "low")]
    Low,

    [EnumMember(Value = "medium")]
    Medium,

    [EnumMember(Value = "high")]
    High
}
