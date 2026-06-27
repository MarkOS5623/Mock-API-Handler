using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using BugTriageWorkflow.Helpers;

namespace BugTriageWorkflow.Constants;

/// <summary>
/// Evidence verification status for the bug report.
/// </summary>
[JsonConverter(typeof(EnumMemberJsonConverter<VerificationEnum>))]
public enum VerificationEnum {
    [EnumMember(Value = "supported_by_evidence")]
    Supported,

    [EnumMember(Value = "contradicted_by_evidence")]
    Contradicted,

    [EnumMember(Value = "inconclusive")]
    Inconclusive
}
