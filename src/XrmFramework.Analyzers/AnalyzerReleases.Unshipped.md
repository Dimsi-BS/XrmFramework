; Unshipped analyzer release
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|--------------------
XRM0300 | Usage | Error | Use IDateTimeProvider instead of DateTime.Now / DateTime.UtcNow / DateTime.Today in plugins and services. [Documentation](https://github.com/Dimsi-BS/XrmFramework/blob/main/docs/Analyzers.md#xrm0300)
XRM1001 | XrmFramework.Generators | Error | Conflicting names for one table — emitted when several .table files declare the same table (same LogName) under different Name values. [Documentation](https://github.com/Dimsi-BS/XrmFramework/blob/main/docs/Analyzers.md#xrm1001)
XRM1002 | XrmFramework.Generators | Error | EnumGenerator failure — emitted when the smart-enum generator cannot produce the Items collection for a class decorated with [EnumGeneration]. [Documentation](https://github.com/Dimsi-BS/XrmFramework/blob/main/docs/Analyzers.md#xrm1002)
XRM1003 | XrmFramework.Generators | Error | Conflicting names for one option set — emitted when several .table files give one Name to option sets that are not the same option set. [Documentation](https://github.com/Dimsi-BS/XrmFramework/blob/main/docs/Analyzers.md#xrm1003)
XRM1004 | XrmFramework.Generators | Error | Option set member the enum cannot declare — emitted when an option set member's name yields no valid C# identifier or collides with another member's. [Documentation](https://github.com/Dimsi-BS/XrmFramework/blob/main/docs/Analyzers.md#xrm1004)
XRM2001 | XrmFramework.Generators | Warning | MappingGenerator failure — emitted when the mapping generator cannot produce the mapping for a binding model. [Documentation](https://github.com/Dimsi-BS/XrmFramework/blob/main/docs/Analyzers.md#xrm2001)
