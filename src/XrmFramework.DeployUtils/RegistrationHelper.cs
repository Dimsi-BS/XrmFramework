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

            var pluginSolutionUniqueName = projectConfig.TargetSolution;

            var connectionString = ConfigurationManager.ConnectionStrings[xrmFrameworkConfigSection.SelectedConnection].ConnectionString;

            Console.WriteLine($"You are about to deploy on {connectionString} organization. If ok press any key.");
            Console.ReadKey();
            Console.WriteLine("Connecting to CRM...");

            RegistrationService.MaxConnectionTimeout = TimeSpan.FromMinutes(10);

            var service = new RegistrationService(connectionString);

            service.OrganizationServiceProxy?.EnableProxyTypes();

            _registrationContext = new RegistrationContext(pluginSolutionUniqueName);
            _registrationContext.InitMetadata(service);

            var pluginAssembly = typeof(TPlugin).Assembly;

            var pluginType = pluginAssembly.GetType("XrmFramework.Plugin");
            var customApiType = pluginAssembly.GetType("XrmFramework.CustomApi");
            var workflowType = pluginAssembly.GetType("XrmFramework.Workflow.CustomWorkflowActivity");

            var pluginTypes = pluginAssembly.GetTypes()
                                            .Where(t => pluginType.IsAssignableFrom(t)
                                                     && !customApiType.IsAssignableFrom(t)
                                                     && t.IsPublic
                                                     && !t.IsAbstract)
                                            .ToList();

            var workflowTypes = pluginAssembly.GetTypes()
                                              .Where(t => workflowType.IsAssignableFrom(t)
                                                       && !t.IsAbstract
                                                       && t.IsPublic)
                                              .ToList();

            var customApiTypes = pluginAssembly.GetTypes()
                                               .Where(t => customApiType.IsAssignableFrom(t)
                                                        && t.IsPublic
                                                        && !t.IsAbstract)
                                               .ToList();

            var pluginList = CreateInstanceOfTypeList<Plugin>(pluginTypes, PluginRegistrationType.Plugin);

            var workflowList = CreateInstanceOfTypeList<Plugin>(workflowTypes, PluginRegistrationType.Workflow);

            pluginList.AddRange(workflowList);

            var customApisList = CreateInstanceOfTypeList<CustomApi>(customApiTypes, PluginRegistrationType.CustomApi);

            var assembly = service.GetAssemblyByName(pluginAssembly.GetName().Name);

            var profilerAssembly = service.GetProfilerAssembly();

            var registeredPluginTypes = new List<PluginType>();
            var registeredCustomApis = new List<CustomApi>();
            var registeredCustomApiRequestParameters = new List<CustomApiRequestParameter>();
            var registeredCustomApiResponseProperties = new List<CustomApiResponseProperty>();
            var profiledSteps = new List<SdkMessageProcessingStep>();
            ICollection<SdkMessageProcessingStep> registeredSteps = Enumerable.Empty<SdkMessageProcessingStep>().ToList();

            var assemblyPath = pluginAssembly.Location;

            if (assembly == null)
            {
                Console.WriteLine("Registering assembly");

                assembly = GetAssemblyToRegister(pluginAssembly, assemblyPath);

                assembly.Id = service.Create(assembly);
            }
            else
            {
                Console.WriteLine("Updating plugin assembly");

                var updatedAssembly = new Entity(PluginAssemblyDefinition.EntityName)
                {
                    Id = assembly.Id,
                    [PluginAssemblyDefinition.Columns.Content] = Convert.ToBase64String(File.ReadAllBytes(assemblyPath))
                };

                registeredPluginTypes = service.GetRegisteredPluginTypes(assembly.Id).ToList();
                registeredCustomApis = service.GetRegisteredCustomApis(assembly.Id).ToList();
                registeredSteps = service.GetRegisteredSteps(assembly.Id);

                if (profilerAssembly != null)
                {
                    profiledSteps = service.GetRegisteredSteps(profilerAssembly.Id).ToList();
                }

                // Delete components that are not in the project anymore
                // Find the types registered on the CRM which are not on the local project
                var registeredTypesToDelete = FilterUnusedPluginsForLocal(registeredPluginTypes, pluginList, customApisList).ToList();

                // Get all the steps related to those plugins
                var registeredStepsForPluginTypeToDelete = registeredSteps
                    .Where(s => registeredTypesToDelete.Any(t => s.EventHandler.Id == t.Id))
                    .ToList();

                // Same for CustomApis
                var registeredCustomApisToDelete = registeredCustomApis
                    .Where(c => registeredTypesToDelete.Any(s => c.PluginTypeId?.Id == s.Id))
                    .ToList();

                // Delete
                registeredStepsForPluginTypeToDelete.ForEach(s =>
                {
                    service.Delete(SdkMessageProcessingStepDefinition.EntityName, s.Id);
                    registeredSteps.Remove(s);
                });

                registeredCustomApisToDelete.ForEach(c =>
                {
                    service.Delete(CustomApiDefinition.EntityName, c.Id);
                });

                registeredTypesToDelete.ForEach(t =>
                {
                    service.Delete(PluginTypeDefinition.EntityName, t.Id);
                });

                service.Update(updatedAssembly);

                registeredCustomApiRequestParameters = service.GetRegisteredCustomApiRequestParameters(assembly.Id).ToList();
                registeredCustomApiResponseProperties = service.GetRegisteredCustomApiResponseProperties(assembly.Id).ToList();
            }

            AddSolutionComponentToSolution(service, pluginSolutionUniqueName, assembly.ToEntityReference());

            var registeredImages = service.GetRegisteredImages(assembly.Id);

            Console.WriteLine();
            Console.WriteLine(@"Registering Plugins");

            foreach (var plugin in pluginList.Where(p => !p.IsWorkflow))
            {
                Console.WriteLine($@"  - {plugin.FullName}");

                var registeredPluginType = registeredPluginTypes.FirstOrDefault(p => p.Name == plugin.FullName);

                if (registeredPluginType == null)
                {
                    registeredPluginType = GetPluginTypeToRegister(assembly.Id, plugin.FullName);
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
                        registeredSteps.Remove(registeredStep);
                        stepToRegister.Id = registeredStep.Id;
                        UpdateMessageStepIfNeeded(service, stepToRegister, registeredStep, profiledSteps);
                    }

                    AddSolutionComponentToSolution(service, pluginSolutionUniqueName, stepToRegister.ToEntityReference());

                    if (convertedStep.Message != Messages.Associate.ToString()
                     && convertedStep.Message != Messages.Lose.ToString()
                     && convertedStep.Message != Messages.Win.ToString())
                    {
                        UpdateStepImage(service, registeredImages, stepToRegister, convertedStep, PluginImageType.PostImage);
                        UpdateStepImage(service, registeredImages, stepToRegister, convertedStep, PluginImageType.PreImage);
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine(@"Registering Custom Workflow activities");

            foreach (var customWf in pluginList.Where(p => p.IsWorkflow))
            {
                var registeredPluginType = registeredPluginTypes.FirstOrDefault(p => p.TypeName == customWf.FullName);

                Console.WriteLine($@"  - {customWf.FullName}");

                if (registeredPluginType == null)
                {
                    registeredPluginType = GetCustomWorkflowTypeToRegister(assembly.Id, customWf.FullName, customWf.DisplayName);
                    registeredPluginType.Id = service.Create(registeredPluginType);
                }
                if (registeredPluginType.Name != customWf.DisplayName)
                {
                    registeredPluginType.Name = customWf.DisplayName;
                    service.Update(registeredPluginType);
                }
            }

            Console.WriteLine();
            Console.WriteLine(@"Registering Custom Apis");

            var customApiEntityTypeCode = GetEntityTypeCode(CustomApiDefinition.EntityName, service);
            var customApiParameterEntityTypeCode = GetEntityTypeCode(CustomApiRequestParameterDefinition.EntityName, service);
            var customApiResponseEntityTypeCode = GetEntityTypeCode(CustomApiResponsePropertyDefinition.EntityName, service);

            foreach (var customApi in customApisList)
            {
                Console.WriteLine($@"  - {customApi.FullName}");

                var registeredPluginType = registeredPluginTypes.FirstOrDefault(p => p.Name == customApi.FullName);

                if (registeredPluginType == null)
                {
                    registeredPluginType = GetPluginTypeToRegister(assembly.Id, customApi.FullName);
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

                    AddSolutionComponentToSolution(service, pluginSolutionUniqueName, customApiRequestParameter.ToEntityReference(), customApiParameterEntityTypeCode);
                }

                foreach (var customApiResponseProperty in customApi.OutArguments)
                {
                    UpdateCustomApiComponent(service, existingCustomApi, customApiResponseProperty, registeredCustomApiResponseProperties);

                    AddSolutionComponentToSolution(service, pluginSolutionUniqueName, customApiResponseProperty.ToEntityReference(), customApiResponseEntityTypeCode);
                }

                AddSolutionComponentToSolution(service, pluginSolutionUniqueName, customApi.ToEntityReference(),
                    customApiEntityTypeCode);
            }

            if (registeredCustomApiRequestParameters.Any() || registeredCustomApiResponseProperties.Any())
            {
                Console.WriteLine("Deleting unnecessary request parameters and response properties");

                foreach (var parameterToRemove in registeredCustomApiRequestParameters)
                {
                    service.Delete(CustomApiRequestParameterDefinition.EntityName, parameterToRemove.Id);
                }

                foreach (var responseToRemove in registeredCustomApiResponseProperties)
                {
                    service.Delete(CustomApiResponsePropertyDefinition.EntityName, responseToRemove.Id);
                }
            }

            Console.WriteLine();

            foreach (var step in registeredSteps)
            {
                service.Delete(SdkMessageProcessingStepDefinition.EntityName, step.Id);
            }
        }

        private static void AddSolutionComponentToSolution(IRegistrationService service, string solutionUniqueName, EntityReference objectRef, int? objectTypeCode = null)
        {
            if (GetRegisteredSolutionComponent(objectRef) == null)
            {
                var s = new AddSolutionComponentRequest
                {
                    AddRequiredComponents = false,
                    ComponentId = objectRef.Id,
                    SolutionUniqueName = solutionUniqueName
                };

                if (objectTypeCode.HasValue)
                {
                    s.ComponentType = objectTypeCode.Value;
                }
                else
                {
                    switch (objectRef.LogicalName)
                    {
                        case PluginAssemblyDefinition.EntityName:
                            s.ComponentType = (int)componenttype.PluginAssembly;
                            break;

                        case PluginTypeDefinition.EntityName:
                            s.ComponentType = (int)componenttype.PluginType;
                            break;

                        case SdkMessageProcessingStepDefinition.EntityName:
                            s.ComponentType = (int)componenttype.SDKMessageProcessingStep;
                            break;

                        case SdkMessageProcessingStepImageDefinition.EntityName:
                            s.ComponentType = (int)componenttype.SDKMessageProcessingStepImage;
                            break;
                    }
                }

                service.Execute(s);
            }
        }

        private static PluginAssembly GetAssemblyToRegister(Assembly a, string assemblyPath)
        {
            var fullNameSplit = a.FullName.Split(',');

            var name = fullNameSplit[0];
            var version = fullNameSplit[1].Substring(fullNameSplit[1].IndexOf('=') + 1);
            var culture = fullNameSplit[2].Substring(fullNameSplit[2].IndexOf('=') + 1);
            var publicKeyToken = fullNameSplit[3].Substring(fullNameSplit[3].IndexOf('=') + 1);
            var description = string.Format("{0} plugin assembly", name);

            var t = new PluginAssembly()
            {
                Name = name,
                SourceType = new OptionSetValue((int)pluginassembly_sourcetype.Database),
                IsolationMode = new OptionSetValue((int)pluginassembly_isolationmode.Sandbox),
                Culture = culture,
                PublicKeyToken = publicKeyToken,
                Version = version,
                Description = description,
                Content = Convert.ToBase64String(File.ReadAllBytes(assemblyPath))
            };

            return t;
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

        private static int GetEntityTypeCode(string logicalName, IRegistrationService service)
        {
            var entityRequest = new RetrieveEntityRequest { LogicalName = logicalName };

            var entityResponse = (RetrieveEntityResponse)service.Execute(entityRequest);

            return entityResponse.EntityMetadata.ObjectTypeCode.GetValueOrDefault();
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

        private static SolutionComponent GetRegisteredSolutionComponent(EntityReference objectRef)
        {
            return _registrationContext.Components.FirstOrDefault(c => c.ObjectId.Equals(objectRef.Id));
        }

        private static SdkMessageProcessingStep GetStepToRegister(Guid pluginTypeId, Model.Step step)
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
                                                     T customApiComponent, List<T> registeredCustomApiComponents)
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
            registeredCustomApiComponents.Remove(existingComponent);
        }

        public static dynamic CreateInstanceOfType(Type type, PluginRegistrationType kind)
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


        public static List<T> CreateInstanceOfTypeList<T>(List<Type> types, PluginRegistrationType kind)
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

        public static IEnumerable<PluginType> FilterUnusedPluginsForLocal(List<PluginType> registeredTypesList,
                                                                   List<Plugin> localPluginList,
                                                                   List<CustomApi> localCustomApiList)
        {
            return registeredTypesList.Where(r => localPluginList.All(p => p.FullName != r.Name)
                                               && localPluginList.Where(p => p.IsWorkflow).All(c => c.FullName != r.TypeName)
                                               && localCustomApiList.All(c => c.FullName != r.TypeName));
        }

        public static void UpdateMessageStepIfNeeded(IRegistrationService service, SdkMessageProcessingStep stepToRegister,
                                          SdkMessageProcessingStep registeredStep, List<SdkMessageProcessingStep> profiledSteps)
        {
            var comparer = new SdkMessageStepComparer();
            if (comparer.NeedsUpdate(stepToRegister, registeredStep))
            {
                var profiledStep = profiledSteps.FirstOrDefault(p => p.Name.StartsWith(registeredStep.Name));

                if (profiledStep != null)
                {
                    service.Delete(SdkMessageProcessingStepDefinition.EntityName, profiledStep.Id);

                    service.Execute(new SetStateRequest
                    {
                        EntityMoniker = registeredStep.ToEntityReference(),
                        State = new OptionSetValue((int)SdkMessageProcessingStepState.Enabled),
                        Status = new OptionSetValue(-1)
                    });
                }

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
    }
}