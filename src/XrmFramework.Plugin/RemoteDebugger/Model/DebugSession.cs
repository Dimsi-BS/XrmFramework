﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using XrmFramework.BindingModel;
using XrmFramework.RemoteDebugger.Model.CrmComponentInfos;

namespace XrmFramework.RemoteDebugger
{
    [CrmEntity(DebugSessionDefinition.EntityName)]
    public partial class DebugSession : IBindingModel
    {
        [CrmMapping(DebugSessionDefinition.Columns.DebugeeId)]
        public Guid Debugee { get; set; }

        [CrmMapping(DebugSessionDefinition.Columns.CreatedOn)]
        public DateTime SessionStart { get; set; }

        [CrmMapping(DebugSessionDefinition.Columns.SessionEnd)]
        public DateTime SessionEnd { get; set; }

        [CrmMapping(DebugSessionDefinition.Columns.RelayUrl)]
        public string RelayUrl { get; set; }

        [CrmMapping(DebugSessionDefinition.Columns.HybridConnectionName)]
        public string? HybridConnectionName { get; set; }

        [CrmMapping(DebugSessionDefinition.Columns.SasKeyName)]
        public string SasKeyName { get; set; }

        [CrmMapping(DebugSessionDefinition.Columns.SasConnectionKey)]
        public string SasConnectionKey { get; set; }

        [CrmMapping(DebugSessionDefinition.Columns.StateCode)]
        public DebugSessionState StateCode { get; set; }

        [CrmMapping(DebugSessionDefinition.Columns.DebugInfo)]
        public string AssembliesDebugInfo
        {
            get => JsonConvert.SerializeObject(AssemblyContexts);
            set => AssemblyContexts = JsonConvert.DeserializeObject<List<AssemblyContextInfo>>(value)!;
        }

        public List<AssemblyContextInfo> AssemblyContexts { get; set; } = new();

        public Guid Id { get; set; }
        public AssemblyContextInfo GetCorrespondingAssemblyInfo(string customApiUniqueName)
        {
            return AssemblyContexts
                .Find(a => a.CustomApis.Exists(c => c.UniqueName == customApiUniqueName));
        }

        public void CopyTo(DebugSession to)
        {
            to.Id = Id;
            to.AssembliesDebugInfo = AssembliesDebugInfo;
            to.StateCode = StateCode;
            to.Debugee = Debugee;
            to.HybridConnectionName = HybridConnectionName;
            to.RelayUrl = RelayUrl;
            to.SessionEnd = SessionEnd;
            to.SessionStart = SessionStart;
            to.SasKeyName = SasKeyName;
            to.SasConnectionKey = SasConnectionKey;
        }
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
