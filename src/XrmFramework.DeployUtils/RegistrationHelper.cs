// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Configuration;
using System.Linq;
using XrmFramework.DeployUtils.Configuration;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Service;
using XrmFramework.DeployUtils.Utils;

namespace XrmFramework.DeployUtils
{
    public class RegistrationHelper
    {
        private readonly IRegistrationService _registrationService;
        private readonly IAssemblyExporter _assemblyExporter;
        private readonly IAssemblyFactory _assemblyFactory;
        private readonly AssemblyDiffFactory _assemblyDiffFactory;

        private IDiffPatch _registrationStrategy;

        public RegistrationHelper(IRegistrationService registrationService,
                                  IAssemblyExporter assemblyExporter,
                                  IAssemblyFactory assemblyFactory,
                                  AssemblyDiffFactory assemblyDiffFactory)
        {
            _registrationService = registrationService;
            _assemblyExporter = assemblyExporter;
            _assemblyFactory = assemblyFactory;
            _assemblyDiffFactory = assemblyDiffFactory;
        }

        public static void RegisterPluginsAndWorkflows<TPlugin>(string projectName)
        {
            var serviceProvider = InitServiceProvider(projectName);

            var solutionSettings = serviceProvider.GetRequiredService<IOptions<SolutionSettings>>();
            Console.WriteLine($@"You are about to deploy on organization:\n
{solutionSettings.Value.ConnectionString.Replace(";", "\n")}
If ok press any key.");
            Console.ReadKey();
            Console.WriteLine("Connecting to CRM...");

            var registrationHelper = serviceProvider.GetRequiredService<RegistrationHelper>();

            registrationHelper.Register<TPlugin>(projectName);
        }

        protected void Register<TPlugin>(string projectName)
        {
            Console.WriteLine("Fetching Local Assembly...");

            var localAssembly = _assemblyFactory.CreateFromLocalAssemblyContext(typeof(TPlugin));

            Console.WriteLine("Fetching Remote Assembly...");

            var registeredAssembly = _assemblyFactory.CreateFromRemoteAssemblyContext(_registrationService, projectName);

            Console.Write("Computing Difference...");

            _registrationStrategy = _assemblyDiffFactory.ComputeDiffFromPools(localAssembly, registeredAssembly);

            var stepsForMetadata = _registrationStrategy
                .RetrieveWhere(c => c.Component is Step)
                .Select(s => (Step)s)
                .ToList();

            _assemblyExporter.InitExportMetadata(stepsForMetadata);

            Console.WriteLine();
            Console.WriteLine("Registering assembly");

            RegisterAssembly();

            var componentsToCreate = _registrationStrategy
                .RetrieveWhere(d => d.DiffResult == RegistrationState.ToCreate)
                .ToList();

            componentsToCreate.Sort((x, y) => x.Rank.CompareTo(y.Rank));

            _assemblyExporter.CreateAllComponents(componentsToCreate);

            var componentsToUpdate = _registrationStrategy
                .RetrieveWhere(d => d.DiffResult == RegistrationState.ToUpdate)
                .ToList();

            _assemblyExporter.UpdateAllComponents(componentsToUpdate);
        }

        private void RegisterAssembly()
        {
            var assembly = _registrationStrategy.PluginAssembly;

            if (assembly.RegistrationState == RegistrationState.ToCreate)
            {
                Console.WriteLine("Creating assembly");

                assembly.Id = _registrationService.Create(assembly);
            }
            else if (assembly.RegistrationState == RegistrationState.ToUpdate)
            {
                Console.WriteLine();
                Console.WriteLine(@"Cleaning Assembly");

                CleanAssembly();

                Console.WriteLine();
                Console.WriteLine("Updating plugin assembly");

                _registrationService.Update(assembly);
            }
            var addSolutionComponentRequest = _assemblyExporter.CreateAddSolutionComponentRequest(assembly.ToEntityReference());
            if (addSolutionComponentRequest != null)
            {
                _registrationService.Execute(addSolutionComponentRequest);
            }
        }

        private void CleanAssembly()
        {
            var componentsToDelete = _registrationStrategy
                .RetrieveWhere(d => d.DiffResult == RegistrationState.ToDelete)
                .ToList();

            //Sort in descending order so that children are deleted before their parent
            componentsToDelete.Sort((x, y) => -x.Rank.CompareTo(y.Rank));

            _assemblyExporter.DeleteAllComponents(componentsToDelete);
        }

        private static void ParseSolutionSettings(string projectName, out string pluginSolutionUniqueName, out string connectionString)
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

        private static IServiceProvider InitServiceProvider(string projectName)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddScoped<IRegistrationService, RegistrationService>();
            serviceCollection.AddScoped<ISolutionContext, SolutionContext>();
            serviceCollection.AddScoped<IAssemblyExporter, AssemblyExporter>();
            serviceCollection.AddScoped<IAssemblyImporter, AssemblyImporter>();
            serviceCollection.AddScoped<ICrmComponentComparer, CrmComponentComparer>();
            serviceCollection.AddScoped<ICrmComponentConverter, CrmComponentConverter>();
            serviceCollection.AddScoped<AssemblyDiffFactory>();
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

    }
}