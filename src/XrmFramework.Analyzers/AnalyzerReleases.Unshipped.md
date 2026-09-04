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
XRM1005 | XrmFramework.Generators | Error | Model references an unknown table — emitted when a .model targets a table no .table file declares. [Documentation](https://github.com/Dimsi-BS/XrmFramework/blob/main/docs/Analyzers.md#xrm1005)
XRM1006 | XrmFramework.Generators | Error | Model property cannot be mapped to a column — emitted when a .model property names a column the table does not declare, or one that is not selected. [Documentation](https://github.com/Dimsi-BS/XrmFramework/blob/main/docs/Analyzers.md#xrm1006)
XRM1007 | XrmFramework.Generators | Error | Lookup property without a relationship — emitted when a .model maps a lookup column the table declares no many-to-one relationship for. [Documentation](https://github.com/Dimsi-BS/XrmFramework/blob/main/docs/Analyzers.md#xrm1007)
XRM1008 | XrmFramework.Generators | Error | Malformed .model file — emitted when a .model cannot be read, carrying the parser message. [Documentation](https://github.com/Dimsi-BS/XrmFramework/blob/main/docs/Analyzers.md#xrm1008)
XRM1009 | XrmFramework.Generators | Warning | Model property type does not match its column — emitted when the C# type a .model gives a property cannot hold the value of the column it maps to. [Documentation](https://github.com/Dimsi-BS/XrmFramework/blob/main/docs/Analyzers.md#xrm1009)
XRM1010 | XrmFramework.Generators | Error | Ambiguous lookup target — emitted when a .model maps a polymorphic lookup without naming the table it points at, or names one the column does not reach. [Documentation](https://github.com/Dimsi-BS/XrmFramework/blob/main/docs/Analyzers.md#xrm1010)
XRM2001 | XrmFramework.Generators | Warning | MappingGenerator failure — emitted when the mapping generator cannot produce the mapping for a binding model. [Documentation](https://github.com/Dimsi-BS/XrmFramework/blob/main/docs/Analyzers.md#xrm2001)
