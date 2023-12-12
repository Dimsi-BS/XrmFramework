using System.ComponentModel;

namespace XrmFramework
{

    public static partial class DebugSessionDefinition
    {

    }

    [OptionSetDefinition(DebugSessionDefinition.EntityName, DebugSessionDefinition.Columns.StateCode)]
    public enum DebugSessionState
    {
        [Description("Active")]
        Active = 0,
        [Description("Inactive")]
        Inactive = 1,
    }
}