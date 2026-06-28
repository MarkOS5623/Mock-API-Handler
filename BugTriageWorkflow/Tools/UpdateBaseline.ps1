#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Promote a run to baseline status after validation.

.DESCRIPTION
    Validates that a run passes quality gates, then copies it to the Baselines directory
    with standardized naming. Stages the new baseline for git commit.

.PARAMETER RunFile
    Path to the run JSON file to promote to baseline.

.PARAMETER PromptType
    Prompt type used in the run (Detailed, Medium, or Vague).

.PARAMETER Scenario
    Scenario used in the run (All, Supports, Contradicts, or Inconclusive).

.EXAMPLE
    .\UpdateBaseline.ps1 -RunFile logs/run_20260627_192503.json -PromptType Detailed -Scenario All
#>

param(
    [Parameter(Mandatory = $true)]
    [string]$RunFile,

    [Parameter(Mandatory = $true)]
    [ValidateSet("Detailed", "Medium", "Vague")]
    [string]$PromptType,

    [Parameter(Mandatory = $true)]
    [ValidateSet("All", "Supports", "Contradicts", "Inconclusive")]
    [string]$Scenario
)

$ErrorActionPreference = "Stop"

# Verify run file exists
if (-not (Test-Path $RunFile)) {
    Write-Error "Run file not found: $RunFile"
    exit 1
}

Write-Host "Validating run before baseline promotion..."
Write-Host "  Run: $RunFile"
Write-Host "  Prompt: $PromptType"
Write-Host "  Scenario: $Scenario"
Write-Host ""

# Validate that run passes quality gate
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$qualityGateScript = Join-Path $scriptDir "CheckQualityGate.ps1"

if (-not (Test-Path $qualityGateScript)) {
    Write-Error "CheckQualityGate.ps1 not found in Tools directory."
    exit 1
}

& $qualityGateScript -RunFile $RunFile

if ($LASTEXITCODE -ne 0) {
    Write-Error "Run failed quality gate validation. Cannot promote to baseline."
    exit 1
}

Write-Host ""

# Generate baseline filename (lowercase, underscore-separated)
$promptTypeLower = $PromptType.ToLower()
$scenarioLower = $Scenario.ToLower()
$baselineFilename = "${promptTypeLower}_${scenarioLower}_baseline.json"

# Determine baseline directory (relative to script location)
$baselinesDir = Join-Path (Split-Path -Parent $scriptDir) "Baselines"

if (-not (Test-Path $baselinesDir)) {
    Write-Host "Creating Baselines directory: $baselinesDir"
    New-Item -ItemType Directory -Path $baselinesDir | Out-Null
}

$baselinePath = Join-Path $baselinesDir $baselineFilename

# Copy run to baseline
Write-Host "Promoting run to baseline..."
Copy-Item -Path $RunFile -Destination $baselinePath -Force

Write-Host "Baseline updated: $baselinePath" -ForegroundColor Green
Write-Host ""

# Stage for git commit
try {
    $gitRoot = Split-Path -Parent (Split-Path -Parent $scriptDir)
    Push-Location $gitRoot

    git add $baselinePath 2>&1 | Out-Null

    if ($LASTEXITCODE -eq 0) {
        Write-Host "Staged for commit. Don't forget to commit with a clear explanation:" -ForegroundColor Yellow
        Write-Host "  git commit -m ""Update baseline: $PromptType / $Scenario (reason here)""" -ForegroundColor Yellow
    }
    else {
        Write-Warning "Git staging failed. You may need to manually stage: $baselinePath"
    }
}
catch {
    Write-Warning "Failed to stage baseline for git: $_"
}
finally {
    Pop-Location
}

exit 0
