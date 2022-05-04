﻿// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
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

        /// <summary>
        /// Entrypoint for registering the <typeparamref name="TPlugin"/> in the solution <paramref name="projectName"/>
        /// </summary>
        /// <typeparam name="TPlugin"></typeparam>
        /// <param name="projectName"></param>
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

        /// <summary>
        /// Main algorithm for deploying a <typeparam name="TPlugin"></typeparam> assembly into the <paramref name="projectName"/>
        /// </summary>
        /// <typeparam name="TPlugin"></typeparam>
        /// <param name="projectName"></param>
        protected void Register<TPlugin>(string projectName)
        {
            Console.WriteLine("Fetching Local Assembly...");

            var localAssembly = _assemblyFactory.CreateFromLocalAssemblyContext(typeof(TPlugin));

            Console.WriteLine("Fetching Remote Assembly...");

            var registeredAssembly = _assemblyFactory.CreateFromRemoteAssemblyContext(_registrationService, projectName);

            Console.Write("Computing Difference...");

            var registrationStrategy = _assemblyDiffFactory.ComputeDiffPatch(localAssembly, registeredAssembly);

            ExecuteStrategy(registrationStrategy);
        }

        /// <summary>
        /// Deploy the <paramref name="strategy"/>'s components according to their <see cref="RegistrationState"/>
        /// </summary>
        /// <param name="strategy"></param>
        private void ExecuteStrategy(IAssemblyContext strategy)
        {
            var strategyPool = strategy.ComponentsOrderedPool;

            var stepsForMetadata = strategyPool.OfType<Step>();

            _assemblyExporter.InitExportMetadata(stepsForMetadata);

            Console.WriteLine();
            Console.WriteLine("Registering assembly");

            RegisterAssembly(strategy);

            var componentsToCreate = strategyPool
                .Where(d => d.RegistrationState == RegistrationState.ToCreate);

            _assemblyExporter.CreateAllComponents(componentsToCreate);

            var componentsToUpdate = strategyPool
                .Where(d => d.RegistrationState == RegistrationState.ToUpdate);

            _assemblyExporter.UpdateAllComponents(componentsToUpdate);
        }

        /// <summary>Create or Update the Assembly, deleting all obsolete components in the process</summary>
        private void RegisterAssembly(IAssemblyContext strategy)
        {

            if (strategy.RegistrationState == RegistrationState.ToCreate)
            {
                Console.WriteLine("Creating assembly");

                _assemblyExporter.CreateComponent(strategy.AssemblyInfo);
                strategy.RegistrationState = RegistrationState.Computed;
                return;
            }

            Console.WriteLine();
            Console.WriteLine(@"Cleaning Assembly");

            CleanAssembly(strategy);

            if (strategy.RegistrationState != RegistrationState.ToUpdate) return;

            Console.WriteLine();
            Console.WriteLine("Updating plugin assembly");

            _assemblyExporter.UpdateComponent(strategy.AssemblyInfo);
            strategy.RegistrationState = RegistrationState.Computed;
        }

        /// <summary>Deletes all components with <see cref="RegistrationState.ToDelete"/></summary>
        private void CleanAssembly(IAssemblyContext strategy)
        {
            var strategyPool = strategy.ComponentsOrderedPool;

            var componentsToDelete = strategyPool
                .Where(d => d.RegistrationState == RegistrationState.ToDelete);

            _assemblyExporter.DeleteAllComponents(componentsToDelete);
        }
    }
}