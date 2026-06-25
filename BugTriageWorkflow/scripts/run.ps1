$ErrorActionPreference = "Stop"

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectDir = Split-Path -Parent $ScriptDir

dotnet run --project (Join-Path $ProjectDir "BugTriageWorkflow.csproj")
