using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using BugTriageWorkflow.Helpers;

namespace BugTriageWorkflow.Constants;

/// <summary>
/// High-level category for bug report classification.
/// </summary>
[JsonConverter(typeof(EnumMemberJsonConverter<CategoryEnum>))]
public enum CategoryEnum {
    [EnumMember(Value = "frontend")]
    Frontend,

    [EnumMember(Value = "backend")]
    Backend,

    [EnumMember(Value = "infrastructure")]
    Infrastructure,

    [EnumMember(Value = "unknown")]
    Unknown
}
