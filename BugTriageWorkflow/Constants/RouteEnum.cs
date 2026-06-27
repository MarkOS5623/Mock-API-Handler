using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using BugTriageWorkflow.Helpers;

namespace BugTriageWorkflow.Constants;

/// <summary>
/// Routing destination for the bug report.
/// </summary>
[JsonConverter(typeof(EnumMemberJsonConverter<RouteEnum>))]
public enum RouteEnum {
    [EnumMember(Value = "frontend_team")]
    FrontendTeam,

    [EnumMember(Value = "backend_team")]
    BackendTeam,

    [EnumMember(Value = "infrastructure_team")]
    InfrastructureTeam,

    [EnumMember(Value = "human_review")]
    HumanReview
}
