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


        public void Register<TPlugin>(string projectName)
        {
            Console.Write("Fetching Local Assembly...");

            var localAssembly = _assemblyFactory.CreateFromLocalAssemblyContext(typeof(TPlugin));

            Console.WriteLine("Fetching Registered Assembly...");

            var registeredAssembly = _assemblyFactory.CreateFromRemoteAssemblyContext(_registrationService, projectName);


            Console.Write("Computing Difference...");

            AssemblyDiffFactory.ComputeAssemblyDiff(localAssembly, registeredAssembly);


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