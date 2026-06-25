The workflow consists of the following stages:

1. Preprocessing

Raw bug reports are cleaned and normalized using a workflow executor

The preprocessing stage:
 - Extracts keywords
 - Normalizes report text
 - Preserves supporting evidence
 - Produces a structured input for the classifier

2. Classification

An agent executor analyzes the bug report and produces a structured JSON response containing:
 - Category
 - Category confidence vector
 - Urgency
 - Urgency confidence value
 - Missing information
 - Recommended route
 - Route confidence vector
 - Human escalation recommendation
 - Verification status
 - Verification confidence vector
 - False report risk
 - Verification reasoning

The program supports multiple configurations:

Keyword count: recommended at least 5

Prompt options:
 - Detailed
 - Medium
 - Vague

Bonus part: 
 - Human responses can be toggle on or off while starting a run. 
 - Turning it on will require the user to interact with any human escalation

3. Routing

After successful classification, the routing workflow executor determines where the bug should be routed.

Possible routes include:
 - Frontend Team
 - Backend Team
 - Infrastructure Team
 - Human Review

4. Evaluation

Each classification is compared against an expected result dataset.

The evaluation system measures:

Confidence Score
 - Measures how much confidence the model assigned to the correct answer.
 - This rewards calibrated predictions and penalizes confident mistakes.

Accuracy Score
 - Measures exact-match correctness across:
   - Category
   - Urgency
   - Route
   - Escalation
   - Verification Status
   - False Report Risk
   - Quality Score

Quality is calculated as: (Confidence Score + Accuracy Score) / 2

Findings while working with the model:
 - Testing across multiple benchmark runs revealed several consistent trends:
   - Detailed prompts outperform Medium and Vague prompts.
   - Routing, verification status, false report risk, and escalation decisions achieve high accuracy.
   - Urgency classification remains the most challenging prediction task.
   - Category boundary decisions (Frontend vs Backend vs Infrastructure) account for most remaining classification errors.
   - Confidence scoring provides valuable insight into model calibration, while accuracy scoring measures exact correctness.
   - Quality scoring offers a balanced view of overall classifier performance.