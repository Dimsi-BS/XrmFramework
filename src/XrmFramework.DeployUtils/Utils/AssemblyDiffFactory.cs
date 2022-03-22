using Deploy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XrmFramework.DeployUtils.Context;
using static XrmFramework.DeployUtils.Model.StepCollection;

namespace XrmFramework.DeployUtils.Utils
{
    public class AssemblyDiffFactory
    {
        private static StepComparer _stepComparer = new StepComparer();

        public static void ComputeDiff(IAssemblyContext x, IAssemblyContext y)
        {
            foreach (var plugin in x.Plugins)
            {
                var correspondingPlugin = AssemblyComparer.CorrespondingPlugin(plugin, y);
                if (correspondingPlugin == null)
                {
                    FlagAllFromPlugin(plugin, RegistrationState.ToCreate);
                }
                else
                {
                    ComputePluginDiff(plugin, correspondingPlugin);
                }
            }
            foreach (var plugin in y.Plugins)
            {
                var correspondingPlugin = AssemblyComparer.CorrespondingPlugin(plugin, x);
                if (correspondingPlugin == null)
                {
                    FlagAllFromPlugin(plugin, RegistrationState.ToDelete);
                    x.Plugins.Add(plugin);
                }
            }

            foreach (var wf in x.Workflows)
            {
                var correspondingWf = AssemblyComparer.CorrespondingWorkflow(wf, y);
                if (correspondingWf == null)
                {
                    wf.RegistrationState = RegistrationState.ToCreate;
                }
                else
                {
                    wf.AssemblyId = correspondingWf.AssemblyId;
                    wf.Id = correspondingWf.Id;
                    if (wf.DisplayName != correspondingWf.FullName)
                    {
                        wf.RegistrationState = RegistrationState.ToUpdate;
                    }
                    else
                    {
                        wf.RegistrationState = RegistrationState.Ignore;
                    }
                }
            }
            foreach (var wf in y.Workflows)
            {
                var correspondingWf = AssemblyComparer.CorrespondingWorkflow(wf, x);
                if (correspondingWf == null)
                {
                    wf.RegistrationState = RegistrationState.ToDelete;
                    x.Workflows.Add(wf);
                }
            }

            foreach (var customApi in x.CustomApis)
            {
                var correspondingCustomApi = AssemblyComparer.CorrespondingCustomApi(customApi, y);
                if (correspondingCustomApi == null)
                {
                    FlagAllFromCustomApi(customApi, RegistrationState.ToCreate);
                }
                else
                {
                    ComputeCustomApiDiff(customApi, correspondingCustomApi);
                }
            }
            foreach (var customApi in y.CustomApis)
            {
                var correspondingCustomApi = AssemblyComparer.CorrespondingCustomApi(customApi, x);
                if (correspondingCustomApi == null)
                {
                    FlagAllFromCustomApi(customApi, RegistrationState.ToDelete);
                    x.CustomApis.Add(customApi);
                }
            }
        }

        private static void ComputeCustomApiDiff(CustomApi x, CustomApi y)
        {
            x.PluginTypeId = y.PluginTypeId;
            x.Id = y.Id;

            foreach (var request in x.InArguments)
            {
                var correspondingRequest = AssemblyComparer.CorrespondingCustomApiComponent(request, y);
                if (correspondingRequest == null)
                {
                    request.RegistrationState = RegistrationState.ToCreate;
                }
                else if (AssemblyComparer.NeedsUpdate(request, correspondingRequest))
                {
                    request.Id = correspondingRequest.Id;
                    request.CustomApiId = correspondingRequest.CustomApiId;
                    request.RegistrationState = RegistrationState.ToUpdate;
                }
                else
                {
                    request.RegistrationState = RegistrationState.Ignore;
                }
            }
            foreach(var request in y.InArguments)
            {
                var correspondingRequest = AssemblyComparer.CorrespondingCustomApiComponent(request, x);
                if(correspondingRequest == null)
                {
                    request.RegistrationState = RegistrationState.ToDelete;
                    x.InArguments.Add(request);
                }
            }

            foreach (var response in x.OutArguments)
            {
                var correspondingResponse = AssemblyComparer.CorrespondingCustomApiComponent(response, y);
                if (correspondingResponse == null)
                {
                    response.RegistrationState = RegistrationState.ToCreate;
                }
                else if (AssemblyComparer.NeedsUpdate(response, correspondingResponse))
                {
                    response.Id = correspondingResponse.Id;
                    response.CustomApiId = correspondingResponse.CustomApiId;
                    response.RegistrationState = RegistrationState.ToUpdate;
                }
                else
                {
                    response.RegistrationState = RegistrationState.Ignore;
                }
            }
            foreach (var response in y.OutArguments)
            {
                var correspondingRequest = AssemblyComparer.CorrespondingCustomApiComponent(response, x);
                if (correspondingRequest == null)
                {
                    response.RegistrationState = RegistrationState.ToDelete;
                    x.OutArguments.Add(response);
                }
            }

            if (AssemblyComparer.NeedsUpdate(x, y))
            {
                x.RegistrationState = RegistrationState.ToUpdate;
            }
            else
            {
                x.RegistrationState = RegistrationState.Ignore;
            }
        }

        private static void ComputePluginDiff(Model.Plugin x, Model.Plugin y)
        {
            x.AssemblyId = y.AssemblyId;
            x.Id = y.Id;
            x.RegistrationState = RegistrationState.Ignore;

            foreach (var step in x.Steps)
            {
                var correspondingStep = AssemblyComparer.CorrespondingStep(step, y);
                if (correspondingStep == null)
                {
                    step.PluginId = x.Id;
                    FlagAllFromStep(step, RegistrationState.ToCreate);
                }
                else
                {
                    step.PluginId = x.Id;
                    ComputeStepDiff(step, correspondingStep);
                }
            }
            foreach (var step in y.Steps)
            {
                var correspondingStep = AssemblyComparer.CorrespondingStep(step, x);
                if (correspondingStep == null)
                {
                    FlagAllFromStep(step, RegistrationState.ToDelete);
                    x.Steps.Add(step);
                }
            }
        }

        private static void ComputeStepDiff(Model.Step x, Model.Step y)
        {
            x.Id = y.Id;
            ComputeStepImageDiff(x.PreImage, y.PreImage);
            ComputeStepImageDiff(x.PostImage, y.PostImage);
            if (_stepComparer.NeedsUpdate(x, y))
            {
                x.RegistrationState = RegistrationState.ToUpdate;
            }
            else
            {
                x.RegistrationState = RegistrationState.Ignore;
            }
        }

        private static void ComputeStepImageDiff(Model.StepImage x, Model.StepImage y)
        {
            if (x.IsUsed && y.IsUsed)
            {
                x.StepId = y.StepId;
                x.Id = y.Id;
                if (x.JoinedAttributes != y.JoinedAttributes)
                {
                    FlagStepImage(x, RegistrationState.ToUpdate);
                }
                else
                {
                    FlagStepImage(x, RegistrationState.Ignore);
                }
            }
            else if (x.IsUsed && !y.IsUsed)
            {
                FlagStepImage(x, RegistrationState.ToCreate);
            }
            else if (y.IsUsed && !x.IsUsed)
            {
                x.Id = y.Id;
                x.StepId = y.StepId;
                FlagStepImage(x, RegistrationState.ToDelete);
            }
            else
            {
                FlagStepImage(x, RegistrationState.Ignore);
            }
        }

        private static void FlagAllFromCustomApi(CustomApi customApi, RegistrationState state)
        {
            foreach (var request in customApi.InArguments)
            {
                request.RegistrationState = state;
            }
            foreach (var response in customApi.OutArguments)
            {
                response.RegistrationState = state;
            }
            customApi.RegistrationState = state;
        }

        private static void FlagAllFromPlugin(Model.Plugin plugin, RegistrationState state)
        {
            foreach (var step in plugin.Steps)
            {
                FlagAllFromStep(step, state);
            }
            plugin.RegistrationState = state;
        }

        private static void FlagAllFromStep(Model.Step step, RegistrationState state)
        {
            FlagStepImage(step.PreImage, state);
            FlagStepImage(step.PostImage, state);
            step.RegistrationState = state;
        }

        private static void FlagStepImage(Model.StepImage image, RegistrationState state)
        {
            if (!image.IsUsed)
            {
                image.RegistrationState = RegistrationState.Ignore;
            }
            else
            {
                image.RegistrationState = state;
            }

        }
    }
}
