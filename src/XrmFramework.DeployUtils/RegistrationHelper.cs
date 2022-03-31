// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using XrmFramework.Definitions;
using XrmFramework.DeployUtils.Configuration;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Utils;
using XrmFramework.DeployUtils.Service;
using Microsoft.Extensions.DependencyInjection;

namespace XrmFramework.DeployUtils
{ 
    public class RegistrationHelper
    {
        private readonly IRegistrationService _registrationService;
        private readonly IAssemblyExporter _assemblyExporter;
        private readonly IAssemblyFactory _assemblyFactory;
        private IFlatAssemblyContext _flatAssemblyContext;

        public RegistrationHelper(IRegistrationService registrationService,
                                  IAssemblyExporter assemblyExporter,
                                  IAssemblyFactory assemblyFactory)
        {
            _registrationService = registrationService;
            _assemblyExporter = assemblyExporter;
            _assemblyFactory = assemblyFactory;
        }

        public static void RegisterPluginsAndWorkflows<TPlugin>(string projectName)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddScoped<IRegistrationService, RegistrationService>();
            serviceCollection.AddScoped<ISolutionContext, SolutionContext>();
            serviceCollection.AddScoped<IAssemblyExporter, AssemblyExporter>();
            serviceCollection.AddScoped<IAssemblyImporter, AssemblyImporter>();
            serviceCollection.AddSingleton<IAssemblyFactory, AssemblyFactory>();
            serviceCollection.AddSingleton<RegistrationHelper>();

            string pluginSolutionUniqueName, connectionString;

            ParseSolutionSettings(projectName, out pluginSolutionUniqueName, out connectionString);

            serviceCollection.Configure<SolutionSettings>((settings) => 
            {
                settings.ConnectionString = connectionString;
                settings.PluginSolutionUniqueName = pluginSolutionUniqueName;
            });

            var serviceProvider = serviceCollection.BuildServiceProvider();

            Console.WriteLine($"You are about to deploy on {connectionString} organization. If ok press any key.");
            Console.ReadKey();
            Console.WriteLine("Connecting to CRM...");

            var registrationHelper = serviceProvider.GetRequiredService<RegistrationHelper>();

            registrationHelper.Register<TPlugin>(projectName);
        }
        public static void UpdateRemoteDebuggerPlugin<TPlugin>(string projectName)
        {
            Console.WriteLine(projectName);
            // Connect a service to the debug session
            var xrmFrameworkConfigSection = ConfigHelper.GetSection();
            var projectConfig = xrmFrameworkConfigSection.Projects.OfType<ProjectElement>()
                .FirstOrDefault(p => p.Name == projectName);
            if (projectConfig == null)
            {
                var defaultColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"No reference to the project {projectName} has been found in the xrmFramework.config file.");
                Console.ForegroundColor = defaultColor;
                return;
            }
            //var pluginSolutionUniqueName = "dimsi_debugsession";
            var pluginSolutionUniqueName = projectConfig.TargetSolution;
            var connectionString = ConfigurationManager.ConnectionStrings[xrmFrameworkConfigSection.SelectedConnection].ConnectionString;

            Console.WriteLine($"You are about to modify the debug session");
            Console.WriteLine($"Do you want to register new steps to debug ? (y/n)");
            var r = Console.ReadLine();
            while (r != "y" && r != "n")
            {
                Console.WriteLine($"Do you want to register new steps to debug ? (y/n)");
                r = Console.ReadLine();
            }
            if (r == "n")
            {
                return;
            }

            Console.WriteLine("Connecting to CRM...");
            CrmServiceClient.MaxConnectionTimeout = TimeSpan.FromMinutes(10);

            //Create the CrmServiceClient to interact with the CrmProject 
            var service = new CrmServiceClient(connectionString);
            Console.WriteLine("Created serviceClient");
            //Kind of deprecated, allow use of early bound classes like in DeployUtils.Model.Entities, to make a strongly typed object from a table 
            //service.OrganizationServiceProxy?.EnableProxyTypes();
            var debugAssembly = GetAssemblyByName(service, "XrmFramework.RemoteDebuggerPlugin");

            //InitMetadata(service, pluginSolutionUniqueName);
            InitStepMetadata(service, pluginSolutionUniqueName);


            //Now get the local assembly for the plugin(s) to be debugged
            var pluginAssembly = typeof(TPlugin).Assembly;
            //Get each possible type of plugin
            var pluginType = pluginAssembly.GetType("XrmFramework.Plugin");
            var customApiType = pluginAssembly.GetType("XrmFramework.CustomApi");
            var workflowType = pluginAssembly.GetType("XrmFramework.Workflow.CustomWorkflowActivity");
            Console.WriteLine(pluginType.Name);
            var pluginList = new List<Plugin>();
            var customApis = new List<CustomApi>();
            // Get all plugin types that were developped by the users
            var pluginTypes = pluginAssembly.GetTypes().Where(t => pluginType.IsAssignableFrom(t) && !customApiType.IsAssignableFrom(t) && t.IsPublic && !t.IsAbstract).ToList();
            // Now for each local plugin we assign steps to the debug plugin
            GetPluginData(pluginTypes, pluginList);


            // Now we get the remoteDebugger plugin
            var debuggerPlugin = GetRegisteredPluginTypes(service, debugAssembly.Id).ToList()[0];
            Console.WriteLine("name of debugger plugin is {0}", debuggerPlugin.Name);
            var registeredSteps = GetRegisteredSteps(service, debugAssembly.Id);
            var registeredImages = GetRegisteredImages(service, debugAssembly.Id);

            var registeredStepsForPluginType = registeredSteps.Where(s => s.EventHandler.Id == debuggerPlugin.Id).ToList();
            foreach (var step in registeredStepsForPluginType)
            {

                service.Delete(SdkMessageProcessingStep.EntityLogicalName, step.Id);
                //registeredStepsForPluginType.Remove(step);
            }
            foreach (var image in registeredImages)
            {
                service.Delete(SdkMessageProcessingStepImage.EntityLogicalName, image.Id);
                //registeredImages.Remove(image);
            }

            foreach (var plugin in pluginTypes)
            {
                foreach (var pluginData in pluginList)
                {
                    if (pluginData.FullName == plugin.FullName)
                    {
                        foreach (var convertedStep in pluginData.Steps)
                        {
                            Console.WriteLine("There is a step here");

                            if (convertedStep.Message != Messages.Associate.ToString() && convertedStep.Message != Messages.Lose.ToString() && convertedStep.Message != Messages.Win.ToString())
                            {
                                convertedStep.UnsecureConfig = plugin.AssemblyQualifiedName;
                                //convertedStep.SecuredConfig = "oulalalalolo";
                                var stepToRegister = GetStepToRegister(debuggerPlugin.Id, convertedStep);

                                //Console.WriteLine(s.ImpersonationUsername);
                                //stepToRegister.

                                stepToRegister.Id = service.Create(stepToRegister);
                                AddSolutionComponentToSolution(service, pluginSolutionUniqueName, stepToRegister.ToEntityReference());

                                if (convertedStep.PostImageUsed && convertedStep.Message != Messages.Delete.ToString())
                                {

                                    var postImage = GetImageToRegister(service, stepToRegister.Id, convertedStep, false);
                                    postImage.Id = service.Create(postImage);
                                    AddSolutionComponentToSolution(service, pluginSolutionUniqueName, postImage.ToEntityReference());


                                }


                                //Add the relevant preimages for each step
                                //var registeredPreImage = registeredImages.FirstOrDefault(i => i.Name == "PreImage" && i.SdkMessageProcessingStepId.Id == stepToRegister.Id);

                                if (convertedStep.PreImageUsed)
                                {
                                    var preImage = GetImageToRegister(service, stepToRegister.Id, convertedStep, true);
                                    preImage.Id = service.Create(preImage);
                                    AddSolutionComponentToSolution(service, pluginSolutionUniqueName, preImage.ToEntityReference());



                                }

                            }
                        }
                    }
                }

            }


        }



        public void Register<TPlugin>(string projectName)
        {
            Console.Write("Fetching Local Assembly...");

            var localAssembly = _assemblyFactory.CreateFromLocalAssemblyContext(typeof(TPlugin));

            Console.WriteLine("Fetching Registered Assembly...");

            var registeredAssembly = _assemblyFactory.CreateFromRemoteAssemblyContext(_registrationService, projectName);


            Console.Write("Computing Difference...");

            AssemblyDiffFactory.ComputeAssemblyDiff(localAssembly, registeredAssembly);

            InitMetadata(service, pluginSolutionUniqueName);
            // Get the assembly that contains the classes Plugin, CustomWorkflowActivity and CustomApi that are needed in order to understand what will be registered
            var pluginAssembly = typeof(TPlugin).Assembly;
            //Get each possible type of plugin
            var pluginType = pluginAssembly.GetType("XrmFramework.Plugin");
            var customApiType = pluginAssembly.GetType("XrmFramework.CustomApi");
            var workflowType = pluginAssembly.GetType("XrmFramework.Workflow.CustomWorkflowActivity");

            _flatAssemblyContext = _assemblyFactory.CreateFlatAssemblyContextFromAssemblyContext(localAssembly);

            Console.WriteLine();
            Console.WriteLine("Registering assembly");

            RegisterAssembly();

            Console.WriteLine();
            Console.WriteLine(@"Registering Plugins");

            RegisterPlugins();

            Console.WriteLine();
            Console.WriteLine(@"Registering Custom Workflow activities");

            RegisterWorkflows();

            Console.WriteLine();
            Console.WriteLine(@"Registering Custom Apis");

            RegisterCustomApis();
        }

        private void RegisterAssembly()
        {
            if (_flatAssemblyContext.Assembly.RegistrationState == RegistrationState.ToCreate)
            {
                Console.WriteLine("Creating assembly");

                _flatAssemblyContext.Assembly.Id = _registrationService.Create(_flatAssemblyContext.Assembly);
            }
            else if (_flatAssemblyContext.Assembly.RegistrationState == RegistrationState.ToUpdate)
            {
                Console.WriteLine();
                Console.WriteLine(@"Assembly Exists, Cleaning Assembly");

                CleanAssembly();

                Console.WriteLine("Updating plugin assembly");

                _registrationService.Update(_flatAssemblyContext.Assembly);
            }
            var addSolutionComponentRequest = _assemblyExporter.CreateAddSolutionComponentRequest(_flatAssemblyContext.Assembly.ToEntityReference());
            if(addSolutionComponentRequest != null)
            {
                _registrationService.Execute(addSolutionComponentRequest);
            }
        }

        private void CleanAssembly()
        {
            // Delete
            DeleteAllComponents(_flatAssemblyContext.StepImages);
            DeleteAllComponents(_flatAssemblyContext.Steps);
            DeleteAllComponents(_flatAssemblyContext.Plugins);
            DeleteAllComponents(_flatAssemblyContext.CustomApiRequestParameters);
            DeleteAllComponents(_flatAssemblyContext.CustomApiResponseProperties);
            DeleteAllComponents(_flatAssemblyContext.CustomApis);

            var customApisTypeToDelete = _flatAssemblyContext.CustomApis
                    .Where(x => x.RegistrationState == RegistrationState.ToDelete)
                    .Select(x => x.PluginTypeId.Id)
                    .ToList();

            _registrationService.DeleteMany(PluginTypeDefinition.EntityName, customApisTypeToDelete);
        }

        private void RegisterPlugins()
        {
            CreateAllComponents(_flatAssemblyContext.Plugins);
            CreateAllComponents(_flatAssemblyContext.Steps, doAddToSolution: true);
            CreateAllComponents(_flatAssemblyContext.StepImages);

            UpdateAllComponents(_flatAssemblyContext.Plugins);
            UpdateAllComponents(_flatAssemblyContext.Steps);
            UpdateAllComponents(_flatAssemblyContext.StepImages);
        }

        private void RegisterWorkflows()
        {
            CreateAllComponents(_flatAssemblyContext.Workflows);

            UpdateAllComponents(_flatAssemblyContext.Workflows);
        }

        private void RegisterCustomApis()
        {
            var customApisTypeToCreate = _flatAssemblyContext.CustomApis
                    .Where(x => x.RegistrationState == RegistrationState.ToCreate)
                    .ToList();
            foreach(var customApi in customApisTypeToCreate)
            {
                var customApiPluginType = _assemblyExporter.ToRegisterPluginType(_flatAssemblyContext.Assembly.Id, customApi.FullName);
                var id = _registrationService.Create(customApiPluginType);
                customApi.PluginTypeId.Id = id;
            }

            CreateAllComponents(_flatAssemblyContext.CustomApis, true, true);
            CreateAllComponents(_flatAssemblyContext.CustomApiRequestParameters, true, true);
            CreateAllComponents(_flatAssemblyContext.CustomApiResponseProperties, true, true);

            UpdateAllComponents(_flatAssemblyContext.CustomApis);
            UpdateAllComponents(_flatAssemblyContext.CustomApiRequestParameters);
            UpdateAllComponents(_flatAssemblyContext.CustomApiResponseProperties);
        }

        private void UpdateAllComponents<T>(IEnumerable<T> components) where T : ISolutionComponent
        {
            var updateComponents = components
                .Where(x => x.RegistrationState == RegistrationState.ToUpdate)
                .ToList();
            foreach (var component in updateComponents)
            {
                var registeringComponent = _assemblyExporter.ToRegisterComponent(component);
                _registrationService.Update(registeringComponent);
            }
        }

        private void CreateAllComponents<T>(IEnumerable<T> components,
                                            bool doAddToSolution = false,
                                            bool doFetchCode = false) where T : ISolutionComponent
        {
            var createComponents = components
                .Where(x => x.RegistrationState == RegistrationState.ToCreate)
                .ToList();

            if (!createComponents.Any()) return;
            int? entityTypeCode = doFetchCode ? _registrationService?.GetIntEntityTypeCode(createComponents.FirstOrDefault()?.EntityTypeName) : null;
            foreach (var component in createComponents)
            {
                var registeringComponent = _assemblyExporter.ToRegisterComponent(component);
                component.Id = _registrationService.Create(registeringComponent);
                registeringComponent.Id = component.Id;
                if (doAddToSolution)
                {
                    var addSolutionComponentRequest = _assemblyExporter.CreateAddSolutionComponentRequest(registeringComponent.ToEntityReference(), entityTypeCode);
                    if (addSolutionComponentRequest != null)
                    {
                        _registrationService.Execute(addSolutionComponentRequest);
                    }
                }
            }
        }

        private void DeleteAllComponents<T>(IEnumerable<T> components) where T : ISolutionComponent
        {
            var deleteComponents = components
                .Where(x => x.RegistrationState == RegistrationState.ToDelete)
                .ToList();
            foreach (var component in deleteComponents)
            {
                _registrationService.Delete(component.EntityTypeName, component.Id);
            }
        }

        public static void ParseSolutionSettings(string projectName, out string pluginSolutionUniqueName, out string connectionString)
        {
            var xrmFrameworkConfigSection = ConfigHelper.GetSection();

            var projectConfig = xrmFrameworkConfigSection.Projects.OfType<ProjectElement>()
                .FirstOrDefault(p => p.Name == projectName);

            if (projectConfig == null)
            {
                var defaultColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"No reference to the project {projectName} has been found in the xrmFramework.config file.");
                Console.ForegroundColor = defaultColor;
                System.Environment.Exit(1);
            }

            pluginSolutionUniqueName = projectConfig.TargetSolution;

            connectionString = ConfigurationManager.ConnectionStrings[xrmFrameworkConfigSection.SelectedConnection].ConnectionString;
        }
    }

    
}