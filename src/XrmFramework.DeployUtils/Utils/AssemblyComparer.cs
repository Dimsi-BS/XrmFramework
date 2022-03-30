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
        public static readonly StepComparer _stepComparer = new();
        public static ISolutionComponent CorrespondingComponent<T>(T from, IEnumerable<T> target) where T : ISolutionComponent
        {
            return target.FirstOrDefault(x => ComponentEqual(from, x));
        }

        private static bool ComponentEqual(ISolutionComponent x, ISolutionComponent y)
        {
            if (x.GetType() != y.GetType()) return false;

            switch (x)
            {
                case Step:
                    return _stepComparer.Equals((Step)x, (Step)y);
                case StepImage:
                    return ((StepImage)x).IsPreImage == ((StepImage)y).IsPreImage;
                default:
                    return x.UniqueName == y.UniqueName;
            }
        }

        public static bool NeedsUpdate(ISolutionComponent x, ISolutionComponent y)
        {
            if (x.GetType() != y.GetType()) return false;
            switch (x)
            {
                case Plugin:
                    return false;
                case Step:
                    return _stepComparer.NeedsUpdate((Step)x, (Step)y);
                case StepImage:
                    return ((StepImage)x).JoinedAttributes == ((StepImage)y).JoinedAttributes;
                case CustomApi:
                    return NeedsUpdate((CustomApi)x, (CustomApi)y);
                case ICustomApiComponent:
                    return NeedsUpdate((ICustomApiComponent)x, (ICustomApiComponent)y);
                default:
                    throw new ArgumentException("SolutionComponent not recognised");
            }
        }

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
            return y.Steps.FirstOrDefault(s => _stepComparer.Equals(step, s));
        }

        public static ICustomApiComponent CorrespondingCustomApiComponent(ICustomApiComponent component, CustomApi y)
        {
            if (component.GetType().IsAssignableFrom(typeof(CustomApiRequestParameter)))
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
                || x.IsFunction ^ y.IsFunction
                || x.WorkflowSdkStepEnabled ^ y.WorkflowSdkStepEnabled
                || !x.AllowedCustomProcessingStepType.Equals(y.AllowedCustomProcessingStepType);
        }
    }
}
