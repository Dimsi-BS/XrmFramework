#Requires -Version 7.0
<#
.SYNOPSIS
    Aggregates the .trx files of a test run into a Markdown summary for
    $GITHUB_STEP_SUMMARY.

.DESCRIPTION
    Azure DevOps rendered the .trx server-side (the "Tests" tab); GitHub Actions has no
    first-party equivalent. This script fills that gap without granting any third-party
    action write access to the repository.

    `dotnet test` writes one .trx per test project AND per target framework, so a single
    run of XrmFramework.slnx produces ~15 files and counts a multi-targeted test once per
    TFM. Both numbers are reported: the executions (what the runner did) and the distinct
    tests (what was actually covered), so the inflated total is never mistaken for a test
    count.

    Namespaces are not declared anywhere on purpose: PowerShell's dot notation on an
    [xml] document ignores the TeamTest default namespace, which avoids carrying an
    XmlNamespaceManager around for every lookup.

.PARAMETER ResultsDirectory
    Folder holding the .trx files, searched recursively.

.PARAMETER OutputPath
    File the Markdown is appended to. Defaults to $GITHUB_STEP_SUMMARY; pass a path to
    render the summary locally.

.PARAMETER MaxFailuresDetailed
    Upper bound on detailed failure blocks, so a mass failure cannot blow past the
    1 MiB job-summary limit. The table keeps counting every failure regardless.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string] $ResultsDirectory,

    [string] $OutputPath = $env:GITHUB_STEP_SUMMARY,

    [int] $MaxFailuresDetailed = 20
)

$ErrorActionPreference = 'Stop'

# Set-StrictMode is deliberately NOT enabled: probing an optional element on an [xml]
# document ($run.TestDefinitions on a run that produced none) is a property access that
# strict mode turns into a terminating error instead of $null.

if (-not $OutputPath) {
    throw 'No output path: pass -OutputPath or run with GITHUB_STEP_SUMMARY set.'
}

$trxFiles = @(Get-ChildItem -Path $ResultsDirectory -Filter *.trx -Recurse -ErrorAction SilentlyContinue)
if ($trxFiles.Count -eq 0) {
    "## Tests`n`nNo .trx file found in ``$ResultsDirectory``." |
        Out-File -FilePath $OutputPath -Append -Encoding utf8
    return
}

# Paths inside a .trx are the runner's Windows paths, so neither Split-Path nor
# [IO.Path] can be trusted when this script runs on Linux/macOS. Splitting on both
# separators works everywhere.
function Get-PathParts([string] $path) {
    return @($path -split '[\\/]' | Where-Object { $_ })
}

$rows = [System.Collections.Generic.List[object]]::new()
$failures = [System.Collections.Generic.List[object]]::new()
$distinctTests = [System.Collections.Generic.HashSet[string]]::new()

foreach ($file in $trxFiles) {
    $doc = [xml](Get-Content -LiteralPath $file.FullName -Raw)
    $run = $doc.TestRun

    # Resolve the project and TFM from the first test's assembly path:
    #   ...\src\Tests\XrmFramework.Generators.Tests\bin\Release\net8.0\XrmFramework.Generators.Tests.dll
    $definitions = @()
    if ($run.TestDefinitions) { $definitions = @($run.TestDefinitions.UnitTest) }

    $project = $file.BaseName
    $tfm = ''
    if ($definitions.Count -gt 0) {
        $parts = Get-PathParts $definitions[0].TestMethod.codeBase
        if ($parts.Count -ge 2) {
            $project = $parts[-1] -replace '\.dll$', ''
            $tfm = $parts[-2]
        }
    }

    # testId -> className, to qualify a failure with the class it belongs to.
    $classNames = @{}
    foreach ($definition in $definitions) {
        $classNames[$definition.id] = $definition.TestMethod.className
    }

    # A run aborted before it wrote its summary has no Counters; fall back to zeroes
    # rather than failing the whole summary over one truncated file.
    $counters = if ($run.ResultSummary) { $run.ResultSummary.Counters } else { $null }
    $total = [int] $counters.total
    $passed = [int] $counters.passed
    $failed = [int] $counters.failed
    # NotExecuted is what VSTest writes for an [Ignore]d or filtered-out test; it is not
    # a failure, and `executed` already excludes it.
    $skipped = [int] $counters.notExecuted
    $errored = [int] $counters.error + [int] $counters.aborted + [int] $counters.timeout

    $duration = $null
    if ($run.Times) {
        $duration = [datetime] $run.Times.finish - [datetime] $run.Times.start
    }

    $results = @()
    if ($run.Results) { $results = @($run.Results.UnitTestResult) }

    foreach ($result in $results) {
        $className = if ($result.testId -and $classNames.ContainsKey($result.testId)) { $classNames[$result.testId] } else { '' }
        [void] $distinctTests.Add("$className.$($result.testName)")

        if ($result.outcome -in @('Failed', 'Error', 'Timeout', 'Aborted')) {
            $message = ''
            $stackTrace = ''
            if ($result.Output -and $result.Output.ErrorInfo) {
                $message = [string] $result.Output.ErrorInfo.Message
                $stackTrace = [string] $result.Output.ErrorInfo.StackTrace
            }
            $failures.Add([pscustomobject]@{
                Project    = $project
                Tfm        = $tfm
                ClassName  = $className
                TestName   = $result.testName
                Outcome    = $result.outcome
                Message    = $message.Trim()
                StackTrace = $stackTrace.Trim()
            })
        }
    }

    $rows.Add([pscustomobject]@{
        Project  = $project
        Tfm      = $tfm
        Total    = $total
        Passed   = $passed
        Failed   = $failed + $errored
        Skipped  = $skipped
        Duration = $duration
    })
}

$totalExecutions = ($rows | Measure-Object -Property Total -Sum).Sum
$totalPassed = ($rows | Measure-Object -Property Passed -Sum).Sum
$totalFailed = ($rows | Measure-Object -Property Failed -Sum).Sum
$totalSkipped = ($rows | Measure-Object -Property Skipped -Sum).Sum
$targetFrameworks = @($rows | Select-Object -ExpandProperty Tfm -Unique | Where-Object { $_ })

$lines = [System.Collections.Generic.List[string]]::new()
$verdict = if ($totalFailed -gt 0) { '❌' } else { '✅' }
$lines.Add("## $verdict Tests")
$lines.Add('')
$lines.Add("**$totalPassed passed · $totalFailed failed · $totalSkipped skipped** out of $totalExecutions executions.")
$lines.Add('')
$frameworkWord = if ($targetFrameworks.Count -eq 1) { 'target framework' } else { 'target frameworks' }
$assemblyWord = if ($rows.Count -eq 1) { 'test assembly' } else { 'test assemblies' }
$lines.Add("$($distinctTests.Count) distinct tests, run across $($targetFrameworks.Count) $frameworkWord ($($targetFrameworks -join ', ')) in $($rows.Count) $assemblyWord.")
$lines.Add('')
$lines.Add('| Project | TFM | Tests | Passed | Failed | Skipped | Duration |')
$lines.Add('|:---|:---|---:|---:|---:|---:|---:|')

# Failures first: the point of this table is to find what broke without scrolling.
foreach ($row in $rows | Sort-Object -Property @{ Expression = 'Failed'; Descending = $true }, Project, Tfm) {
    # Trimmed: a passing row would otherwise start with the empty status plus a space.
    $name = "$(if ($row.Failed -gt 0) { '❌ ' })$($row.Project)".Trim()
    $elapsed = if ($row.Duration) { '{0:mm\:ss}' -f $row.Duration } else { '—' }
    $lines.Add("| $name | $($row.Tfm) | $($row.Total) | $($row.Passed) | $($row.Failed) | $($row.Skipped) | $elapsed |")
}

if ($failures.Count -gt 0) {
    $lines.Add('')
    $lines.Add("### Failures ($($failures.Count))")

    # A test failing on four TFMs is one bug, not four: group so the summary stays
    # readable, and list the TFMs it failed on.
    $grouped = $failures | Group-Object -Property { "$($_.ClassName).$($_.TestName)" }
    $shown = 0

    foreach ($group in $grouped) {
        if ($shown -ge $MaxFailuresDetailed) {
            $lines.Add('')
            $lines.Add("_… and $($grouped.Count - $shown) more failing test(s). Full detail in the ``TestResults`` artifact._")
            break
        }

        $first = $group.Group[0]
        $tfms = @($group.Group | Select-Object -ExpandProperty Tfm -Unique) -join ', '
        $lines.Add('')
        $lines.Add("<details><summary><code>$($group.Name)</code> — $($first.Project) ($tfms)</summary>")
        $lines.Add('')
        if ($first.Message) {
            $lines.Add('```')
            $lines.Add($first.Message)
            $lines.Add('```')
        }
        if ($first.StackTrace) {
            # Blank line between the two fences: adjacent fences render as one block.
            $lines.Add('')
            $lines.Add('```')
            $lines.Add($first.StackTrace)
            $lines.Add('```')
        }
        $lines.Add('</details>')
        $shown++
    }
}

$lines.Add('')
($lines -join "`n") | Out-File -FilePath $OutputPath -Append -Encoding utf8
