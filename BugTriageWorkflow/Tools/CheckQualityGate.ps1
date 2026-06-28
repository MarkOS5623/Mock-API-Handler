#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Enforce quality gate thresholds for a benchmark run.

.DESCRIPTION
    Validates a benchmark run against absolute quality thresholds.
    Optionally checks for regressions against a baseline.
    Exit code 0 = pass, 1 = fail.

.PARAMETER RunFile
    Path to the run JSON file to validate.

.PARAMETER Baseline
    Optional path to baseline JSON for regression comparison.

.EXAMPLE
    .\CheckQualityGate.ps1 -RunFile logs/run_20260627_192503.json

.EXAMPLE
    .\CheckQualityGate.ps1 -RunFile logs/run_20260627_192503.json -Baseline baselines/detailed_all_baseline.json
#>

param(
    [Parameter(Mandatory = $true)]
    [string]$RunFile,

    [string]$Baseline = ""
)

$ErrorActionPreference = "Stop"

# Absolute quality thresholds (from ROADMAP.md)
$MinConfidence = 0.70
$MinAccuracy = 0.80
$MinQuality = 0.75
$MinPassRate = 0.80

# Verify run file exists
if (-not (Test-Path $RunFile)) {
    Write-Error "Run file not found: $RunFile"
    exit 1
}

# Load run JSON
Write-Host "Quality Gate Check:"
Write-Host "  Run: $RunFile"
Write-Host ""

try {
    $runData = Get-Content $RunFile -Raw | ConvertFrom-Json
}
catch {
    Write-Error "Failed to parse JSON file: $_"
    exit 1
}

# Extract summary metrics
$summary = $runData.summary

$confidence = $summary.AverageConfidenceScore
$accuracy = $summary.AverageAccuracyScore
$quality = $summary.AverageQualityScore

$passRate = if ($summary.TotalPredictions -gt 0) {
    $summary.TotalCorrectPredictions / $summary.TotalPredictions
} else { 0.0 }

# Display absolute threshold checks
Write-Host "Absolute Thresholds:"

$confidencePass = $confidence -ge $MinConfidence
$confidenceStatus = if ($confidencePass) { "PASS" } else { "FAIL" }
Write-Host ("  Confidence: {0:F2} >= {1:F2} {2}" -f $confidence, $MinConfidence, $confidenceStatus)

$accuracyPass = $accuracy -ge $MinAccuracy
$accuracyStatus = if ($accuracyPass) { "PASS" } else { "FAIL" }
Write-Host ("  Accuracy:   {0:F2} >= {1:F2} {2}" -f $accuracy, $MinAccuracy, $accuracyStatus)

$qualityPass = $quality -ge $MinQuality
$qualityStatus = if ($qualityPass) { "PASS" } else { "FAIL" }
Write-Host ("  Quality:    {0:F2} >= {1:F2} {2}" -f $quality, $MinQuality, $qualityStatus)

$passRatePass = $passRate -ge $MinPassRate
$passRateStatus = if ($passRatePass) { "PASS" } else { "FAIL" }
Write-Host ("  Pass Rate:  {0:P0} >= {1:P0} {2}" -f $passRate, $MinPassRate, $passRateStatus)

Write-Host ""

# Check if all absolute thresholds passed
$absoluteThresholdsPassed = $confidencePass -and $accuracyPass -and $qualityPass -and $passRatePass

# Optionally check for regressions against baseline
$regressionCheckPassed = $true
if ($Baseline -ne "" -and (Test-Path $Baseline)) {
    Write-Host "Regression Check (vs baseline):"

    # Call CompareRuns.ps1
    $scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
    $compareScript = Join-Path $scriptDir "CompareRuns.ps1"

    if (Test-Path $compareScript) {
        & $compareScript -Baseline $Baseline -Current $RunFile
        $regressionCheckPassed = $LASTEXITCODE -eq 0
        Write-Host ""
    }
    else {
        Write-Warning "CompareRuns.ps1 not found, skipping regression check."
    }
}

# Final result
if ($absoluteThresholdsPassed -and $regressionCheckPassed) {
    Write-Host "Quality gate PASSED" -ForegroundColor Green
    exit 0
}
else {
    Write-Host "Quality gate FAILED" -ForegroundColor Red

    if (-not $absoluteThresholdsPassed) {
        Write-Host "  Reason: One or more absolute thresholds not met." -ForegroundColor Red
    }

    if (-not $regressionCheckPassed) {
        Write-Host "  Reason: Regressions detected compared to baseline." -ForegroundColor Red
    }

    exit 1
}
