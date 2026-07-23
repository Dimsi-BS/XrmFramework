using System;

namespace XrmFramework.DefinitionManager.Extensions;

internal static class DateTimeBehaviorExtensions
{
    public static DateTimeBehavior ToFrameworkDateTimeBehavior(this Microsoft.Xrm.Sdk.Metadata.DateTimeBehavior behavior) => (behavior?.Value ?? nameof(DateTimeBehavior.UserLocal)) switch
    {
        nameof(DateTimeBehavior.UserLocal) => XrmFramework.DateTimeBehavior.UserLocal,
        nameof(DateTimeBehavior.DateOnly) => XrmFramework.DateTimeBehavior.DateOnly,
        nameof(DateTimeBehavior.TimeZoneIndependent) => XrmFramework.DateTimeBehavior.TimeZoneIndependent,
        _ => throw new NotImplementedException(),
    };
    
}
