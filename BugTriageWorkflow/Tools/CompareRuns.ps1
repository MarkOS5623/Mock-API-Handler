#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Compare two benchmark runs and detect metric regressions.

.DESCRIPTION
    Compares summary metrics between a baseline and current run.
    Detects regressions when metrics drop beyond configured thresholds.
    Exit code 0 = no regressions, 1 = regressions detected.

.PARAMETER Baseline
    Path to the baseline JSON file.

.PARAMETER Current
    Path to the current run JSON file.

.PARAMETER ConfidenceThreshold
    Maximum allowed confidence drop (default: 0.05 = 5%).

.PARAMETER AccuracyThreshold
    Maximum allowed accuracy drop (default: 0.10 = 10%).

.PARAMETER QualityThreshold
    Maximum allowed quality drop (default: 0.05 = 5%).

.EXAMPLE
    .\CompareRuns.ps1 -Baseline baselines/detailed_all_baseline.json -Current logs/run_20260627_192503.json
#>

param(
    [Parameter(Mandatory = $true)]
    [string]$Baseline,

    [Parameter(Mandatory = $true)]
    [string]$Current,

    [double]$ConfidenceThreshold = 0.05,
    [double]$AccuracyThreshold = 0.10,
    [double]$QualityThreshold = 0.05
)

$ErrorActionPreference = "Stop"

# Verify files exist
if (-not (Test-Path $Baseline)) {
    Write-Error "Baseline file not found: $Baseline"
    exit 1
}

if (-not (Test-Path $Current)) {
    Write-Error "Current run file not found: $Current"
    exit 1
}

# Load JSON files
Write-Host "Comparing runs:"
Write-Host "  Baseline: $Baseline"
Write-Host "  Current:  $Current"
Write-Host ""

try {
    $baselineData = Get-Content $Baseline -Raw | ConvertFrom-Json
    $currentData = Get-Content $Current -Raw | ConvertFrom-Json
}
catch {
    Write-Error "Failed to parse JSON files: $_"
    exit 1
}

# Extract summary metrics
$baselineSummary = $baselineData.summary
$currentSummary = $currentData.summary

$baselineConfidence = $baselineSummary.AverageConfidenceScore
$currentConfidence = $currentSummary.AverageConfidenceScore
$confidenceDelta = $currentConfidence - $baselineConfidence

$baselineAccuracy = $baselineSummary.AverageAccuracyScore
$currentAccuracy = $currentSummary.AverageAccuracyScore
$accuracyDelta = $currentAccuracy - $baselineAccuracy

$baselineQuality = $baselineSummary.AverageQualityScore
$currentQuality = $currentSummary.AverageQualityScore
$qualityDelta = $currentQuality - $baselineQuality

# Calculate pass rates
$baselinePassRate = if ($baselineSummary.TotalPredictions -gt 0) {
    $baselineSummary.TotalCorrectPredictions / $baselineSummary.TotalPredictions
} else { 0.0 }

$currentPassRate = if ($currentSummary.TotalPredictions -gt 0) {
    $currentSummary.TotalCorrectPredictions / $currentSummary.TotalPredictions
} else { 0.0 }

$passRateDelta = $currentPassRate - $baselinePassRate

# Display metrics
Write-Host "Metrics:"

$confidenceStatus = if ($confidenceDelta -lt -$ConfidenceThreshold) { "REGRESSION" } else { "OK" }
Write-Host ("  Confidence: {0:F2} (baseline {1:F2}, delta {2:F2}) {3}" -f $currentConfidence, $baselineConfidence, $confidenceDelta, $confidenceStatus)

$accuracyStatus = if ($accuracyDelta -lt -$AccuracyThreshold) { "REGRESSION" } else { "OK" }
Write-Host ("  Accuracy:   {0:F2} (baseline {1:F2}, delta {2:F2}) {3}" -f $currentAccuracy, $baselineAccuracy, $accuracyDelta, $accuracyStatus)

$qualityStatus = if ($qualityDelta -lt -$QualityThreshold) { "REGRESSION" } else { "OK" }
Write-Host ("  Quality:    {0:F2} (baseline {1:F2}, delta {2:F2}) {3}" -f $currentQuality, $baselineQuality, $qualityDelta, $qualityStatus)

$passRateStatus = if ($passRateDelta -lt -$AccuracyThreshold) { "REGRESSION" } else { "OK" }
Write-Host ("  Pass Rate:  {0:P0} (baseline {1:P0}, delta {2:P0}) {3}" -f $currentPassRate, $baselinePassRate, $passRateDelta, $passRateStatus)

Write-Host ""

# Detect regressions
$regressions = @()

if ($confidenceDelta -lt -$ConfidenceThreshold) {
    $regressions += "Confidence regressed: $($confidenceDelta.ToString('F2'))"
}

if ($accuracyDelta -lt -$AccuracyThreshold) {
    $regressions += "Accuracy regressed: $($accuracyDelta.ToString('F2'))"
}

if ($qualityDelta -lt -$QualityThreshold) {
    $regressions += "Quality regressed: $($qualityDelta.ToString('F2'))"
}

if ($passRateDelta -lt -$AccuracyThreshold) {
    $regressions += "Pass rate regressed: $($passRateDelta.ToString('F2'))"
}

# Report results
if ($regressions.Count -gt 0) {
    Write-Host "Regressions detected:" -ForegroundColor Red
    foreach ($regression in $regressions) {
        Write-Host "  - $regression" -ForegroundColor Red
    }
    exit 1
}
else {
    Write-Host "No regressions detected." -ForegroundColor Green
    exit 0
}
