// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Text;

namespace XrmFramework.PluginInventory
{
    // Internal models + hand-rolled JSON serialization (no dependency).
    // The schema emitted here is exactly the one read by
    // XrmFramework.DeployUtils.Factories.PluginInventoryReader.

    internal sealed class PluginInfo
    {
        public PluginInfo(string fullName, List<StepInfo> steps) { FullName = fullName; Steps = steps; }
        public string FullName { get; }
        public List<StepInfo> Steps { get; }
    }

    internal sealed class WorkflowInfo
    {
        public WorkflowInfo(string fullName, string displayName) { FullName = fullName; DisplayName = displayName; }
        public string FullName { get; }
        public string DisplayName { get; }
    }

    internal sealed class StepInfo
    {
        public string Message = "";
        public string Stage = "";
        public string Mode = "";
        public string EntityName = "";
        public string MethodName = "";
        public List<string> MethodNames = new List<string>();
        public List<string> FilteringAttributes = new List<string>();
        public bool PreImageAll;
        public List<string> PreImageAttributes = new List<string>();
        public bool PostImageAll;
        public List<string> PostImageAttributes = new List<string>();
        public int Order = 1;
        public string ImpersonationUsername = "";
        public string UnsecureConfig;
    }

    internal sealed class CustomApiInfo
    {
        public string FullName = "";
        public string Name = "";
        public string DisplayName;
        public string Description;
        public string BindingType;
        public string BoundEntityLogicalName;
        public bool IsFunction;
        public bool IsPrivate;
        public string AllowedCustomProcessing;
        public string ExecutePrivilegeName;
        public bool WorkflowSdkStepEnabled;
        public List<ArgInfo> Arguments = new List<ArgInfo>();
    }

    internal sealed class ArgInfo
    {
        public bool IsIn;
        public string Name = "";
        public string TypeFullName = "";
        public bool IsEnum;
        public string DisplayName;
        public string Description;
        public string LogicalEntityName;
        public bool IsOptional;
    }

    internal static class ManifestJson
    {
        public static string Build(List<PluginInfo> plugins, List<WorkflowInfo> workflows, List<CustomApiInfo> customApis)
        {
            var sb = new StringBuilder();
            sb.Append("{\"plugins\":[");
            for (var pi = 0; pi < plugins.Count; pi++)
            {
                var p = plugins[pi];
                if (pi > 0) sb.Append(',');
                sb.Append("{\"fullName\":").Append(Str(p.FullName)).Append(",\"steps\":[");
                for (var si = 0; si < p.Steps.Count; si++)
                {
                    var s = p.Steps[si];
                    if (si > 0) sb.Append(',');
                    sb.Append("{\"message\":").Append(Str(s.Message))
                      .Append(",\"stage\":").Append(Str(s.Stage))
                      .Append(",\"mode\":").Append(Str(s.Mode))
                      .Append(",\"entityName\":").Append(Str(s.EntityName))
                      .Append(",\"methodName\":").Append(Str(s.MethodName))
                      .Append(",\"methodNames\":").Append(Arr(s.MethodNames))
                      .Append(",\"filteringAttributes\":").Append(Arr(s.FilteringAttributes))
                      .Append(",\"order\":").Append(s.Order)
                      .Append(",\"impersonationUsername\":").Append(Str(s.ImpersonationUsername))
                      .Append(",\"unsecureConfig\":").Append(s.UnsecureConfig == null ? "null" : Str(s.UnsecureConfig))
                      .Append(",\"preImage\":{\"allAttributes\":").Append(s.PreImageAll ? "true" : "false")
                      .Append(",\"attributes\":").Append(Arr(s.PreImageAttributes)).Append('}')
                      .Append(",\"postImage\":{\"allAttributes\":").Append(s.PostImageAll ? "true" : "false")
                      .Append(",\"attributes\":").Append(Arr(s.PostImageAttributes)).Append('}')
                      .Append('}');
                }
                sb.Append("]}");
            }
            sb.Append("],\"workflows\":[");
            for (var wi = 0; wi < workflows.Count; wi++)
            {
                if (wi > 0) sb.Append(',');
                sb.Append("{\"fullName\":").Append(Str(workflows[wi].FullName))
                  .Append(",\"displayName\":").Append(Str(workflows[wi].DisplayName)).Append('}');
            }
            sb.Append("],\"customApis\":[");
            for (var ci = 0; ci < customApis.Count; ci++)
            {
                var c = customApis[ci];
                if (ci > 0) sb.Append(',');
                sb.Append("{\"fullName\":").Append(Str(c.FullName))
                  .Append(",\"name\":").Append(Str(c.Name))
                  .Append(",\"displayName\":").Append(StrOrNull(c.DisplayName))
                  .Append(",\"description\":").Append(StrOrNull(c.Description))
                  .Append(",\"bindingType\":").Append(StrOrNull(c.BindingType))
                  .Append(",\"boundEntityLogicalName\":").Append(StrOrNull(c.BoundEntityLogicalName))
                  .Append(",\"isFunction\":").Append(c.IsFunction ? "true" : "false")
                  .Append(",\"isPrivate\":").Append(c.IsPrivate ? "true" : "false")
                  .Append(",\"allowedCustomProcessing\":").Append(StrOrNull(c.AllowedCustomProcessing))
                  .Append(",\"executePrivilegeName\":").Append(StrOrNull(c.ExecutePrivilegeName))
                  .Append(",\"workflowSdkStepEnabled\":").Append(c.WorkflowSdkStepEnabled ? "true" : "false")
                  .Append(",\"arguments\":[");
                for (var ai = 0; ai < c.Arguments.Count; ai++)
                {
                    var a = c.Arguments[ai];
                    if (ai > 0) sb.Append(',');
                    sb.Append("{\"isInArgument\":").Append(a.IsIn ? "true" : "false")
                      .Append(",\"name\":").Append(Str(a.Name))
                      .Append(",\"typeFullName\":").Append(Str(a.TypeFullName))
                      .Append(",\"isEnum\":").Append(a.IsEnum ? "true" : "false")
                      .Append(",\"displayName\":").Append(StrOrNull(a.DisplayName))
                      .Append(",\"description\":").Append(StrOrNull(a.Description))
                      .Append(",\"logicalEntityName\":").Append(StrOrNull(a.LogicalEntityName))
                      .Append(",\"isOptional\":").Append(a.IsOptional ? "true" : "false")
                      .Append('}');
                }
                sb.Append("]}");
            }
            sb.Append("]}");
            return sb.ToString();
        }

        private static string StrOrNull(string value) => value == null ? "null" : Str(value);

        private static string Arr(IReadOnlyList<string> items)
        {
            var sb = new StringBuilder("[");
            for (var i = 0; i < items.Count; i++)
            {
                if (i > 0) sb.Append(',');
                sb.Append(Str(items[i]));
            }
            sb.Append(']');
            return sb.ToString();
        }

        private static string Str(string value)
        {
            if (value == null) return "null";
            var sb = new StringBuilder("\"");
            foreach (var c in value)
            {
                switch (c)
                {
                    case '"': sb.Append("\\\""); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '\b': sb.Append("\\b"); break;
                    case '\f': sb.Append("\\f"); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\t': sb.Append("\\t"); break;
                    default:
                        if (c < 0x20) sb.Append("\\u").Append(((int)c).ToString("x4"));
                        else sb.Append(c);
                        break;
                }
            }
            sb.Append('"');
            return sb.ToString();
        }
    }
}
