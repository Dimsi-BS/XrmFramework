using Deploy;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;
using static XrmFramework.DeployUtils.Model.StepCollection;

namespace XrmFramework.DeployUtils.Utils
{
    public class AssemblyComparer
    {
        public static Plugin CorrespondingPlugin(Plugin plugin, IAssemblyContext assembly)
        {
            return assembly.Plugins.FirstOrDefault(p => p.FullName == plugin.FullName);
        }

        public static Plugin CorrespondingWorkflow(Plugin wf, IAssemblyContext assembly)
        {
            return assembly.Workflows.FirstOrDefault(p => p.FullName == wf.FullName);
        }

        public static CustomApi CorrespondingCustomApi(CustomApi customApi, IAssemblyContext assembly)
        {
            return assembly.CustomApis.FirstOrDefault(p => p.UniqueName == customApi.UniqueName);
        }

        public static Step CorrespondingStep(Step step, Plugin y)
        {
            var comparer = new StepComparer();
            return y.Steps.FirstOrDefault(s => comparer.Equals(step, s));
        }

        public static ICustomApiComponent CorrespondingCustomApiComponent(ICustomApiComponent component, CustomApi y)
        {
            if(component.GetType().IsAssignableFrom(typeof(CustomApiRequestParameter)))
            {
                return y.InArguments.FirstOrDefault(p => p.UniqueName == component.UniqueName);
            }
            else
            {
                return y.OutArguments.FirstOrDefault(p => p.UniqueName == component.UniqueName);

            }
        }

        public static bool NeedsUpdate(ICustomApiComponent x, ICustomApiComponent y)
        {
            return !x.Type.Equals(y.Type);
        }

        public static bool NeedsUpdate(CustomApi x, CustomApi y)
        {
            return !x.BindingType.Equals(y.BindingType)
                ||  x.IsFunction ^ y.IsFunction
                ||  x.WorkflowSdkStepEnabled ^ y.WorkflowSdkStepEnabled
                || !x.AllowedCustomProcessingStepType.Equals(y.AllowedCustomProcessingStepType);
        }
    }
}
