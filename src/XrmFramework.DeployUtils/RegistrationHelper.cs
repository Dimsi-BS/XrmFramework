// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Navigation;
using Deploy;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using XrmFramework.Definitions;
using XrmFramework.DeployUtils.Comparers;
using XrmFramework.DeployUtils.Configuration;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils
{
    public static class RegistrationHelper
    {
        private static List<PluginAssembly> _list = new List<PluginAssembly>();

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

            CrmServiceClient.MaxConnectionTimeout = TimeSpan.FromMinutes(10);

            var service = new CrmServiceClient(connectionString);

            service.OrganizationServiceProxy?.EnableProxyTypes();

            InitMetadata(service, pluginSolutionUniqueName);

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



            var assembly = GetAssemblyByName(service, pluginAssembly.GetName().Name);

            var profilerAssembly = GetProfilerAssembly(service);

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

                registeredPluginTypes = GetRegisteredPluginTypes(service, assembly.Id).ToList();
                registeredCustomApis = GetRegisteredCustomApis(service, assembly.Id).ToList();
                registeredSteps = GetRegisteredSteps(service, assembly.Id);

                if (profilerAssembly != null)
                {
                    profiledSteps = GetRegisteredSteps(service, profilerAssembly.Id).ToList();
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

                registeredCustomApiRequestParameters = GetRegisteredCustomApiRequestParameters(service, assembly.Id).ToList();
                registeredCustomApiResponseProperties = GetRegisteredCustomApiResponseProperties(service, assembly.Id).ToList();
            }


            AddSolutionComponentToSolution(service, pluginSolutionUniqueName, assembly.ToEntityReference());

            var registeredImages = GetRegisteredImages(service, assembly.Id);

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
                    var updatedRequestParameter = UpdateCustomApiComponent(service, existingCustomApi, customApiRequestParameter, registeredCustomApiRequestParameters);
                    
                    registeredCustomApiRequestParameters.Remove(updatedRequestParameter);
                    AddSolutionComponentToSolution(service, pluginSolutionUniqueName, customApiRequestParameter.ToEntityReference(),customApiParameterEntityTypeCode);
                }

                foreach (var customApiResponseProperty in customApi.OutArguments)
                {
                    var updatedResponseProperty = UpdateCustomApiComponent(service, existingCustomApi, customApiResponseProperty, registeredCustomApiResponseProperties);

                    registeredCustomApiResponseProperties.Remove(updatedResponseProperty);

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

        private static int GetEntityTypeCode(string logicalName, IOrganizationService service)
        {
            var entityRequest = new RetrieveEntityRequest { LogicalName = logicalName };

            var entityResponse = (RetrieveEntityResponse)service.Execute(entityRequest);

            return entityResponse.EntityMetadata.ObjectTypeCode.GetValueOrDefault();
        }

        private static PluginAssembly GetProfilerAssembly(IOrganizationService service)
        {
            var assemblies = GetAssemblies(service);

            return assemblies.FirstOrDefault(a => a.Name.Contains("PluginProfiler"));
        }

        private static IEnumerable<PluginType> GetRegisteredPluginTypes(IOrganizationService service, Guid pluginAssemblyId)
        {
            var list = new List<PluginType>();

            var query = new QueryExpression(PluginTypeDefinition.EntityName);
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition(PluginTypeDefinition.Columns.PluginAssemblyId, ConditionOperator.Equal, pluginAssemblyId);

            var result = RetrieveAll(service, query);
            foreach (var type in result)
            {
                list.Add(type.ToEntity<PluginType>());
            }

            return list;
        }

        private static IEnumerable<CustomApi> GetRegisteredCustomApis(IOrganizationService service, Guid assemblyId)
        {
            var list = new List<CustomApi>();

            var query = new QueryExpression(CustomApiDefinition.EntityName);
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition(CustomApiDefinition.Columns.IsManaged, ConditionOperator.Equal, false);
            var linkPluginType = query.AddLink(PluginTypeDefinition.EntityName,
                                               CustomApiDefinition.Columns.PluginTypeId,
                                               PluginTypeDefinition.Columns.Id);
            linkPluginType.LinkCriteria.AddCondition(PluginTypeDefinition.Columns.PluginAssemblyId, ConditionOperator.Equal, assemblyId);

            var result = RetrieveAll(service, query);
            foreach (var type in result)
            {
                list.Add(type.ToEntity<CustomApi>());
            }

            return list;
        }

        private static IEnumerable<CustomApiRequestParameter> GetRegisteredCustomApiRequestParameters(IOrganizationService service, Guid assemblyId)
        {
            var list = new List<CustomApiRequestParameter>();

            var query = new QueryExpression(CustomApiRequestParameterDefinition.EntityName);
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition(CustomApiRequestParameterDefinition.Columns.IsManaged, ConditionOperator.Equal, false);

            var linkCustomApi = query.AddLink(CustomApiDefinition.EntityName,
                                              CustomApiRequestParameterDefinition.Columns.CustomAPIId,
                                              CustomApiDefinition.Columns.Id);
            var linkPluginType = linkCustomApi.AddLink(PluginTypeDefinition.EntityName,
                                                       CustomApiDefinition.Columns.PluginTypeId,
                                                       PluginTypeDefinition.Columns.Id);
            linkPluginType.LinkCriteria.AddCondition(PluginTypeDefinition.Columns.PluginAssemblyId, ConditionOperator.Equal, assemblyId);

            var result = RetrieveAll(service, query);
            foreach (var type in result)
            {
                list.Add(type.ToEntity<CustomApiRequestParameter>());
            }

            return list;
        }

        private static IEnumerable<CustomApiResponseProperty> GetRegisteredCustomApiResponseProperties(IOrganizationService service, Guid assemblyId)
        {
            var list = new List<CustomApiResponseProperty>();

            var query = new QueryExpression(CustomApiResponsePropertyDefinition.EntityName);
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition(CustomApiResponsePropertyDefinition.Columns.IsManaged, ConditionOperator.Equal, false);

            var linkCustomApi = query.AddLink(CustomApiDefinition.EntityName,
                                              CustomApiResponsePropertyDefinition.Columns.CustomAPIId,
                                              CustomApiDefinition.Columns.Id);
            var linkPluginType = linkCustomApi.AddLink(PluginTypeDefinition.EntityName,
                                                       CustomApiDefinition.Columns.PluginTypeId,
                                                       PluginTypeDefinition.Columns.Id);
            linkPluginType.LinkCriteria.AddCondition(PluginTypeDefinition.Columns.PluginAssemblyId, ConditionOperator.Equal, assemblyId);

            var result = RetrieveAll(service, query);
            foreach (var type in result)
            {
                list.Add(type.ToEntity<CustomApiResponseProperty>());
            }

            return list;
        }

        private static ICollection<SdkMessageProcessingStep> GetRegisteredSteps(IOrganizationService service, Guid assemblyId)
        {
            var list = new List<SdkMessageProcessingStep>();

            var query = new QueryExpression(SdkMessageProcessingStepDefinition.EntityName);
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition(SdkMessageProcessingStepDefinition.Columns.Stage, ConditionOperator.NotEqual, 30);

            var linkPluginType = query.AddLink(PluginTypeDefinition.EntityName,
                                               SdkMessageProcessingStepDefinition.Columns.EventHandler,
                                               PluginTypeDefinition.Columns.Id);
            linkPluginType.LinkCriteria.AddCondition(PluginTypeDefinition.Columns.PluginAssemblyId, ConditionOperator.Equal, assemblyId);

            var result = RetrieveAll(service, query);
            foreach (var type in result)
            {
                list.Add(type.ToEntity<SdkMessageProcessingStep>());
            }

            return list;
        }

        private static ICollection<SdkMessageProcessingStepImage> GetRegisteredImages(IOrganizationService service, Guid assemblyId)
        {
            var list = new List<SdkMessageProcessingStepImage>();

            var query = new QueryExpression(SdkMessageProcessingStepImageDefinition.EntityName);
            query.ColumnSet.AllColumns = true;
            var stepLink = query.AddLink(SdkMessageProcessingStepDefinition.EntityName,
                                         SdkMessageProcessingStepImageDefinition.Columns.SdkMessageProcessingStepId,
                                         SdkMessageProcessingStepDefinition.Columns.Id);
            var linkPluginType = stepLink.AddLink(PluginTypeDefinition.EntityName,
                                                  SdkMessageProcessingStepDefinition.Columns.EventHandler,
                                                  PluginTypeDefinition.Columns.Id);
            linkPluginType.LinkCriteria.AddCondition(PluginTypeDefinition.Columns.PluginAssemblyId, ConditionOperator.Equal, assemblyId);

            var result = RetrieveAll(service, query);
            foreach (var type in result)
            {
                list.Add(type.ToEntity<SdkMessageProcessingStepImage>());
            }

            return list;
        }

        private static IEnumerable<PluginAssembly> GetAssemblies(IOrganizationService service)
        {
            if (_list.Count == 0)
            {
                var query = new QueryExpression(PluginAssemblyDefinition.EntityName);
                query.ColumnSet.AddColumns(PluginAssemblyDefinition.Columns.Id, PluginAssemblyDefinition.Columns.Name);
                query.Distinct = true;
                query.Criteria.FilterOperator = LogicalOperator.And;
                query.Criteria.AddCondition(PluginAssemblyDefinition.Columns.Name, ConditionOperator.NotLike, "CompiledWorkflow%");
                var filter = query.Criteria.AddFilter(LogicalOperator.Or);
                filter.AddCondition(PluginAssemblyDefinition.Columns.CustomizationLevel, ConditionOperator.Null);
                filter.AddCondition(PluginAssemblyDefinition.Columns.CustomizationLevel, ConditionOperator.NotEqual, 0);
                filter.AddCondition(PluginAssemblyDefinition.Columns.Name, ConditionOperator.In, "Microsoft.Crm.ObjectModel", "Microsoft.Crm.ServiceBus");

                var result = RetrieveAll(service, query);
                foreach (var assembly in result)
                {
                    _list.Add(assembly.ToEntity<PluginAssembly>());
                }
            }

            return _list;
        }

        private static PluginAssembly GetAssemblyByName(IOrganizationService service, string assemblyName)
        {
            var assemblies = GetAssemblies(service);

            return assemblies.FirstOrDefault(a => assemblyName == a.Name);
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

        private static SdkMessageProcessingStep GetStepToRegister(Guid pluginTypeId, Model.Step step)
        {
            // Issue with CRM SDK / Description field max length = 256 characters 
            var descriptionAttributeMaxLength = 256;
            var description = $"{step.PluginTypeName} : {step.Stage} {step.Message} of {step.EntityName} ({step.MethodsDisplayName})";
            description = description.Length <= descriptionAttributeMaxLength ? description : description.Substring(0, descriptionAttributeMaxLength - 4) + "...)";

            if (!string.IsNullOrEmpty(step.ImpersonationUsername))
            {
                var count = _users.Count(u => u.Key == step.ImpersonationUsername);

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
                ImpersonatingUserId = string.IsNullOrEmpty(step.ImpersonationUsername) ? null : new EntityReference(SystemUserDefinition.EntityName, _users.First(u => u.Key == step.ImpersonationUsername).Value),
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
                SdkMessageId = _messages[step.Message], //GetSdkMessageRef(service, step.Message),
                SdkMessageFilterId = _filters.Where(f => f.SdkMessageId.Name == step.Message && f.PrimaryObjectTypeCode == step.EntityName)
                                             .Select(f => f.ToEntityReference()).FirstOrDefault(), //GetSdkMessageFilterRef(service, step),
                //SdkMessageProcessingStepSecureConfigId = GetSdkMessageProcessingStepSecureConfigRef(service, step),
                Stage = new OptionSetValue((int)step.Stage),
                SupportedDeployment = new OptionSetValue((int)sdkmessageprocessingstep_supporteddeployment.ServerOnly),
                Configuration = step.UnsecureConfig
            };

            return t;
        }

        private static SdkMessageProcessingStepImage GetImageToRegister(IOrganizationService service, Guid stepId, Model.Step step, bool isPreImage)
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

        private static void AddSolutionComponentToSolution(IOrganizationService service, string solutionUniqueName, EntityReference objectRef, int? objectTypeCode = null)
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

        private static SolutionComponent GetRegisteredSolutionComponent(EntityReference objectRef)
        {
            return _components.FirstOrDefault(c => c.ObjectId.Equals(objectRef.Id));
        }

        private static void InitMetadata(IOrganizationService service, string solutionName)
        {
            Console.WriteLine("Metadata initialization");

            var sw = Stopwatch.StartNew();

            var query = new QueryExpression(SdkMessageFilterDefinition.EntityName);
            query.ColumnSet.AddColumns(SdkMessageFilterDefinition.Columns.Id,
                                       SdkMessageFilterDefinition.Columns.SdkMessageId,
                                       SdkMessageFilterDefinition.Columns.PrimaryObjectTypeCode);
            query.Criteria.AddCondition(SdkMessageFilterDefinition.Columns.IsCustomProcessingStepAllowed, ConditionOperator.Equal, true);
            query.Criteria.AddCondition(SdkMessageFilterDefinition.Columns.IsVisible, ConditionOperator.Equal, true);

            var filters = RetrieveAll(service, query);

            //Console.WriteLine($"Retrieved {filters.Count} message filters in {sw.Elapsed}");

            _filters.Clear();
            _filters.AddRange(filters.Select(f => f.ToEntity<SdkMessageFilter>()));

            sw.Restart();
            query = new QueryExpression(SdkMessageDefinition.EntityName);
            query.ColumnSet.AddColumns(SdkMessageDefinition.Columns.Id, SdkMessageDefinition.Columns.Name);

            var messages = RetrieveAll(service, query).Select(e => e.ToEntity<SdkMessage>());

            //Console.WriteLine($"Retrieved {messages.Count()} messages in {sw.Elapsed}");

            _messages.Clear();
            foreach (SdkMessage e in messages)
            {
                _messages.Add(e.Name, e.ToEntityReference());
            }

            sw.Restart();

            query = new QueryExpression(SolutionDefinition.EntityName);
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition(SolutionDefinition.Columns.UniqueName, ConditionOperator.Equal, solutionName);

            _solution = RetrieveAll(service, query).Select(s => s.ToEntity<Solution>()).FirstOrDefault();

            //Console.WriteLine($"Retrieved solution by name in {sw.Elapsed}");

            if (_solution == null)
            {
                Console.WriteLine("The solution {0} does not exist in the CRM, modify App.config to point to an existing solution.", solutionName);
                Console.WriteLine("\r\nAppuyez sur une touche pour arrêter.");
                Console.ReadKey();
                System.Environment.Exit(1);
            }
            else if (_solution.GetAttributeValue<bool>(SolutionDefinition.Columns.IsManaged))
            {
                Console.WriteLine("The solution {0} is managed in the CRM, modify App.config to point to a development environment.", solutionName);
                System.Environment.Exit(1);
            }
            else
            {
                _publisher = service.Retrieve(PublisherDefinition.EntityName, _solution.PublisherId.Id, new ColumnSet(true)).ToEntity<Publisher>();


                query = new QueryExpression(SolutionComponentDefinition.EntityName);
                query.ColumnSet.AllColumns = true;
                query.Criteria.AddCondition(SolutionComponentDefinition.Columns.SolutionId, ConditionOperator.Equal, _solution.Id);

                var components = RetrieveAll(service, query).Select(s => s.ToEntity<SolutionComponent>());

                _components.AddRange(components);
            }

            query = new QueryExpression(SystemUserDefinition.EntityName);
            query.ColumnSet.AddColumn(SystemUserDefinition.Columns.DomainName);
            query.Criteria.AddCondition(SystemUserDefinition.Columns.AccessMode, ConditionOperator.NotEqual, 3);
            query.Criteria.AddCondition(SystemUserDefinition.Columns.IsDisabled, ConditionOperator.Equal, false);

            foreach (var user in RetrieveAll(service, query))
            {
                _users.Add(new KeyValuePair<string, Guid>(user.GetAttributeValue<string>(SystemUserDefinition.Columns.DomainName), user.Id));
            }
            //Console.WriteLine($"Retrieved {_users.Count} users in {sw.Elapsed}");
        }

        public static IList<Entity> RetrieveAll(IOrganizationService service, QueryExpression query, bool cleanLinks = true)
        {
            if (!query.TopCount.HasValue)
            {
                query.PageInfo = new PagingInfo { Count = 5000, PageNumber = 1 };
            }

            var result = new List<Entity>();

            EntityCollection ec;

            do
            {
                ec = service.RetrieveMultiple(query);

                result.AddRange(ec.Entities);

                if (query.PageInfo != null)
                {
                    query.PageInfo.PageNumber++;
                    query.PageInfo.PagingCookie = ec.PagingCookie;
                }

            } while (ec.MoreRecords);

            return result;
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
                        list.Add(CustomApi.FromXrmFrameworkCustomApi(temp, _publisher.CustomizationPrefix));
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

        public static void UpdateMessageStepIfNeeded(IOrganizationService service, SdkMessageProcessingStep stepToRegister,
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

        public static void UpdateStepImage(IOrganizationService service, ICollection<SdkMessageProcessingStepImage> registeredImages,
                                           SdkMessageProcessingStep stepToRegister, Step convertedStep, PluginImageType imageType)
        {
            SdkMessageProcessingStepImage registeredImage;
            bool doRegisterImage;
            switch(imageType)
            {
                case PluginImageType.PostImage:
                    registeredImage = registeredImages.FirstOrDefault(i => i.Name == "PostImage"
                                                                        && i.SdkMessageProcessingStepId.Id == stepToRegister.Id);
                    doRegisterImage = convertedStep.PostImageUsed;
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
                    registeredImage = GetImageToRegister(service, stepToRegister.Id, convertedStep, imageType == PluginImageType.PreImage);
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

        private static T UpdateCustomApiComponent<T>(IOrganizationService service, CustomApi existingCustomApi,
                                                     T customApiComponent, IEnumerable<T> registeredCustomApiComponents) where T : Entity, ICustomApiComponent
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
            return existingComponent;
        }
        private static Solution _solution = null;
        private static Publisher _publisher = null;

        private static List<SolutionComponent> _components = new List<SolutionComponent>();
        private static List<SdkMessageFilter> _filters = new List<SdkMessageFilter>();
        private static readonly Dictionary<string, EntityReference> _messages = new Dictionary<string, EntityReference>();
        private static readonly List<KeyValuePair<string, Guid>> _users = new List<KeyValuePair<string, Guid>>();
    }
}