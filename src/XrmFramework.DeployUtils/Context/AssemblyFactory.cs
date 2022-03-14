using Deploy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Service;

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

            var pluginList = CreateInstanceOfTypeList<Plugin>(PluginTypes, PluginRegistrationType.Plugin);

            var workflowList = CreateInstanceOfTypeList<Plugin>(WorkFlowTypes, PluginRegistrationType.Workflow);

            pluginList.AddRange(workflowList);

            var Plugins = pluginList;

            var CustomApis = CreateInstanceOfTypeList<CustomApi>(CustomApiTypes, PluginRegistrationType.CustomApi);


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
                    Steps.Add(RegistrationHelper.GetStepToRegister(new Guid(), s));
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

        public dynamic CreateInstanceOfType(Type type, PluginRegistrationType kind)
        {
            dynamic instance;
            switch (kind)
            {
                case PluginRegistrationType.Plugin:
                    if (type.GetConstructor(new[] { typeof(string), typeof(string) }) != null)
                    {
                        instance = Activator.CreateInstance(type, new object[] { null, null });
                    }
                    else
                    {
                        instance = Activator.CreateInstance(type, new object[] { });
                    }
                    break;

                case PluginRegistrationType.Workflow:
                    instance = Activator.CreateInstance(type, new object[] { });
                    break;

                case PluginRegistrationType.CustomApi:
                    if (type.GetConstructor(new[] { typeof(string), typeof(string) }) != null)
                    {
                        instance = Activator.CreateInstance(type, new object[] { null, null });
                    }
                    else
                    {
                        instance = Activator.CreateInstance(type, new object[] { });
                    }
                    break;

                default:
                    throw new InvalidEnumArgumentException("Unknown PluginRegistrationType");
            }
            return instance;
        }


        public List<T> CreateInstanceOfTypeList<T>(List<Type> types, PluginRegistrationType kind)
        {
            List<T> list = new List<T>();
            foreach (var type in types)
            {
                dynamic temp = CreateInstanceOfType(type, kind);
                switch (kind)
                {
                    case PluginRegistrationType.Plugin:
                        list.Add(Plugin.FromXrmFrameworkPlugin(temp, false));
                        break;

                    case PluginRegistrationType.Workflow:
                        list.Add(Plugin.FromXrmFrameworkPlugin(temp, true));
                        break;

                    case PluginRegistrationType.CustomApi:
                        list.Add(CustomApi.FromXrmFrameworkCustomApi(temp, _registrationContext.Publisher.CustomizationPrefix));
                        break;

                    default:
                        throw new InvalidEnumArgumentException("Unknown PluginRegistrationType");
                }
            }
            return list;
        }
    }
}
