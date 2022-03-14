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
using XrmFramework.DeployUtils.Comparers;
using XrmFramework.DeployUtils.Configuration;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Service;

namespace XrmFramework.DeployUtils
{
    public static class RegistrationHelper
    {
        private static IRegistrationContext _registrationContext;

        public static void RegisterPluginsAndWorkflows<TPlugin>(string projectName)
        {
            string pluginSolutionUniqueName, connectionString;

            initConnection(projectName, out pluginSolutionUniqueName, out connectionString);

            Console.WriteLine($"You are about to deploy on {connectionString} organization. If ok press any key.");
            Console.ReadKey();
            Console.WriteLine("Connecting to CRM...");

            RegistrationService.MaxConnectionTimeout = TimeSpan.FromMinutes(10);

            var service = new RegistrationService(connectionString);

            service.OrganizationServiceProxy?.EnableProxyTypes();

            _registrationContext = new RegistrationContext(pluginSolutionUniqueName);
            _registrationContext.InitMetadata(service);

            var assemblyFactory = new AssemblyFactory(_registrationContext);

            var localAssembly = assemblyFactory.LocalAssemblyContext(typeof(TPlugin));

            var registeredAssembly = assemblyFactory.RegisteredAssemblyContext(service, localAssembly.Name);

            Console.WriteLine();
            Console.WriteLine("Registering assembly");

            RegisterAssembly(service, localAssembly, registeredAssembly);


            Console.WriteLine();
            Console.WriteLine(@"Registering Plugins");

            RegisterPlugins(service, localAssembly, registeredAssembly);

            Console.WriteLine();
            Console.WriteLine(@"Registering Custom Workflow activities");

            RegisterWorkflows(service, localAssembly, registeredAssembly);

            Console.WriteLine();
            Console.WriteLine(@"Registering Custom Apis");

            RegisterCustomApis(service, localAssembly, registeredAssembly);


            ////TODO: Figure out wtf this code is supposed to do and have the same result
            //foreach (var step in registeredSteps)
            //{
            //    service.Delete(SdkMessageProcessingStepDefinition.EntityName, step.Id);
            //}
        }

        public static void RegisterAssembly(RegistrationService service, ILocalAssemblyContext localAssembly, IRegisteredAssemblyContext registeredAssembly)
        {
            if (registeredAssembly.IsNull)
            {
                Console.WriteLine("Creating assembly");

                registeredAssembly.Assembly = localAssembly.ToPluginAssembly();

                registeredAssembly.Id = service.Create(registeredAssembly);
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine(@"Assembly Exists, Cleaning Assembly");

                CleanAssembly(service, localAssembly, registeredAssembly);

                Console.WriteLine("Updating plugin assembly");

                registeredAssembly.Assembly = localAssembly.ToPluginAssembly(registeredAssembly.Id);

                service.Update(registeredAssembly.Assembly);
            }
            AddSolutionComponentToSolution(service, _registrationContext.SolutionName, registeredAssembly.EntityReference) ;
        }

        public static void CleanAssembly(RegistrationService service, ILocalAssemblyContext localAssembly, IRegisteredAssemblyContext registeredAssembly)
        {
            // Delete components that are not in the project anymore
            // Find the types registered on the CRM which are not on the local project
            var registeredTypesToDelete = FilterUnusedPluginsForLocal(localAssembly, registeredAssembly).ToList();

            // Get all the steps related to those plugins
            var registeredStepsForPluginTypeToDelete = registeredAssembly.Steps
                .Where(s => registeredTypesToDelete.Any(t => s.EventHandler.Id == t.Id))
                .ToList();

            // Same for CustomApis
            var registeredCustomApisToDelete = registeredAssembly.CustomApis
                .Where(c => registeredTypesToDelete.Any(s => c.PluginTypeId?.Id == s.Id))
                .ToList();

            // Here goes CustomApiRequestParameters
            var requestParametersToDelete = registeredAssembly.CustomApiRequestParameters
                .Where(r => localAssembly.CustomApiRequestParameters.All(l => r.UniqueName != l.UniqueName))
                .ToList();
            var responseParametersToDelete = registeredAssembly.CustomApiResponseProperties
                .Where(r => localAssembly.CustomApiRequestParameters.All(l => r.UniqueName != l.UniqueName))
                .ToList();

            // Delete
            service.DeleteMany(registeredStepsForPluginTypeToDelete);

            service.DeleteMany(responseParametersToDelete);

            service.DeleteMany(requestParametersToDelete);

            service.DeleteMany(registeredCustomApisToDelete);

            service.DeleteMany(registeredTypesToDelete);

            //Remove from Fetched lists
            registeredAssembly.Steps = registeredAssembly.Steps
                .Where(s => !registeredStepsForPluginTypeToDelete.Contains(s));

            registeredAssembly.CustomApis = registeredAssembly.CustomApis
                .Where(s => !registeredCustomApisToDelete.Contains(s));

            registeredAssembly.CustomApiRequestParameters = registeredAssembly.CustomApiRequestParameters
                .Where(s => !requestParametersToDelete.Contains(s));

            registeredAssembly.CustomApiResponseProperties = registeredAssembly.CustomApiResponseProperties
                .Where(s => !responseParametersToDelete.Contains(s));

            registeredAssembly.PluginTypes = registeredAssembly.PluginTypes
                .Where(s => !registeredTypesToDelete.Contains(s));
        }

        public static void RegisterPlugins(RegistrationService service, ILocalAssemblyContext localAssembly, IRegisteredAssemblyContext registeredAssembly)
        {
            var registeredPluginTypes = registeredAssembly.PluginTypes;
            var registeredSteps = registeredAssembly.Steps;
            var registeredImages = registeredAssembly.ImageSteps;

            foreach (var plugin in localAssembly.Plugins.Where(p => !p.IsWorkflow))
            {
                Console.WriteLine($@"  - {plugin.FullName}");

                var registeredPluginType = registeredPluginTypes.FirstOrDefault(p => p.Name == plugin.FullName);

                if (registeredPluginType == null)
                {
                    registeredPluginType = GetPluginTypeToRegister(registeredAssembly.Id, plugin.FullName);
                    registeredPluginType.Id = service.Create(registeredPluginType);
                }

                var registeredStepsForPluginType = registeredSteps.Where(s => s.EventHandler.Id == registeredPluginType.Id).ToList();

                var comparer = new SdkMessageStepComparer();

                foreach (var convertedStep in plugin.Steps)
                {
                    var stepToRegister = GetStepToRegister(registeredPluginType.Id, convertedStep);

                    var registeredStep = registeredStepsForPluginType.FirstOrDefault(s => comparer.Equals(s, stepToRegister));

                    if (registeredStep == null)
                    {
                        stepToRegister.Id = service.Create(stepToRegister);
                    }
                    else
                    {
                        stepToRegister.Id = registeredStep.Id;
                        UpdateMessageStepIfNeeded(service, stepToRegister, registeredStep);
                    }

                    AddSolutionComponentToSolution(service, _registrationContext.SolutionName, stepToRegister.ToEntityReference());

                    if (convertedStep.Message != Messages.Associate.ToString()
                     && convertedStep.Message != Messages.Lose.ToString()
                     && convertedStep.Message != Messages.Win.ToString())
                    {
                        UpdateStepImage(service, registeredImages, stepToRegister, convertedStep, PluginImageType.PostImage);
                        UpdateStepImage(service, registeredImages, stepToRegister, convertedStep, PluginImageType.PreImage);
                    }
                }
            }
        }
        public static void RegisterWorkflows(RegistrationService service, ILocalAssemblyContext localAssembly, IRegisteredAssemblyContext registeredAssembly)
        {
            var registeredPluginTypes = registeredAssembly.PluginTypes;
            foreach (var customWf in localAssembly.Plugins.Where(p => p.IsWorkflow))
            {
                var registeredPluginType = registeredPluginTypes.FirstOrDefault(p => p.TypeName == customWf.FullName);

                Console.WriteLine($@"  - {customWf.FullName}");

                if (registeredPluginType == null)
                {
                    registeredPluginType = GetCustomWorkflowTypeToRegister(registeredAssembly.Id, customWf.FullName, customWf.DisplayName);
                    registeredPluginType.Id = service.Create(registeredPluginType);
                }
                if (registeredPluginType.Name != customWf.DisplayName)
                {
                    registeredPluginType.Name = customWf.DisplayName;
                    service.Update(registeredPluginType);
                }
            }
        }

        public static void RegisterCustomApis(RegistrationService service, ILocalAssemblyContext localAssembly, IRegisteredAssemblyContext registeredAssembly)
        {
            var customApiEntityTypeCode = service.GetIntEntityTypeCode(CustomApiDefinition.EntityName);
            var customApiParameterEntityTypeCode = service.GetIntEntityTypeCode(CustomApiRequestParameterDefinition.EntityName);
            var customApiResponseEntityTypeCode = service.GetIntEntityTypeCode(CustomApiResponsePropertyDefinition.EntityName);

            var registeredPluginTypes = registeredAssembly.PluginTypes;
            var registeredCustomApis = registeredAssembly.CustomApis;
            var registeredCustomApiRequestParameters = registeredAssembly.CustomApiRequestParameters;
            var registeredCustomApiResponseProperties = registeredAssembly.CustomApiResponseProperties;
            foreach (var customApi in localAssembly.CustomApis)
            {
                Console.WriteLine($@"  - {customApi.FullName}");

                var registeredPluginType = registeredPluginTypes.FirstOrDefault(p => p.Name == customApi.FullName);

                if (registeredPluginType == null)
                {
                    registeredPluginType = GetPluginTypeToRegister(registeredAssembly.Id, customApi.FullName);
                    registeredPluginType.Id = service.Create(registeredPluginType);
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
                    UpdateCustomApiComponent(service, existingCustomApi, customApiRequestParameter, registeredCustomApiRequestParameters);

                    AddSolutionComponentToSolution(service, _registrationContext.SolutionName, customApiRequestParameter.ToEntityReference(), customApiParameterEntityTypeCode);
                }

                foreach (var customApiResponseProperty in customApi.OutArguments)
                {
                    UpdateCustomApiComponent(service, existingCustomApi, customApiResponseProperty, registeredCustomApiResponseProperties);

                    AddSolutionComponentToSolution(service, _registrationContext.SolutionName, customApiResponseProperty.ToEntityReference(), customApiResponseEntityTypeCode);
                }

                AddSolutionComponentToSolution(service, _registrationContext.SolutionName, customApi.ToEntityReference(), customApiEntityTypeCode);
            }
        }

        private static PluginType GetCustomWorkflowTypeToRegister(Guid pluginAssemblyId, string pluginFullName, string displayName)
        {
            var t = new PluginType()
            {
                PluginAssemblyId = new EntityReference()
                {
                    LogicalName = PluginAssemblyDefinition.EntityName,
                    Id = pluginAssemblyId
                },
                TypeName = pluginFullName,
                FriendlyName = pluginFullName,
                Name = displayName,
                Description = string.Empty,
                WorkflowActivityGroupName = "Workflows"
            };

            return t;
        }

        private static SdkMessageProcessingStepImage GetImageToRegister(Guid stepId, Model.Step step, bool isPreImage)
        {
            var isAllColumns = isPreImage ? step.PreImageAllAttributes : step.PostImageAllAttributes;
            var columns = isPreImage ? step.JoinedPreImageAttributes : step.JoinedPostImageAttributes;
            var name = isPreImage ? "PreImage" : "PostImage";

            var messagePropertyName = "Target";

            if (step.Message == Model.Messages.Create.ToString() && !isPreImage)
            {
                messagePropertyName = "Id";
            }
#pragma warning disable 618
            else if (step.Message == Messages.SetState.ToString() || step.Message == Messages.SetStateDynamicEntity.ToString())
#pragma warning restore 618
            {
                messagePropertyName = "EntityMoniker";
            }

            var t = new SdkMessageProcessingStepImage()
            {
                Attributes1 = isAllColumns ? null : columns,
                EntityAlias = name,
                ImageType = new OptionSetValue(isPreImage ? (int)sdkmessageprocessingstepimage_imagetype.PreImage
                                                          : (int)sdkmessageprocessingstepimage_imagetype.PostImage),
                IsCustomizable = new BooleanManagedProperty(true),
                MessagePropertyName = messagePropertyName,
                Name = name,
                SdkMessageProcessingStepId = new EntityReference(SdkMessageProcessingStepDefinition.EntityName, stepId)
            };

            return t;
        }

        private static PluginType GetPluginTypeToRegister(Guid pluginAssemblyId, string pluginFullName)
        {
            var t = new PluginType()
            {
                PluginAssemblyId = new EntityReference()
                {
                    LogicalName = PluginAssemblyDefinition.EntityName,
                    Id = pluginAssemblyId
                },
                TypeName = pluginFullName,
                FriendlyName = pluginFullName,
                Name = pluginFullName,
                Description = pluginFullName
            };

            return t;
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
            return _registrationContext.Components.FirstOrDefault(c => c.ObjectId.Equals(objectRef.Id));
        }

        public static SdkMessageProcessingStep GetStepToRegister(Guid pluginTypeId, Model.Step step)
        {
            // Issue with CRM SDK / Description field max length = 256 characters
            var descriptionAttributeMaxLength = 256;
            var description = $"{step.PluginTypeName} : {step.Stage} {step.Message} of {step.EntityName} ({step.MethodsDisplayName})";
            description = description.Length <= descriptionAttributeMaxLength ? description : description.Substring(0, descriptionAttributeMaxLength - 4) + "...)";

            if (!string.IsNullOrEmpty(step.ImpersonationUsername))
            {
                var count = _registrationContext.Users.Count(u => u.Key == step.ImpersonationUsername);

                if (count == 0)
                {
                    throw new Exception($"{description} : No user have fullname '{step.ImpersonationUsername}' in CRM.");
                }
                if (count > 1)
                {
                    throw new Exception($"{description} : {count} users have the fullname '{step.ImpersonationUsername}' in CRM.");
                }
            }

            var t = new SdkMessageProcessingStep()
            {
                AsyncAutoDelete = step.Mode == Model.Modes.Asynchronous,
                Description = description,
                EventHandler = new EntityReference(PluginTypeDefinition.EntityName, pluginTypeId),
                FilteringAttributes = step.FilteringAttributes.Any() ? string.Join(",", step.FilteringAttributes) : null,
                ImpersonatingUserId = string.IsNullOrEmpty(step.ImpersonationUsername) ? null : new EntityReference(SystemUserDefinition.EntityName, _registrationContext.Users.First(u => u.Key == step.ImpersonationUsername).Value),
#pragma warning disable 0612
                InvocationSource = new OptionSetValue((int)sdkmessageprocessingstep_invocationsource.Child),
#pragma warning restore 0612
                IsCustomizable = new BooleanManagedProperty(true),
                IsHidden = new BooleanManagedProperty(false),
                Mode = new OptionSetValue((int)step.Mode),
                Name = description,
#pragma warning disable 0612
                PluginTypeId = new EntityReference(PluginTypeDefinition.EntityName, pluginTypeId),
#pragma warning restore 0612
                Rank = step.Order,
                SdkMessageId = _registrationContext.Messages[step.Message], //GetSdkMessageRef(service, step.Message),
                SdkMessageFilterId = _registrationContext.Filters.Where(f => f.SdkMessageId.Name == step.Message && f.PrimaryObjectTypeCode == step.EntityName)
                                             .Select(f => f.ToEntityReference()).FirstOrDefault(), //GetSdkMessageFilterRef(service, step),
                //SdkMessageProcessingStepSecureConfigId = GetSdkMessageProcessingStepSecureConfigRef(service, step),
                Stage = new OptionSetValue((int)step.Stage),
                SupportedDeployment = new OptionSetValue((int)sdkmessageprocessingstep_supporteddeployment.ServerOnly),
                Configuration = step.UnsecureConfig
            };

            return t;
        }

        private static void UpdateCustomApiComponent<T>(IRegistrationService service, CustomApi existingCustomApi,
                                                     T customApiComponent, IEnumerable<T> registeredCustomApiComponents)
            where T : Entity, ICustomApiComponent
        {
            var existingComponent = registeredCustomApiComponents.FirstOrDefault(p => p.UniqueName == customApiComponent.UniqueName
                                                                                   && p.CustomApiId.Id == existingCustomApi.Id);

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


        public static IEnumerable<PluginType> FilterUnusedPluginsForLocal(ILocalAssemblyContext local, IRegisteredAssemblyContext remote)
        {
            return remote.PluginTypes.Where(r => local.Plugins.All(p => p.FullName != r.Name)
                                              && local.Plugins.Where(p => p.IsWorkflow).All(c => c.FullName != r.TypeName)
                                              && local.CustomApis.All(c => c.FullName != r.TypeName));
        }

        public static void UpdateMessageStepIfNeeded(IRegistrationService service, SdkMessageProcessingStep stepToRegister,
                                          SdkMessageProcessingStep registeredStep)
        {
            var comparer = new SdkMessageStepComparer();
            if (comparer.NeedsUpdate(stepToRegister, registeredStep))
            {
                service.Update(stepToRegister);
            }
        }

        public static void UpdateStepImage(IRegistrationService service, ICollection<SdkMessageProcessingStepImage> registeredImages,
                                           SdkMessageProcessingStep stepToRegister, Step convertedStep, PluginImageType imageType)
        {
            SdkMessageProcessingStepImage registeredImage;
            bool doRegisterImage;
            switch (imageType)
            {
                case PluginImageType.PostImage:
                    registeredImage = registeredImages.FirstOrDefault(i => i.Name == "PostImage"
                                                                        && i.SdkMessageProcessingStepId.Id == stepToRegister.Id);
                    doRegisterImage = convertedStep.PostImageUsed && convertedStep.Message != Messages.Delete.ToString();
                    break;

                case PluginImageType.PreImage:
                    registeredImage = registeredImages.FirstOrDefault(i => i.Name == "PreImage"
                                                                        && i.SdkMessageProcessingStepId.Id == stepToRegister.Id);
                    doRegisterImage = convertedStep.PreImageUsed;
                    break;

                default:
                    throw new InvalidEnumArgumentException("Unknown Enum");
            }

            if (doRegisterImage)
            {
                if (registeredImage == null)
                {
                    registeredImage = GetImageToRegister(stepToRegister.Id, convertedStep, imageType == PluginImageType.PreImage);
                    registeredImage.Id = service.Create(registeredImage);
                }
                else if (registeredImage.Attributes1 != convertedStep.JoinedPostImageAttributes)
                {
                    registeredImage.Attributes1 = convertedStep.JoinedPostImageAttributes;
                    service.Update(registeredImage);
                }
            }
            else if (registeredImage != null)
            {
                service.Delete(registeredImage.LogicalName, registeredImage.Id);
            }
        }
    
        private static void initConnection(string projectName, out string pluginSolutionUniqueName, out string connectionString)
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