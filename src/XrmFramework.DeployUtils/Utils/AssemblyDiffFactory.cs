using System.Collections.Generic;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;
using static XrmFramework.DeployUtils.Model.StepCollection;

namespace XrmFramework.DeployUtils.Utils
{
    public class AssemblyDiffFactory
    {
        private static StepComparer _stepComparer = new StepComparer();

        public static void ComputeAssemblyDiff(IAssemblyContext x, IAssemblyContext y)
        {
            if (y?.Assembly == null)
            {
                x.Assembly.RegistrationState = RegistrationState.ToCreate;
            }
            else
            {
                x.Assembly.RegistrationState = RegistrationState.ToUpdate;
                x.Assembly.Id = y.Assembly.Id;

                foreach(var plugin in x.Plugins)
                {
                    plugin.ParentId = y.Assembly.Id;
                }
                foreach(var workflow in x.Workflows)
                {
                    workflow.ParentId = y.Assembly.Id;
                }

                ComputeComponentDiffRoot(x.Plugins, y.Plugins);
                ComputeComponentDiffRoot(x.Workflows, y.Workflows);
                ComputeComponentDiffRoot(x.CustomApis, y.CustomApis);
            }
        }

        private static void ComputeComponentDiffRoot<T>(ICollection<T> from, ICollection<T> target) where T : ISolutionComponent
        {
            foreach (var component in from)
            {
                var correspondingComponent = AssemblyComparer.CorrespondingComponent(component, target);
                if (correspondingComponent == null)
                {
                    FlagAllFromComponent(component, RegistrationState.ToCreate);
                }
                else
                {
                    ComputeComponentDiff(component, correspondingComponent);
                }
            }
            foreach (var component in target)
            {
                var correspondingComponent = AssemblyComparer.CorrespondingComponent(component, from);
                if (correspondingComponent == null)
                {
                    FlagAllFromComponent(component, RegistrationState.ToDelete);
                    from.Add(component);
                }
            }
        }

        private static void ComputeComponentDiff(ISolutionComponent from, ISolutionComponent target)
        {
            from.ParentId = target.ParentId;
            from.Id = target.Id;
            if(AssemblyComparer.NeedsUpdate(from, target))
            {
                from.RegistrationState = RegistrationState.ToUpdate;
            }
            else
            {
                from.RegistrationState = RegistrationState.Ignore;
            }
            foreach(var fromChild in from.Children)
            {
                var targetChild = AssemblyComparer.CorrespondingComponent(fromChild, target.Children);
                if(targetChild == null)
                {
                    FlagAllFromComponent(fromChild, RegistrationState.ToCreate);
                    fromChild.RegistrationState = RegistrationState.ToCreate;
                }
                else
                {
                    ComputeComponentDiff(fromChild, targetChild);
                }
            }
            foreach(var targetChild in target.Children)
            {
                if(AssemblyComparer.CorrespondingComponent(targetChild, from.Children) == null)
                {
                    FlagAllFromComponent(targetChild, RegistrationState.ToDelete);
                    from.AddChild(targetChild);
                }
            }
        }

        private static void FlagAllFromComponent(ISolutionComponent target, RegistrationState state)
        {
            target.RegistrationState = state;
            foreach(var child in target.Children)
            {
                FlagAllFromComponent(child, state);
            }
        }
    }
}