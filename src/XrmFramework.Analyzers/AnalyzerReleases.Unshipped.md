; Unshipped analyzer release
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|--------------------
XRM0300 | Usage | Error | Use IDateTimeProvider instead of DateTime.Now / DateTime.UtcNow in plugins and services. [Documentation](https://github.com/Dimsi-BS/XrmFramework/blob/main/docs/Analyzers.md#xrm0300)
XRM1002 | XrmFramework.Generators | Error | EnumGenerator failure — emitted when the smart-enum generator cannot produce the Items collection for a class decorated with [EnumGeneration]. [Documentation](https://github.com/Dimsi-BS/XrmFramework/blob/main/docs/Analyzers.md#xrm1002)
XRM2001 | XrmFramework.Generators | Warning | MappingGenerator failure — emitted when the mapping generator cannot produce the mapping for a binding model. [Documentation](https://github.com/Dimsi-BS/XrmFramework/blob/main/docs/Analyzers.md#xrm2001)
