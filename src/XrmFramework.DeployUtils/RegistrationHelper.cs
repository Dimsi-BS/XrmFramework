// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Deploy;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Navigation;
using XrmFramework.Definitions;
using XrmFramework.DeployUtils.Configuration;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Service;
using XrmFramework.DeployUtils.Utils;

namespace XrmFramework.DeployUtils
{
    public static class RegistrationHelper
    {
        private static ISolutionContext _solutionContext;

        public static void RegisterPluginsAndWorkflows<TPlugin>(string projectName)
        {
            string pluginSolutionUniqueName, connectionString;

            ParseSolutionSettings(projectName, out pluginSolutionUniqueName, out connectionString);

            Console.WriteLine($"You are about to deploy on {connectionString} organization. If ok press any key.");
            Console.ReadKey();
            Console.WriteLine("Connecting to CRM...");

            RegistrationService.MaxConnectionTimeout = TimeSpan.FromMinutes(10);

            var service = new RegistrationService(connectionString);

            service.OrganizationServiceProxy?.EnableProxyTypes();

            _solutionContext = new SolutionContext(service, pluginSolutionUniqueName);

            var assemblyFactory = new AssemblyFactory(_solutionContext);



            Console.Write("Fetching Local Assembly...");

            var localAssembly = assemblyFactory.CreateFromLocalAssemblyContext(typeof(TPlugin));
            Console.WriteLine("\tDone");

            Console.WriteLine("Fetching Registered Assembly...");

            var registeredAssembly = assemblyFactory.CreateFromRemoteAssemblyContext(service, projectName);
            Console.WriteLine("\tDone");


            Console.Write("Computing Difference...");

            AssemblyDiffFactory.ComputeAssemblyDiff(localAssembly, registeredAssembly);

            Console.WriteLine("\tDone");


            var flatAssembly = assemblyFactory.CreateFlatAssemblyContextFromAssemblyContext(localAssembly);

            Console.WriteLine();
            Console.WriteLine("Registering assembly");

            RegisterAssembly(service, flatAssembly);

            Console.WriteLine();
            Console.WriteLine(@"Registering Plugins");

            RegisterPluginsV2(service, flatAssembly);


            Console.WriteLine();
            Console.WriteLine(@"Registering Custom Workflow activities");

            RegisterWorkflowsV2(service, flatAssembly);

            Console.WriteLine();
            Console.WriteLine(@"Registering Custom Apis");

            RegisterCustomApisV2(service, flatAssembly);
        }

        public static void RegisterAssembly(IRegistrationService service, IFlatAssemblyContext assembly)
        {

            if (assembly.Assembly.RegistrationState == RegistrationState.ToCreate)
            {
                Console.WriteLine("Creating assembly");

                assembly.Assembly.Id = service.Create(assembly.Assembly);
            }
            else if (assembly.Assembly.RegistrationState == RegistrationState.ToUpdate)
            {
                Console.WriteLine();
                Console.WriteLine(@"Assembly Exists, Cleaning Assembly");

                CleanAssembly(service, assembly);

                Console.WriteLine("Updating plugin assembly");

                service.Update(assembly.Assembly);
            }
            AddSolutionComponentToSolution(service, _solutionContext.SolutionName, assembly.Assembly.ToEntityReference());
        }

        public static void CleanAssembly(IRegistrationService service, IFlatAssemblyContext assembly)
        {
            // Delete

            DeleteAllComponents(service, assembly.StepImages);
            DeleteAllComponents(service, assembly.Steps);
            DeleteAllComponents(service, assembly.Plugins);
            DeleteAllComponents(service, assembly.CustomApiRequestParameters);
            DeleteAllComponents(service, assembly.CustomApiResponseProperties);
            DeleteAllComponents(service, assembly.CustomApis);


            var customApisTypeToDelete = assembly.CustomApis
                    .Where(x => x.RegistrationState == RegistrationState.ToDelete)
                    .Select(x => x.PluginTypeId.Id)
                    .ToList();

            service.DeleteMany(PluginTypeDefinition.EntityName, customApisTypeToDelete);
        }

        public static void RegisterPluginsV2(IRegistrationService service, IFlatAssemblyContext context)
        {
            CreateAllComponents(service, context.Plugins);
            CreateAllComponents(service, context.Steps, doAddToSolution: true);
            CreateAllComponents(service, context.StepImages);

            UpdateAllComponents(service, context.Plugins);
            UpdateAllComponents(service, context.Steps);
            UpdateAllComponents(service, context.StepImages);
        }

        public static void RegisterWorkflowsV2(IRegistrationService service, IFlatAssemblyContext context)
        {
            CreateAllComponents(service, context.Workflows);

            UpdateAllComponents(service, context.Workflows);
        }

        public static void RegisterCustomApisV2(IRegistrationService service, IFlatAssemblyContext context)
        {
            var customApisTypeToCreate = context.CustomApis
                    .Where(x => x.RegistrationState == RegistrationState.ToCreate)
                    .ToList();
            foreach(var customApi in customApisTypeToCreate)
            {
                var customApiPluginType = customApi.ToPluginType(context.Assembly.Id);
                var id = service.Create(customApiPluginType);
                customApi.PluginTypeId = new EntityReference(PluginTypeDefinition.EntityName, id);
            }

            CreateAllComponents(service, context.CustomApis, true, true);
            CreateAllComponents(service, context.CustomApiRequestParameters, true, true);
            CreateAllComponents(service, context.CustomApiResponseProperties, true, true);

            UpdateAllComponents(service, context.CustomApis);
            UpdateAllComponents(service, context.CustomApiRequestParameters);
            UpdateAllComponents(service, context.CustomApiResponseProperties);
        }

        private static void UpdateAllComponents<T>(IRegistrationService service,
                                                   IEnumerable<T> components) where T : ISolutionComponent
        {
            var updateComponents = components
                .Where(x => x.RegistrationState == RegistrationState.ToUpdate)
                .ToList();
            foreach (var component in updateComponents)
            {
                var registeringComponent = component.ToRegisterComponent(_solutionContext);

                service.Update(registeringComponent);
            }
        }

        public static void CreateAllComponents<T>(IRegistrationService service,
                                                  IEnumerable<T> components,
                                                  bool doAddToSolution = false,
                                                  bool doFetchCode = false) where T : ISolutionComponent
        {
            var createComponents = components
                .Where(x => x.RegistrationState == RegistrationState.ToCreate)
                .ToList();

            if (!createComponents.Any()) return;
            int? entityTypeCode = doFetchCode ? service?.GetIntEntityTypeCode(createComponents.FirstOrDefault()?.EntityTypeName) : null;
            foreach (var component in createComponents)
            {
                var registeringComponent = component.ToRegisterComponent(_solutionContext);
                component.Id = service.Create(registeringComponent);
                registeringComponent.Id = component.Id;
                if (doAddToSolution)
                {
                    AddSolutionComponentToSolution(service, _solutionContext.SolutionName, registeringComponent.ToEntityReference(), entityTypeCode);
                }
            }
        }

        private static void DeleteAllComponents<T>(IRegistrationService service,
                                                   IEnumerable<T> components) where T : ISolutionComponent
        {
            var deleteComponents = components
                .Where(x => x.RegistrationState == RegistrationState.ToDelete)
                .ToList();
            foreach (var component in deleteComponents)
            {
                service.Delete(component.EntityTypeName, component.Id);
            }
        }


        private static void AddSolutionComponentToSolution(IRegistrationService service, string solutionUniqueName, EntityReference objectRef, int? objectTypeCode = null)
        {
            if (GetRegisteredSolutionComponent(objectRef) == null)
            {
                service.AddSolutionComponentToSolution(solutionUniqueName, objectRef, objectTypeCode);
            }
        }
        private static SolutionComponent GetRegisteredSolutionComponent(EntityReference objectRef)
        {
            return _solutionContext.Components.FirstOrDefault(c => c.ObjectId.Equals(objectRef.Id));
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


        public static void RegisterPlugins(IRegistrationService service, IAssemblyContext assembly)
        {
            foreach (var plugin in assembly.Plugins)
            {
                Console.WriteLine($@"  - {plugin.FullName}");
                switch (plugin.RegistrationState)
                {
                    case RegistrationState.Ignore:
                        foreach (var step in plugin.Steps)
                        {
                            RegisterStep(service, step);
                        }
                        break;
                    case RegistrationState.ToCreate:
                        var registeringPluginType = AssemblyBridge.ToRegisterPluginType(plugin);
                        plugin.Id = service.Create(registeringPluginType);
                        foreach (var step in plugin.Steps)
                        {
                            step.ParentId = plugin.Id;
                            RegisterStep(service, step);
                        }
                        break;
                    case RegistrationState.ToDelete:
                        foreach (var step in plugin.Steps)
                        {
                            RegisterStep(service, step);
                        }
                        break;
                }
            }
        }

        public static void RegisterStep(IRegistrationService service, Step step)
        {
            switch (step.RegistrationState)
            {
                case RegistrationState.Ignore:
                    RegisterStepImage(service, step.PreImage);
                    RegisterStepImage(service, step.PostImage);
                    break;
                case RegistrationState.ToCreate:
                    Console.WriteLine($"\t\t Creating {step.Stage} {step.Message} of {step.EntityTypeName} ({step.MethodsDisplayName})");

                    step.Id = service.Create(AssemblyBridge.ToRegisterStep(step, _solutionContext));
                    step.PreImage.ParentId = step.Id;
                    step.PostImage.ParentId = step.Id;

                    RegisterStepImage(service, step.PreImage);
                    RegisterStepImage(service, step.PostImage);

                    AddSolutionComponentToSolution(service, _solutionContext.SolutionName,
                                   new EntityReference(SdkMessageProcessingStepDefinition.EntityName, step.Id));
                    break;
                case RegistrationState.ToUpdate:
                    Console.WriteLine($"\t\t Updating {step.Stage} {step.Message} of {step.EntityTypeName} ({step.MethodsDisplayName})");
                    service.Update(AssemblyBridge.ToRegisterStep(step, _solutionContext));

                    RegisterStepImage(service, step.PreImage);
                    RegisterStepImage(service, step.PostImage);

                    break;
                case RegistrationState.ToDelete:
                    break;
            }
        }

        public static void RegisterStepImage(IRegistrationService service, StepImage image)
        {
            switch (image.RegistrationState)
            {
                case RegistrationState.Ignore:
                    return;
                case RegistrationState.ToCreate:
                    image.Id = service.Create(AssemblyBridge.ToRegisterImage(image));
                    break;
                case RegistrationState.ToUpdate:
                    service.Update(AssemblyBridge.ToRegisterImage(image));
                    break;
                default:
                    return;
            }
        }

        public static void RegisterWorkflows(IRegistrationService service, IAssemblyContext localAssembly, IAssemblyContext registeredAssembly)
        {
            var registeredPlugins = registeredAssembly.Plugins;
            foreach (var customWf in localAssembly.Plugins.Where(p => p.IsWorkflow))
            {
                var registeredPluginType = registeredPlugins.FirstOrDefault(p => p.FullName == customWf.FullName);

                Console.WriteLine($@"  - {customWf.FullName}");

                var registeringPluginType = AssemblyBridge.ToRegisterCustomWorkflowType(registeredAssembly.Assembly.Id, customWf.FullName, customWf.DisplayName);

                if (registeredPluginType == null)
                {
                    registeredPluginType.Id = service.Create(registeringPluginType);
                }
                if (registeredPluginType.FullName != customWf.DisplayName)
                {
                    service.Update(registeringPluginType);
                }
            }
        }

        public static void RegisterCustomApis(IRegistrationService service, IAssemblyContext localAssembly, IAssemblyContext registeredAssembly)
        {
            var customApiEntityTypeCode = service.GetIntEntityTypeCode(CustomApiDefinition.EntityName);
            var customApiParameterEntityTypeCode = service.GetIntEntityTypeCode(CustomApiRequestParameterDefinition.EntityName);
            var customApiResponseEntityTypeCode = service.GetIntEntityTypeCode(CustomApiResponsePropertyDefinition.EntityName);

            var registeredPluginTypes = registeredAssembly.Plugins;
            var registeredCustomApis = registeredAssembly.CustomApis;

            foreach (var customApi in localAssembly.CustomApis)
            {
                Console.WriteLine($@"  - {customApi.FullName}");

                var registeredPluginType = registeredPluginTypes.FirstOrDefault(p => p.FullName == customApi.FullName);
                var registeringPlugin = AssemblyBridge.ToRegisterPluginType(registeredAssembly.Assembly.Id, customApi.FullName);

                if (registeredPluginType == null)
                {
                    registeredPluginType.Id = service.Create(registeringPlugin);
                }

                customApi.PluginTypeId = new EntityReference(PluginTypeDefinition.EntityName, registeredPluginType.Id);

                var existingCustomApi = registeredCustomApis.FirstOrDefault(c => c.UniqueName == customApi.UniqueName);

                if (existingCustomApi == null)
                {
                    existingCustomApi = customApi;
                    existingCustomApi.Id = service.Create(customApi);
                    customApi.Id = existingCustomApi.Id;
                }
                else
                {
                    customApi.Id = existingCustomApi.Id;
                    service.Update(customApi);
                }

                foreach (var customApiRequestParameter in customApi.InArguments)
                {
                    UpdateCustomApiComponent(service, existingCustomApi, customApiRequestParameter);

                    AddSolutionComponentToSolution(service, _solutionContext.SolutionName, customApiRequestParameter.ToEntityReference(), customApiParameterEntityTypeCode);
                }

                foreach (var customApiResponseProperty in customApi.OutArguments)
                {
                    UpdateCustomApiComponent(service, existingCustomApi, customApiResponseProperty);

                    AddSolutionComponentToSolution(service, _solutionContext.SolutionName, customApiResponseProperty.ToEntityReference(), customApiResponseEntityTypeCode);
                }

                AddSolutionComponentToSolution(service, _solutionContext.SolutionName, customApi.ToEntityReference(), customApiEntityTypeCode);
            }
        }

        private static void UpdateCustomApiComponent<T>(IRegistrationService service, CustomApi existingCustomApi,
                                             T customApiComponent)
    where T : Entity, ICustomApiComponent
        {
            ICustomApiComponent existingComponent = existingCustomApi.InArguments.FirstOrDefault(p => p.UniqueName == customApiComponent.UniqueName);

            if (existingComponent == null)
            {
                existingComponent = existingCustomApi.OutArguments.FirstOrDefault(p => p.UniqueName == customApiComponent.UniqueName);
            }

            customApiComponent.CustomApiId = new EntityReference(CustomApiDefinition.EntityName, existingCustomApi.Id);

            if (existingComponent == null)
            {
                customApiComponent.Id = service.Create(customApiComponent);
            }
            else
            {
                customApiComponent.Id = existingComponent.Id;
                service.Update(customApiComponent);
            }
        }


        public static void UpdateStepImages(IRegistrationService service, Step convertedStep, Step registeredStep, PluginImageType imageType)
        {
            StepImage image;

            bool doRegisterImage;
            switch (imageType)
            {
                case PluginImageType.PostImage:
                    doRegisterImage = convertedStep.PostImage.IsUsed && convertedStep.Message != Messages.Delete.ToString();
                    image = convertedStep.PostImage;
                    break;

                case PluginImageType.PreImage:
                    doRegisterImage = convertedStep.PreImage.IsUsed;
                    image = convertedStep.PreImage;
                    break;

                default:
                    throw new InvalidEnumArgumentException("Unknown Enum");
            }

            if (doRegisterImage)
            {

                var registeringImage = AssemblyBridge.ToRegisterImage(image); ;
                if ((imageType == PluginImageType.PreImage && registeredStep.PreImage.IsUsed == false)
                  || (imageType == PluginImageType.PostImage && registeredStep.PostImage.IsUsed == false))
                {
                    registeringImage.Id = service.Create(registeringImage);
                }
                else if (registeredStep.PostImage.JoinedAttributes != convertedStep.PostImage.JoinedAttributes)
                {
                    registeringImage.Attributes1 = convertedStep.PostImage.JoinedAttributes;
                    service.Update(registeringImage);
                }
            }
            else if ((imageType == PluginImageType.PreImage && registeredStep.PreImage.IsUsed == true))
            {
                service.Delete(SdkMessageProcessingStepImageDefinition.EntityName, registeredStep.PreImage.Id);
            }
            else if ((imageType == PluginImageType.PostImage && registeredStep.PostImage.IsUsed == true))
            {
                service.Delete(SdkMessageProcessingStepImageDefinition.EntityName, registeredStep.PostImage.Id);

            }
        }

    }
}