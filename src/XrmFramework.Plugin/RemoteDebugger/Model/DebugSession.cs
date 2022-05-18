using System;
using System.Text;
using XrmFramework.BindingModel;
using XrmFramework.Definitions;

namespace XrmFramework.RemoteDebugger
{
    [CrmEntity(DebugSessionDefinition.EntityName)]
    public partial class DebugSession : IBindingModel
    {
        [CrmMapping(DebugSessionDefinition.Columns.Debugee)]
        public string Debugee { get; set; }

        [CrmMapping(DebugSessionDefinition.Columns.CreatedOn)]
        public DateTime SessionStart { get; set; }

        [CrmMapping(DebugSessionDefinition.Columns.SessionEnd)]
        public DateTime SessionEnd { get; set; }

        [CrmMapping(DebugSessionDefinition.Columns.RelayUrl)]
        public string RelayUrl { get; set; }

        [CrmMapping(DebugSessionDefinition.Columns.HybridConnectionName)]
        public string HybridConnectionName { get; set; }

        [CrmMapping(DebugSessionDefinition.Columns.SasKeyName)]
        public string SasKeyName { get; set; }

        [CrmMapping(DebugSessionDefinition.Columns.SasConnectionKey)]
        public string SasConnectionKey { get; set; }

        [CrmMapping(DebugSessionDefinition.Columns.StateCode)]
        public DebugSessionState StateCode { get; set; }

        [CrmMapping(DebugSessionDefinition.Columns.DebugInfo)]
        public string AssembliesDebugInfo { get; set; }

        public Guid Id { get; set; }


        #region Overrides of Object

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"\tDebugeeId = {Debugee}");
            sb.AppendLine($"\tSessionStart = {SessionStart}");
            sb.AppendLine($"\tSessionEnd = {SessionEnd}");
            sb.AppendLine($"\tState = {Enum.ToObject(typeof(DebugSessionState), StateCode)}");
            sb.AppendLine();
            sb.AppendLine($"\tRelayUrl = {RelayUrl}");
            sb.AppendLine($"\tHybridConnectionName = {HybridConnectionName}");
            sb.AppendLine($"\tSasKeyName = {SasKeyName}");
            sb.AppendLine($"\tSasConnectionKey = {SasConnectionKey}");
            sb.AppendLine();

            return sb.ToString();
        }

        #endregion
    }
}
