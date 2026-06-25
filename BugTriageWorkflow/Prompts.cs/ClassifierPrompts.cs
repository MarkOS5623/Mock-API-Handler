using BugTriageWorkflow.Constants;
using BugTriageWorkflow.Models;

namespace BugTriageWorkflow.Prompts;

/// <summary>
/// Builds prompt templates for the bug classification agent.
/// 
/// The important improvement here is that labels, vector orders,
/// numeric ranges, and the JSON response shape are generated from
/// shared constants instead of being repeated manually.
/// </summary>
public static class ClassifierPrompts {
    private static string CategoryOptions => TriageLabels.AsPipeList(TriageLabels.Category.All);
    private static string RouteOptions => TriageLabels.AsPipeList(TriageLabels.Route.All);
    private static string VerificationOptions => TriageLabels.AsPipeList(TriageLabels.Verification.All);
    private static string LevelOptions => TriageLabels.AsPipeList(TriageLabels.Level.All);

    private static string CategoryVectorOrder => TriageLabels.AsVectorOrder(TriageLabels.Category.All);
    private static string RouteVectorOrder => TriageLabels.AsVectorOrder(TriageLabels.Route.All);
    private static string VerificationVectorOrder => TriageLabels.AsVectorOrder(TriageLabels.Verification.All);

    public static string Create(PromptType promptType, PreprocessedBugReport report) {
        return promptType switch {
            PromptType.Detailed => CreateDetailed(report),
            PromptType.Medium => CreateMedium(report),
            PromptType.Vague => CreateVague(report),
            _ => CreateDetailed(report)
        };
    }

    private static string CreateDetailed(PreprocessedBugReport report) {
        return $$"""
                You are a bug triage classifier.

                Analyze the bug report, compare it against the supplied evidence, and return a structured classification.

                {{CreateReportBlock(report)}}

                Category Rules:
                - {{TriageLabels.Category.Frontend}}: UI, screen, button, styling, layout, browser, React, mobile client, client-side rendering.
                - {{TriageLabels.Category.Backend}}: API, authentication, database, email, business logic, server-side validation, background jobs.
                - {{TriageLabels.Category.Infrastructure}}: deployment, outage, networking, CI/CD, gateway timeout, CDN, storage, storage provider, service availability.
                - {{TriageLabels.Category.Unknown}}: use only when the report and evidence do not clearly match another category.
                
                Important Category Rules:
                - Category describes the technical area of the reported bug.
                - Category is independent from whether the report is true.
                - Even if evidence contradicts the report, still classify the reported bug into the most likely technical category.
                - Use unknown only when the technical area cannot reasonably be inferred.

                Backend vs Infrastructure:
                - Backend includes application APIs, authentication, databases, business logic, background jobs, server-side validation, search services, profile services, export services, and email services.
                - Infrastructure includes deployment failures, networking failures, CDN issues, storage provider failures, cloud platform failures, service outages, CI/CD failures, and environment-level issues.
                - A failing application endpoint is usually backend.
                - A failing storage provider, gateway, network, CDN, or deployment platform is usually infrastructure.
                - A timeout alone does not automatically mean infrastructure.

                Frontend vs Backend:
                - If the user interface is broken but backend behavior appears correct, classify as frontend.
                - If a button, page, or UI action fails because an API, database, authentication service, or business process failed, classify as backend.

                Urgency Rules:
                - {{TriageLabels.Level.High}}: production outage, application unavailable, data loss, security issue, account access blocked for many users.
                - {{TriageLabels.Level.Medium}}: important feature broken, checkout/account recovery/search/export failing, repeated errors, degraded user workflow.
                - {{TriageLabels.Level.Low}}: cosmetic issue, intermittent issue, limited impact, workaround available, weak reproduction, or inconsistent evidence.

                Important Urgency Rules:
                - Do not assign high urgency solely because the reporter describes the issue as severe.
                - Do not assign high urgency when evidence is contradicted.
                - Do not assign high urgency when evidence is inconclusive unless strong impact is demonstrated.
                - Intermittent failures should usually be low urgency unless they affect many users.
                - Search, export, profile, download, account recovery, and similar feature failures are usually medium urgency unless widespread outage is proven.

                Verification Rules:
                - If evidence clearly supports the report:
                    - verification_status = "{{TriageLabels.Verification.Supported}}"
                    - false_report_risk = "{{TriageLabels.Level.Low}}"

                - If evidence clearly contradicts the report:
                    - verification_status = "{{TriageLabels.Verification.Contradicted}}"
                    - false_report_risk = "{{TriageLabels.Level.High}}"
                    - recommended_route = "{{TriageLabels.Route.HumanReview}}"
                    - escalate_to_human = true

                - If evidence is mixed, incomplete, weak, inconsistent, intermittent, reproduced only once, or not reproducible consistently:
                    - verification_status = "{{TriageLabels.Verification.Inconclusive}}"
                    - false_report_risk = "{{TriageLabels.Level.Medium}}"

                - Do not mark mixed evidence as supported only because one log line partly matches the report.
                - Do not accuse the reporter.
                - Only assess whether the evidence supports or contradicts the report.
                - Do not mark mixed evidence as supported only because one log line partly matches the report.
                - Only assess whether the evidence supports or contradicts the report.

                Consistency Rules:
                - {{TriageLabels.Verification.Supported}} evidence should usually have {{TriageLabels.Level.Low}} false_report_risk.
                - {{TriageLabels.Verification.Contradicted}} evidence should usually have {{TriageLabels.Level.High}} false_report_risk.
                - {{TriageLabels.Verification.Contradicted}} evidence should usually escalate_to_human = true.
                - {{TriageLabels.Verification.Contradicted}} evidence should usually route to {{TriageLabels.Route.HumanReview}}.
                - {{TriageLabels.Category.Unknown}} category should usually route to {{TriageLabels.Route.HumanReview}}.
                - {{TriageLabels.Level.High}} false_report_risk should usually route to {{TriageLabels.Route.HumanReview}}.
                - {{TriageLabels.Verification.Supported}} evidence should rarely have {{TriageLabels.Level.High}} false_report_risk.

                Routing Rules:
                - {{TriageLabels.Category.Frontend}} category normally routes to "{{TriageLabels.Route.FrontendTeam}}".
                - {{TriageLabels.Category.Backend}} category normally routes to "{{TriageLabels.Route.BackendTeam}}".
                - {{TriageLabels.Category.Infrastructure}} category normally routes to "{{TriageLabels.Route.InfrastructureTeam}}".
                - {{TriageLabels.Verification.Contradicted}} evidence routes to "{{TriageLabels.Route.HumanReview}}".
                - {{TriageLabels.Level.High}} false_report_risk routes to "{{TriageLabels.Route.HumanReview}}".
                - {{TriageLabels.Category.Unknown}} routes to "{{TriageLabels.Route.HumanReview}}".

                Escalation Rules:
                - escalate_to_human = true when verification_status is {{TriageLabels.Verification.Contradicted}}.
                - escalate_to_human = true when false_report_risk is {{TriageLabels.Level.High}}.
                - escalate_to_human = true when category is {{TriageLabels.Category.Unknown}}.
                - escalate_to_human = false for clearly supported or normal inconclusive cases.

                {{CreateNumericMappingsBlock()}}

                {{CreateVectorRulesBlock(includeExamples: true)}}

                {{CreateVectorOrderBlock()}}

                Return ONLY valid JSON with exactly these fields:
                {{CreateJsonShapeBlock()}}

                Do not return markdown.
                Do not return explanations outside the JSON.
                Do not split property names across lines.
                Do not include trailing commas.
            """;
    }

    private static string CreateMedium(PreprocessedBugReport report) {
        return $$"""
                You are a bug triage classifier. Classify the bug report using the text, evidence, and keywords.

                {{CreateReportBlock(report)}}

                Categories: {{ToBulletList(TriageLabels.Category.All)}}
                Routes: {{ToBulletList(TriageLabels.Route.All)}}
                Verification: {{ToBulletList(TriageLabels.Verification.All)}}

                Rules:
                - Clearly contradicted evidence should use verification_status = "{{TriageLabels.Verification.Contradicted}}", false_report_risk = "{{TriageLabels.Level.High}}", recommended_route = "{{TriageLabels.Route.HumanReview}}", and escalate_to_human = true.
                - Mixed, weak, intermittent, or inconsistently reproduced evidence should use verification_status = "{{TriageLabels.Verification.Inconclusive}}" and false_report_risk = "{{TriageLabels.Level.Medium}}".
                - Clearly supported evidence should use verification_status = "{{TriageLabels.Verification.Supported}}" and false_report_risk = "{{TriageLabels.Level.Low}}".
                - Do not mark mixed evidence as supported only because part of the evidence matches the report.
                - Do not increase urgency only because the report is suspicious.
                - Intermittent or inconclusive bugs should usually be low urgency unless broad user impact is proven.

                Category Rules:
                - Category describes the technical area of the reported bug.
                - Category is independent from whether the report is true.
                - Even if evidence contradicts the report, classify the reported bug into the most likely technical category.
                - Use {{TriageLabels.Category.Unknown}} only when the technical area cannot reasonably be inferred.
                - Application APIs, authentication, databases, business logic, search services, profile services, exports, and email services are usually {{TriageLabels.Category.Backend}}.
                - Deployment failures, networking failures, CDN issues, storage provider failures, cloud platform failures, and service outages are usually {{TriageLabels.Category.Infrastructure}}.
                
                {{CreateVectorRulesBlock(includeExamples: false)}}

                Return ONLY valid JSON with:
                {{CreateJsonShapeBlock()}}

                {{CreateVectorOrderBlock()}}

                {{CreateNumericMappingsBlock()}}
            """;
    }

    private static string CreateVague(PreprocessedBugReport report) {
        return $$"""
                Classify this bug report.

                Text: {{report.CleanText}}

                Evidence: {{report.Evidence}}

                Keywords: {{string.Join(", ", report.Keywords)}}

                Return valid JSON:
                {{CreateJsonShapeBlock()}}

                {{CreateVectorOrderBlock()}}

                Vectors should be confidence distributions, not always one-hot values.
            """;
    }

    /// <summary>
    /// Shared report section for detailed and medium prompts.
    /// This avoids repeating the same report formatting in multiple prompts.
    /// </summary>
    private static string CreateReportBlock(PreprocessedBugReport report) {
        return $$"""
                Bug Report Id: {{report.Id}}
                Reporter: {{report.Reporter}}
                Clean Text: {{report.CleanText}}
                Evidence: {{report.Evidence}}
                Keywords: {{string.Join(", ", report.Keywords)}}
            """;
    }

    /// <summary>
    /// Shared JSON response shape.
    /// The label options are generated from TriageLabels so the prompt stays
    /// aligned with the validator and evaluator.
    /// </summary>
    private static string CreateJsonShapeBlock() {
        return $$"""
            {
                "category": "{{CategoryOptions}}",
                "category_vector": [0.0, 0.0, 0.0, 0.0],
                "urgency": "{{LevelOptions}}",
                "urgency_value": 0.0,
                "missing_info": [],
                "recommended_route": "{{RouteOptions}}",
                "recommended_route_vector": [0.0, 0.0, 0.0, 0.0],
                "escalate_to_human": false,
                "escalate_to_human_value": 0.0,
                "verification_status": "{{VerificationOptions}}",
                "verification_status_vector": [0.0, 0.0, 0.0],
                "false_report_risk": "{{LevelOptions}}",
                "false_report_risk_value": 0.0,
                "verification_reason": "short explanation"
            }
        """;
    }

    /// <summary>
    /// Shared vector order section.
    /// The order here must match the validator's vector checks.
    /// </summary>
    private static string CreateVectorOrderBlock() {
        return $$"""
                Vector order:
                category_vector = [{{CategoryVectorOrder}}]
                recommended_route_vector = [{{RouteVectorOrder}}]
                verification_status_vector = [{{VerificationVectorOrder}}]
            """;
    }

    /// <summary>
    /// Shared numeric mapping section.
    /// These ranges come from TriageRanges, so they stay aligned with validation.
    /// </summary>
    private static string CreateNumericMappingsBlock() {
        return $$"""
            Numeric mappings:
            {{TriageLabels.Level.Low}} = {{TriageRanges.Format(TriageRanges.LowMin, TriageRanges.LowMax)}}
            {{TriageLabels.Level.Medium}} = {{TriageRanges.Format(TriageRanges.MediumMin, TriageRanges.MediumMax)}}
            {{TriageLabels.Level.High}} = {{TriageRanges.Format(TriageRanges.HighMin, TriageRanges.HighMax)}}
            false = {{TriageRanges.Format(TriageRanges.FalseEscalationMin, TriageRanges.FalseEscalationMax)}}
            true = {{TriageRanges.Format(TriageRanges.TrueEscalationMin, TriageRanges.TrueEscalationMax)}}
        """;
    }

    /// <summary>
    /// Shared vector guidance.
    /// Detailed prompts include examples; medium prompts keep it shorter.
    /// </summary>
    private static string CreateVectorRulesBlock(bool includeExamples) {
        var examples = includeExamples ? """
                Examples:
                - Strong confidence: [0.90, 0.05, 0.05, 0.00]
                - Moderate confidence: [0.60, 0.30, 0.10, 0.00]
                - High uncertainty: [0.35, 0.30, 0.25, 0.10]
                - If evidence mostly supports the report but some details remain inconclusive: [0.70, 0.00, 0.30]
                - Use one-hot vectors such as [1.00, 0.00, 0.00, 0.00] only when there is almost no uncertainty.
                - Most classifications should contain some uncertainty and should not be one-hot vectors.
            """
            : "";

        return $$"""
                Vector Rules:
                - Vectors are confidence distributions, not one-hot encodings.
                - Each vector must show how likely each possible answer is.
                - Each vector should sum to approximately 1.0.
                - The selected label must be the option with the highest vector value.
                - The selected label must always correspond to the highest value in the vector.
                - Use softer vectors when there is uncertainty.
                - Do not always return one-hot vectors such as [1.00, 0.00, 0.00, 0.00].{{examples}}
            """;
        }

    private static string ToBulletList(IEnumerable<string> options) { return string.Join(Environment.NewLine, options.Select(option => $"- {option}")); }
}