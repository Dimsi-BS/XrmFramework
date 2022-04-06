// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Configuration;
using System.Linq;
using XrmFramework.Definitions;
using XrmFramework.DeployUtils.Configuration;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Service;
using XrmFramework.DeployUtils.Utils;

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
            var serviceProvider = InitServiceProvider(projectName);

            var solutionSettings = serviceProvider.GetRequiredService<IOptions<SolutionSettings>>();
            Console.WriteLine($"You are about to deploy on organization:\n{solutionSettings.Value.ConnectionString.Replace(";", "\n")} If ok press any key.");
            Console.ReadKey();
            Console.WriteLine("Connecting to CRM...");

            var registrationHelper = serviceProvider.GetRequiredService<RegistrationHelper>();

            registrationHelper.Register<TPlugin>(projectName);
        }

        private static IServiceProvider InitServiceProvider(string projectName)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddScoped<IRegistrationService, RegistrationService>();
            serviceCollection.AddScoped<ISolutionContext, SolutionContext>();
            serviceCollection.AddScoped<IAssemblyExporter, AssemblyExporter>();
            serviceCollection.AddScoped<IAssemblyImporter, AssemblyImporter>();
            serviceCollection.AddSingleton<IAssemblyFactory, AssemblyFactory>();
            serviceCollection.AddSingleton<RegistrationHelper>();

            ParseSolutionSettings(projectName, out string pluginSolutionUniqueName, out string connectionString);

            serviceCollection.Configure<SolutionSettings>((settings) =>
            {
                settings.ConnectionString = connectionString;
                settings.PluginSolutionUniqueName = pluginSolutionUniqueName;
            });

            return serviceCollection.BuildServiceProvider();
        }

        protected void Register<TPlugin>(string projectName)
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
            if (addSolutionComponentRequest != null)
            {
                _registrationService.Execute(addSolutionComponentRequest);
            }
        }

        private void CleanAssembly()
        {
            // Delete
            _assemblyExporter.DeleteAllComponents(_flatAssemblyContext.StepImages);
            _assemblyExporter.DeleteAllComponents(_flatAssemblyContext.Steps);
            _assemblyExporter.DeleteAllComponents(_flatAssemblyContext.Plugins);
            _assemblyExporter.DeleteAllComponents(_flatAssemblyContext.CustomApiRequestParameters);
            _assemblyExporter.DeleteAllComponents(_flatAssemblyContext.CustomApiResponseProperties);
            _assemblyExporter.DeleteAllComponents(_flatAssemblyContext.CustomApis);

            var customApisTypeToDelete = _flatAssemblyContext.CustomApis
                    .Where(x => x.RegistrationState == RegistrationState.ToDelete)
                    .Select(x => x.PluginTypeId.Id)
                    .ToList();

            _registrationService.DeleteMany(PluginTypeDefinition.EntityName, customApisTypeToDelete);
        }

        private void RegisterPlugins()
        {
            _assemblyExporter.CreateAllComponents(_flatAssemblyContext.Plugins);
            _assemblyExporter.CreateAllComponents(_flatAssemblyContext.Steps, doAddToSolution: true);
            _assemblyExporter.CreateAllComponents(_flatAssemblyContext.StepImages);

            _assemblyExporter.UpdateAllComponents(_flatAssemblyContext.Plugins);
            _assemblyExporter.UpdateAllComponents(_flatAssemblyContext.Steps);
            _assemblyExporter.UpdateAllComponents(_flatAssemblyContext.StepImages);
        }

        private void RegisterWorkflows()
        {
            _assemblyExporter.CreateAllComponents(_flatAssemblyContext.Workflows);

            _assemblyExporter.UpdateAllComponents(_flatAssemblyContext.Workflows);
        }

        private void RegisterCustomApis()
        {
            var customApisTypeToCreate = _flatAssemblyContext.CustomApis
                    .Where(x => x.RegistrationState == RegistrationState.ToCreate)
                    .ToList();
            foreach (var customApi in customApisTypeToCreate)
            {
                var customApiPluginType = _assemblyExporter.ToRegisterPluginType(_flatAssemblyContext.Assembly.Id, customApi.FullName);
                var id = _registrationService.Create(customApiPluginType);
                customApi.PluginTypeId.Id = id;
            }

            _assemblyExporter.CreateAllComponents(_flatAssemblyContext.CustomApis, true, true);
            _assemblyExporter.CreateAllComponents(_flatAssemblyContext.CustomApiRequestParameters, true, true);
            _assemblyExporter.CreateAllComponents(_flatAssemblyContext.CustomApiResponseProperties, true, true);

            _assemblyExporter.UpdateAllComponents(_flatAssemblyContext.CustomApis);
            _assemblyExporter.UpdateAllComponents(_flatAssemblyContext.CustomApiRequestParameters);
            _assemblyExporter.UpdateAllComponents(_flatAssemblyContext.CustomApiResponseProperties);
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