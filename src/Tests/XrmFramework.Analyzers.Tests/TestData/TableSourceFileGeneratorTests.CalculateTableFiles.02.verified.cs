//HintName: OptionSetDefinition.table.cs
using System.ComponentModel;
using XrmFramework;

namespace FrameworkTests
{

    [OptionSetDefinition("synapselinkprofileentitytype")]
    public enum SynapseLinkProfileEntityType
    {
        [Description("Requested")]
        Requested = 0,
    }

    [OptionSetDefinition("synapselinkscheduletype")]
    public enum SynapseLinkScheduleType
    {
        [Description("Snapshot")]
        Snapshot = 0,
        [Description("Incrementalupdate")]
        Incrementalupdate = 1,
    }
}
