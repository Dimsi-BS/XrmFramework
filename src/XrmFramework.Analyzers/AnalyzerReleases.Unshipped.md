; Unshipped analyzer release
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|--------------------
XRM1002 | XrmFramework.Generators | Error | EnumGenerator failure — emitted when the smart-enum generator cannot produce the Items collection for a class decorated with [EnumGeneration].
