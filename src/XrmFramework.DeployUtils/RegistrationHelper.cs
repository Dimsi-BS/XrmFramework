// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.DeployUtils.Configuration;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Service;
using XrmFramework.DeployUtils.Utils;

namespace XrmFramework.DeployUtils
{
    public partial class RegistrationHelper
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
            var serviceProvider = ServiceCollectionHelper.ConfigureForDeploy(projectName);

            var solutionSettings = serviceProvider.GetRequiredService<IOptions<SolutionSettings>>();

            Console.WriteLine($@"You are about to deploy on organization:\n
{solutionSettings.Value.ConnectionString.Replace(";", "\n")}
If ok press any key.");
            //Console.ReadKey();
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

            _registrationStrategy = _assemblyDiffFactory.ComputeDiffPatchFromAssemblies(localAssembly, registeredAssembly);

            //ExecuteRegistrationStrategy();
        }

        private void ExecuteRegistrationStrategy()
        {
            var stepsForMetadata = _registrationStrategy
                .GetComponentsWhere(c => c.Component is Step)
                .Select(s => (Step)s)
                .ToList();

            _assemblyExporter.InitExportMetadata(stepsForMetadata);

            Console.WriteLine();
            Console.WriteLine("Registering assembly");

            RegisterAssembly();

            var componentsToCreate = _registrationStrategy
                .GetComponentsWhere(d => d.DiffResult == RegistrationState.ToCreate);

            _assemblyExporter.CreateAllComponents(componentsToCreate);

            var componentsToUpdate = _registrationStrategy
                .GetComponentsWhere(d => d.DiffResult == RegistrationState.ToUpdate);

            _assemblyExporter.UpdateAllComponents(componentsToUpdate);
        }

        private void RegisterAssembly()
        {
            var assembly = _registrationStrategy.PluginAssembly;

            if (assembly.RegistrationState == RegistrationState.ToCreate)
            {
                Console.WriteLine("Creating assembly");

                _assemblyExporter.CreateComponent(assembly);
            }
            else if (assembly.RegistrationState == RegistrationState.ToUpdate)
            {
                Console.WriteLine();
                Console.WriteLine(@"Cleaning Assembly");

                CleanAssembly();

                Console.WriteLine();
                Console.WriteLine("Updating plugin assembly");

                _assemblyExporter.UpdateComponent(assembly);
            }
        }

        private void CleanAssembly()
        {
            var componentsToDelete = _registrationStrategy
                .GetComponentsWhere(d => d.DiffResult == RegistrationState.ToDelete);

            //Sort in descending order so that children are deleted before their parent

            _assemblyExporter.DeleteAllComponents(componentsToDelete);
        }
        public static void AutoMapperTest<TPlugin>(string projectName)
        {
            var serviceProvider = ServiceCollectionHelper.ConfigureForDeploy("FrameworkTestsAymeric2.Plugins");

            var factory = serviceProvider.GetRequiredService<IAssemblyFactory>();
            var mapper = serviceProvider.GetRequiredService<IMapper>();

            var localAssembly = factory.CreateFromLocalAssemblyContext(typeof(TPlugin));

            var localAssemblyPool = localAssembly.ComponentsOrderedPool;
            var localAssemblyCopy = mapper.Map<IAssemblyContext>(localAssembly);

            var OrAssembly = localAssembly.Assembly;
            var ClAssembly2 = mapper.Map<PluginAssembly>(localAssembly.Assembly);
            var ClAssembly = localAssemblyCopy.Assembly;


            Console.WriteLine($"\nOriginal Assembly Id is {OrAssembly.Id}");
            Console.WriteLine($"Clone Assembly Id is {ClAssembly.Id}");
            Console.WriteLine($"Messing with Clone Assembly Id ...");
            ClAssembly.Id = Guid.NewGuid();
            Console.WriteLine($"Original Assembly Id is {OrAssembly.Id}");
            Console.WriteLine($"Clone Assembly Id is {ClAssembly.Id}");

            var plug = (Plugin)localAssemblyPool.First(c => c is Plugin);
            var original = plug.Steps;
            var clone = mapper.Map<StepCollection>(original);


            MessWith<PluginAssembly>(mapper, localAssemblyPool);
            MessWith<StepImage>(mapper, localAssemblyPool);
            MessWith<Step>(mapper, localAssemblyPool);
            MessWith<Plugin>(mapper, localAssemblyPool);
            MessWith<CustomApiRequestParameter>(mapper, localAssemblyPool);
            MessWith<CustomApiResponseProperty>(mapper, localAssemblyPool);
            MessWith<CustomApi>(mapper, localAssemblyPool);
        }

        private static void MessWith<T>(IMapper mapper, IReadOnlyCollection<ICrmComponent> pool) where T : ICrmComponent
        {
            try
            {
                var original = pool.First(c => c is T);
                var clone = mapper.Map<T>(original);

                Console.WriteLine($"\nOriginal {typeof(T).Name} Id is {original.Id}");
                Console.WriteLine($"Clone {typeof(T).Name} Id is {clone.Id}");
                Console.WriteLine($"Messing with Clone {typeof(T).Name} Id ...");
                clone.Id = Guid.Empty;
                Console.WriteLine($"Original {typeof(T).Name} Id is {original.Id}");
                Console.WriteLine($"Clone {typeof(T).Name} Id is {clone.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n{ex.Message}\n");
            }
        }

    }
}