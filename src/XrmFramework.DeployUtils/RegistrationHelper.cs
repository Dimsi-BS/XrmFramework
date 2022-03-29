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

namespace XrmFramework.DeployUtils
{
    public static class RegistrationHelper
    {
        private static IRegistrationService _registrationService;
        private static IAssemblyExporter _assemblyExporter;
        private static IFlatAssemblyContext _flatAssemblyContext;

        public static void RegisterPluginsAndWorkflows<TPlugin>(string projectName)
        {
            string pluginSolutionUniqueName, connectionString;

            ParseSolutionSettings(projectName, out pluginSolutionUniqueName, out connectionString);

            Console.WriteLine($"You are about to deploy on {connectionString} organization. If ok press any key.");
            Console.ReadKey();
            Console.WriteLine("Connecting to CRM...");

            RegistrationService.MaxConnectionTimeout = TimeSpan.FromMinutes(10);

            _registrationService = new RegistrationService(connectionString);

            _registrationService.OrganizationServiceProxy?.EnableProxyTypes();

            var solutionContext = new SolutionContext(_registrationService, pluginSolutionUniqueName);

            var assemblyImporter = new AssemblyImporter(solutionContext);
            _assemblyExporter = new AssemblyExporter(solutionContext);
            var assemblyFactory = new AssemblyFactory(assemblyImporter);


            Console.Write("Fetching Local Assembly...");

            var localAssembly = assemblyFactory.CreateFromLocalAssemblyContext(typeof(TPlugin));

            Console.WriteLine("Fetching Registered Assembly...");

            var registeredAssembly = assemblyFactory.CreateFromRemoteAssemblyContext(_registrationService, projectName);


            Console.Write("Computing Difference...");

            AssemblyDiffFactory.ComputeAssemblyDiff(localAssembly, registeredAssembly);


            _flatAssemblyContext = assemblyFactory.CreateFlatAssemblyContextFromAssemblyContext(localAssembly);

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

        public static void RegisterAssembly()
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

        public static void CleanAssembly()
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

        public static void RegisterPlugins()
        {
            CreateAllComponents(_flatAssemblyContext.Plugins);
            CreateAllComponents(_flatAssemblyContext.Steps, doAddToSolution: true);
            CreateAllComponents(_flatAssemblyContext.StepImages);

            UpdateAllComponents(_flatAssemblyContext.Plugins);
            UpdateAllComponents(_flatAssemblyContext.Steps);
            UpdateAllComponents(_flatAssemblyContext.StepImages);
        }

        public static void RegisterWorkflows()
        {
            CreateAllComponents(_flatAssemblyContext.Workflows);

            UpdateAllComponents(_flatAssemblyContext.Workflows);
        }

        public static void RegisterCustomApis()
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

        private static void UpdateAllComponents<T>(IEnumerable<T> components) where T : ISolutionComponent
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

        private static void CreateAllComponents<T>(IEnumerable<T> components,
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

        private static void DeleteAllComponents<T>(IEnumerable<T> components) where T : ISolutionComponent
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