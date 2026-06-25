# Setup and Running the Project

## Prerequisites

Before running the project, ensure the following are installed:

* .NET 9 SDK
* An OpenRouter account
* An OpenRouter API key

You can verify your .NET installation by running:

```bash
dotnet --version
```

---

## Clone the Repository

```bash
git clone <repository-url>
cd <repository-folder>
```

---

## Configure Environment Variables

Create a `.env` file in the project root.

Example:

```env
OPENROUTER_API_KEY=your_api_key_here
MODEL=openai/gpt-oss-120b:free
```

### Environment Variables

| Variable           | Description                       |
| ------------------ | --------------------------------- |
| OPENROUTER_API_KEY | Your OpenRouter API key           |
| MODEL              | The model used for classification |

Example model:

```env
MODEL=openai/gpt-oss-120b:free
```

---

## Restore Dependencies

Run:

```bash
dotnet restore
```

---

## Build the Project

Run:

```bash
dotnet build
```

A successful build should complete without errors.

---

## Run the Workflow

Start the application with:

```bash
dotnet run
```

When the application starts, you will be prompted to configure the run.

### Keyword Count

Select how many keywords should be extracted during preprocessing.

Example:

```text
Keyword Count [Default: 5]:
```

### Prompt Type

Choose one of the available prompt strategies:

```text
1 - Detailed
2 - Medium
3 - Vague
```

### Scenario

Select which test scenarios to execute:

```text
1 - All
2 - Supported
3 - Contradicted
4 - Inconclusive
...
```

### Manual Escalation Handling

Choose whether human escalation decisions should be handled automatically or manually:

```text
Enable manual escalation handling? (y/n)
```

---

## Output

During execution the workflow logs:

* Raw bug reports
* Preprocessing results
* Classifier outputs
* Routing decisions
* Evaluation metrics
* Run summary statistics

At the end of the run a log file is generated automatically.

Example:

```text
logs/run_20260615_143000.txt
```

The log contains:

* Run configuration
* Individual case evaluations
* Confidence scores
* Accuracy scores
* Quality scores
* Runtime statistics
* Final run summary

---

## Running Experiments

The project is designed to compare different prompt strategies and configurations.

Recommended experiments:

### Compare Prompt Types

Run the workflow using:

* Detailed
* Medium
* Vague

and compare:

* Confidence Score
* Accuracy Score
* Quality Score

### Compare Keyword Counts

Run the workflow using different keyword counts:

```text
5
10
15
```

and compare the resulting run summaries.
