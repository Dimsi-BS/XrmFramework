using Deploy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Service;
using XrmFramework.DeployUtils.Utils;

namespace XrmFramework.DeployUtils.Context
{
    public class AssemblyFactory
    {
        private readonly IRegistrationContext _registrationContext;

        public AssemblyFactory(IRegistrationContext context)
        {
            _registrationContext = context;
        }

        public ILocalAssemblyContext LocalAssemblyContext(Type TPlugin)
        {
            var Assembly = TPlugin.Assembly;
            var pluginType = Assembly.GetType("XrmFramework.Plugin");
            var customApiType = Assembly.GetType("XrmFramework.CustomApi");
            var workflowType = Assembly.GetType("XrmFramework.Workflow.CustomWorkflowActivity");

            var PluginTypes = Assembly.GetTypes()
                                     .Where(t => pluginType.IsAssignableFrom(t)
                                              && !customApiType.IsAssignableFrom(t)
                                              && t.IsPublic
                                              && !t.IsAbstract)
                                     .ToList();

            var WorkFlowTypes = Assembly.GetTypes()
                                       .Where(t => workflowType.IsAssignableFrom(t)
                                                && !t.IsAbstract
                                                && t.IsPublic)
                                       .ToList();

            var CustomApiTypes = Assembly.GetTypes()
                                        .Where(t => customApiType.IsAssignableFrom(t)
                                                 && t.IsPublic
                                                 && !t.IsAbstract)
                                        .ToList();

            var pluginList = AssemblyBridge.CreateInstanceOfTypeList<Plugin>(PluginTypes, PluginRegistrationType.Plugin, _registrationContext);

            var workflowList = AssemblyBridge.CreateInstanceOfTypeList<Plugin>(WorkFlowTypes, PluginRegistrationType.Workflow, _registrationContext);

            pluginList.AddRange(workflowList);

            var Plugins = pluginList;

            var CustomApis = AssemblyBridge.CreateInstanceOfTypeList<CustomApi>(CustomApiTypes, PluginRegistrationType.CustomApi, _registrationContext);


            var CustomApiRequestParameters = new List<CustomApiRequestParameter>();
            var CustomApiResponseProperties = new List<CustomApiResponseProperty>();

            CustomApis.ForEach(c =>
            {
                CustomApiRequestParameters.AddRange(c.InArguments);
                CustomApiResponseProperties.AddRange(c.OutArguments);
            });
            var Steps = new List<SdkMessageProcessingStep>();

            Plugins.ForEach(p =>
            {
                foreach (var s in p.Steps)
                {
                    Steps.Add(AssemblyBridge.ToRegisterStep(new Guid(), s, _registrationContext));
                }

            });
            var localAssembly = new LocalAssemblyContext
            {
                Assembly = Assembly,
                Plugins = Plugins,
                CustomApis = CustomApis,
                CustomApiRequestParameters = CustomApiRequestParameters,
                CustomApiResponseProperties = CustomApiResponseProperties,
                Steps = Steps
            };
            return localAssembly;
        }

        public IRegisteredAssemblyContext RegisteredAssemblyContext(IRegistrationService service, string assemblyName)
        {
            var assembly = service.GetAssemblyByName(assemblyName);
            RegisteredAssemblyContext registeredAssembly;

            if (assembly != null)
            {
                registeredAssembly = new RegisteredAssemblyContext(assembly)
                {
                    CustomApis = service.GetRegisteredCustomApis(assembly.Id),
                    CustomApiRequestParameters = service.GetRegisteredCustomApiRequestParameters(assembly.Id),
                    CustomApiResponseProperties = service.GetRegisteredCustomApiResponseProperties(assembly.Id),
                    Steps = service.GetRegisteredSteps(assembly.Id),
                    PluginTypes = service.GetRegisteredPluginTypes(assembly.Id),
                    ImageSteps = service.GetRegisteredImages(assembly.Id),

                };
            }
            else
            {
                registeredAssembly = new RegisteredAssemblyContext(assembly)
                {
                    CustomApis = new List<CustomApi>(),
                    CustomApiRequestParameters = new List<CustomApiRequestParameter>(),
                    CustomApiResponseProperties = new List<CustomApiResponseProperty>(),
                    Steps = new List<SdkMessageProcessingStep>(),
                    PluginTypes = new List<PluginType>(),
                    ImageSteps = new List<SdkMessageProcessingStepImage>()
                };
            }
            return registeredAssembly;
        }
    }
}
