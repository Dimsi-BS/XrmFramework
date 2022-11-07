// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Deploy;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using XrmFramework.DeployUtils.Comparers;
using XrmFramework.DeployUtils.Configuration;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils
{
    public static class RegistrationHelper
    {
        private static readonly List<PluginAssembly> _list = new();

        public static void RegisterPluginsAndWorkflows<TPlugin>(string projectName, bool isOnPrem = false, params string[] args)
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

            if (args == null || !args.Select(a => a.ToLowerInvariant()).Contains("-noprompt"))
            {
                Console.WriteLine($"You are about to deploy on {connectionString} organization. If ok press any key.");
                Console.ReadKey();
            }

            if (args != null && args.Select(a => a.ToLowerInvariant()).Contains("-debug"))
            {
                Console.WriteLine($"ConnectionString : {connectionString}");
            }

            Console.WriteLine("Connecting to CRM...");

            CrmServiceClient.MaxConnectionTimeout = TimeSpan.FromMinutes(10);

            var service = new CrmServiceClient(connectionString);

            service.OrganizationServiceProxy?.EnableProxyTypes();

            InitMetadata(service, pluginSolutionUniqueName);

            var pluginAssembly = typeof(TPlugin).Assembly;

            var pluginType = pluginAssembly.GetType("XrmFramework.Plugin");
            var customApiType = pluginAssembly.GetType("XrmFramework.CustomApi");
            var workflowType = pluginAssembly.GetType("XrmFramework.Workflow.CustomWorkflowActivity");

            var pluginList = new List<Plugin>();

            var pluginTypes = pluginAssembly.GetTypes().Where(t => pluginType.IsAssignableFrom(t) && !customApiType.IsAssignableFrom(t) && t.IsPublic && !t.IsAbstract).ToList();

            var workflowTypes = pluginAssembly.GetTypes().Where(t => workflowType.IsAssignableFrom(t) && !t.IsAbstract && t.IsPublic).ToList();

            var customApiTypes = pluginAssembly.GetTypes().Where(t => customApiType.IsAssignableFrom(t) && t.IsPublic && !t.IsAbstract).ToList();

            foreach (var type in pluginTypes)
            {
                dynamic pluginTemp;
                if (type.GetConstructor(new[] { typeof(string), typeof(string) }) != null)
                {
                    pluginTemp = Activator.CreateInstance(type, new object[] { null, null });
                }
                else
                {
                    pluginTemp = Activator.CreateInstance(type, new object[] { });
                }

                var plugin = Plugin.FromXrmFrameworkPlugin(pluginTemp, false);

                pluginList.Add(plugin);
            }

            foreach (var type in workflowTypes)
            {
                dynamic pluginTemp = Activator.CreateInstance(type, new object[] { });

                var plugin = Plugin.FromXrmFrameworkPlugin(pluginTemp, true);

                pluginList.Add(plugin);
            }

            var customApis = new List<CustomApi>();

            foreach (var type in customApiTypes)
            {
                dynamic customApiTemp;
                if (type.GetConstructor(new[] { typeof(string), typeof(string) }) != null)
                {
                    customApiTemp = Activator.CreateInstance(type, new object[] { null, null });
                }
                else
                {
                    customApiTemp = Activator.CreateInstance(type, new object[] { });
                }

                var customApi = CustomApi.FromXrmFrameworkCustomApi(customApiTemp, _publisher.CustomizationPrefix, isOnPrem);

                customApis.Add(customApi);
            }


            var assembly = GetAssemblyByName(service, pluginAssembly.GetName().Name);

            var major = assembly?.Major;
            var minor = assembly?.Minor;

            var profilerAssembly = GetProfilerAssembly(service);

            var registeredPluginTypes = new List<PluginType>();
            var registeredCustomApis = new List<CustomApi>();
            var registeredCustomApiRequestParameters = new List<CustomApiRequestParameter>();
            var registeredCustomApiResponseProperties = new List<CustomApiResponseProperty>();
            var profiledSteps = new List<SdkMessageProcessingStep>();
            ICollection<SdkMessageProcessingStep> registeredSteps = Enumerable.Empty<SdkMessageProcessingStep>().ToList();

            var assemblyPath = pluginAssembly.Location;

            var newAssembly = GetAssemblyToRegister(pluginAssembly, assemblyPath);

            if (assembly == null || newAssembly.Major != major || newAssembly.Minor != minor)
            {
                if (assembly != null)
                {
                    Console.WriteLine($"Version increase: from {major}.{minor} to {newAssembly.Major}.{newAssembly.Minor}");
                    Console.WriteLine("Registering a new assembly and removing steps and plugin types from previous version");

                    registeredPluginTypes = GetRegisteredPluginTypes(service, assembly.Id).ToList();
                    registeredCustomApis = GetRegisteredCustomApis(service, assembly.Id).ToList();
                    registeredCustomApiRequestParameters = GetRegisteredCustomApiRequestParameters(service, assembly.Id).ToList();
                    registeredCustomApiResponseProperties = GetRegisteredCustomApiResponseProperties(service, assembly.Id).ToList();

                    registeredSteps = GetRegisteredSteps(service, assembly.Id);

                    foreach (var registeredType in registeredPluginTypes.Where(r => r.IsWorkflowActivity != true))
                    {
                        var registeredStepsForPluginType = registeredSteps.Where(s => s.EventHandler.Id == registeredType.Id).ToList();
                        foreach (var step in registeredStepsForPluginType)
                        {
                            service.Delete(SdkMessageProcessingStep.EntityLogicalName, step.Id);
                            registeredSteps.Remove(step);
                        }

                        foreach (var customApi in registeredCustomApis.Where(c => c.PluginTypeId?.Id == registeredType.Id))
                        {
                            service.Delete(customApi.LogicalName, customApi.Id);
                        }

                        service.Delete(PluginType.EntityLogicalName, registeredType.Id);
                    }

                    registeredPluginTypes = new List<PluginType>();
                    registeredCustomApis = new List<CustomApi>();
                    registeredCustomApiRequestParameters = new List<CustomApiRequestParameter>();
                    registeredCustomApiResponseProperties = new List<CustomApiResponseProperty>();
                    profiledSteps = new List<SdkMessageProcessingStep>();
                    registeredSteps = Enumerable.Empty<SdkMessageProcessingStep>().ToList();
                }

                Console.WriteLine("Registering assembly");

                assembly = newAssembly;

                assembly.Id = service.Create(assembly);
            }
            else
            {
                Console.WriteLine("Updating plugin assembly");

                var updatedAssembly = newAssembly;
                updatedAssembly.Id = assembly.Id;

                registeredPluginTypes = GetRegisteredPluginTypes(service, assembly.Id).ToList();
                registeredCustomApis = GetRegisteredCustomApis(service, assembly.Id).ToList();
                registeredCustomApiRequestParameters = GetRegisteredCustomApiRequestParameters(service, assembly.Id).ToList();
                registeredCustomApiResponseProperties = GetRegisteredCustomApiResponseProperties(service, assembly.Id).ToList();

                registeredSteps = GetRegisteredSteps(service, assembly.Id);

                if (profilerAssembly != null)
                {
                    profiledSteps = GetRegisteredSteps(service, profilerAssembly.Id).ToList();
                }

                foreach (var registeredType in registeredPluginTypes)
                {
                    if (pluginList.All(p => p.FullName != registeredType.Name) && pluginList.Where(p => p.IsWorkflow).All(c => c.FullName != registeredType.TypeName) && customApis.All(c => c.FullName != registeredType.TypeName))
                    {
                        var registeredStepsForPluginType = registeredSteps.Where(s => s.EventHandler.Id == registeredType.Id).ToList();
                        foreach (var step in registeredStepsForPluginType)
                        {
                            service.Delete(SdkMessageProcessingStep.EntityLogicalName, step.Id);
                            registeredSteps.Remove(step);
                        }

                        foreach (var customApi in registeredCustomApis.Where(c => c.PluginTypeId?.Id == registeredType.Id))
                        {
                            service.Delete(customApi.LogicalName, customApi.Id);
                        }

                        service.Delete(PluginType.EntityLogicalName, registeredType.Id);
                    }
                }

                service.Update(updatedAssembly);
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
                        if (comparer.NeedsUpdate(stepToRegister, registeredStep))
                        {
                            var profiledStep = profiledSteps.FirstOrDefault(p => p.Name.StartsWith(registeredStep.Name));

                            if (profiledStep != null)
                            {
                                service.Delete(profiledStep.LogicalName, profiledStep.Id);

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

                    AddSolutionComponentToSolution(service, pluginSolutionUniqueName, stepToRegister.ToEntityReference());

                    if (convertedStep.Message != Messages.Associate.ToString() && convertedStep.Message != Messages.Lose.ToString() && convertedStep.Message != Messages.Win.ToString())
                    {
                        var registeredPostImage = registeredImages.FirstOrDefault(i => i.Name == "PostImage" && i.SdkMessageProcessingStepId.Id == stepToRegister.Id);

                        if (convertedStep.PostImageUsed && convertedStep.Message != Messages.Delete.ToString())
                        {
                            if (registeredPostImage == null)
                            {
                                registeredPostImage = GetImageToRegister(service, stepToRegister.Id, convertedStep, false);
                                registeredPostImage.Id = service.Create(registeredPostImage);

                            }
                            else if (registeredPostImage.Attributes1 != convertedStep.JoinedPostImageAttributes)
                            {
                                registeredPostImage.Attributes1 = convertedStep.JoinedPostImageAttributes;
                                service.Update(registeredPostImage);
                            }
                        }
                        else if (registeredPostImage != null)
                        {
                            service.Delete(registeredPostImage.LogicalName, registeredPostImage.Id);
                        }

                        var registeredPreImage = registeredImages.FirstOrDefault(i => i.Name == "PreImage" && i.SdkMessageProcessingStepId.Id == stepToRegister.Id);

                        if (convertedStep.PreImageUsed)
                        {
                            if (registeredPreImage == null)
                            {
                                registeredPreImage = GetImageToRegister(service, stepToRegister.Id, convertedStep, true);
                                registeredPreImage.Id = service.Create(registeredPreImage);
                            }
                            else if (registeredPreImage.Attributes1 != convertedStep.JoinedPreImageAttributes)
                            {
                                registeredPreImage.Attributes1 = convertedStep.JoinedPreImageAttributes;
                                service.Update(registeredPreImage);
                            }
                        }
                        else if (registeredPreImage != null)
                        {
                            service.Delete(registeredPreImage.LogicalName, registeredPreImage.Id);
                        }
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

            var customApiEntityTypeCode = GetEntityTypeCode("customapi", service);
            var customApiParameterEntityTypeCode = GetEntityTypeCode("customapirequestparameter", service);
            var customApiResponseEntityTypeCode = GetEntityTypeCode("customapiresponseproperty", service);

            foreach (var customApi in customApis)
            {
                Console.WriteLine($@"  - {customApi.FullName}");

                var registeredPluginType = registeredPluginTypes.FirstOrDefault(p => p.Name == customApi.FullName);

                if (registeredPluginType == null)
                {
                    registeredPluginType = GetPluginTypeToRegister(assembly.Id, customApi.FullName);
                    registeredPluginType.Id = service.Create(registeredPluginType);
                }

                customApi.PluginTypeId = new EntityReference(PluginType.EntityLogicalName, registeredPluginType.Id);

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
                    var existingRequestParameter = registeredCustomApiRequestParameters.FirstOrDefault(p => p.UniqueName == customApiRequestParameter.UniqueName && p.CustomApiId.Id == existingCustomApi.Id);

                    customApiRequestParameter.CustomApiId = new EntityReference(CustomApi.EntityLogicalName, existingCustomApi.Id);

                    if (existingRequestParameter == null)
                    {
                        customApiRequestParameter.Id = service.Create(customApiRequestParameter);
                    }
                    else
                    {
                        customApiRequestParameter.Id = existingRequestParameter.Id;
                        service.Update(customApiRequestParameter);
                        registeredCustomApiRequestParameters.Remove(existingRequestParameter);
                    }

                    AddSolutionComponentToSolution(service, pluginSolutionUniqueName, customApiRequestParameter.ToEntityReference(),
                        customApiParameterEntityTypeCode);
                }

                foreach (var customApiResponseProperty in customApi.OutArguments)
                {
                    var existingResponseProperty = registeredCustomApiResponseProperties.FirstOrDefault(p => p.UniqueName == customApiResponseProperty.UniqueName && p.CustomApiId.Id == existingCustomApi.Id);

                    customApiResponseProperty.CustomApiId = new EntityReference(CustomApi.EntityLogicalName, existingCustomApi.Id);

                    if (existingResponseProperty == null)
                    {
                        customApiResponseProperty.Id = service.Create(customApiResponseProperty);
                    }
                    else
                    {
                        customApiResponseProperty.Id = existingResponseProperty.Id;
                        service.Update(customApiResponseProperty);
                        registeredCustomApiResponseProperties.Remove(existingResponseProperty);
                    }

                    AddSolutionComponentToSolution(service, pluginSolutionUniqueName, customApiResponseProperty.ToEntityReference(),
                        customApiResponseEntityTypeCode);
                }


                AddSolutionComponentToSolution(service, pluginSolutionUniqueName, customApi.ToEntityReference(),
                    customApiEntityTypeCode);
            }

            if (registeredCustomApiRequestParameters.Any() || registeredCustomApiResponseProperties.Any())
            {
                Console.WriteLine("Deleting unnecessary request parameters and response properties");

                foreach (var parameterToRemove in registeredCustomApiRequestParameters)
                {
                    service.Delete(CustomApiRequestParameter.EntityLogicalName, parameterToRemove.Id);
                }

                foreach (var responseToRemove in registeredCustomApiResponseProperties)
                {
                    service.Delete(CustomApiResponseProperty.EntityLogicalName, responseToRemove.Id);
                }
            }

            Console.WriteLine();

            foreach (var step in registeredSteps)
            {
                service.Delete(SdkMessageProcessingStep.EntityLogicalName, step.Id);
            }
        }

        private static int GetEntityTypeCode(string logicalName, CrmServiceClient service)
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

            var query = new QueryExpression(PluginType.EntityLogicalName);
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition("pluginassemblyid", ConditionOperator.Equal, pluginAssemblyId);

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

            var query = new QueryExpression(CustomApi.EntityLogicalName);
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition("ismanaged", ConditionOperator.Equal, false);
            var linkPluginType = query.AddLink(PluginType.EntityLogicalName, "plugintypeid", "plugintypeid");
            linkPluginType.LinkCriteria.AddCondition("pluginassemblyid", ConditionOperator.Equal, assemblyId);

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

            var query = new QueryExpression(CustomApiRequestParameter.EntityLogicalName);
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition("ismanaged", ConditionOperator.Equal, false);

            var linkCustomApi = query.AddLink(CustomApi.EntityLogicalName, "customapiid", "customapiid");
            var linkPluginType = linkCustomApi.AddLink(PluginType.EntityLogicalName, "plugintypeid", "plugintypeid");
            linkPluginType.LinkCriteria.AddCondition("pluginassemblyid", ConditionOperator.Equal, assemblyId);

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

            var query = new QueryExpression(CustomApiResponseProperty.EntityLogicalName);
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition("ismanaged", ConditionOperator.Equal, false);

            var linkCustomApi = query.AddLink(CustomApi.EntityLogicalName, "customapiid", "customapiid");
            var linkPluginType = linkCustomApi.AddLink(PluginType.EntityLogicalName, "plugintypeid", "plugintypeid");
            linkPluginType.LinkCriteria.AddCondition("pluginassemblyid", ConditionOperator.Equal, assemblyId);

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

            var query = new QueryExpression("sdkmessageprocessingstep");
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition("stage", ConditionOperator.NotEqual, 30);

            var linkPluginType = query.AddLink(PluginType.EntityLogicalName, "eventhandler", "plugintypeid");
            linkPluginType.LinkCriteria.AddCondition("pluginassemblyid", ConditionOperator.Equal, assemblyId);

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

            var query = new QueryExpression("sdkmessageprocessingstepimage");
            query.ColumnSet.AllColumns = true;
            var stepLink = query.AddLink(SdkMessageProcessingStep.EntityLogicalName, "sdkmessageprocessingstepid", "sdkmessageprocessingstepid");
            var linkPluginType = stepLink.AddLink(PluginType.EntityLogicalName, "eventhandler", "plugintypeid");
            linkPluginType.LinkCriteria.AddCondition("pluginassemblyid", ConditionOperator.Equal, assemblyId);

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
                var query = new QueryExpression("pluginassembly");
                query.ColumnSet.AllColumns = true;
                query.Distinct = true;
                query.Criteria.FilterOperator = LogicalOperator.And;
                query.Criteria.AddCondition("name", ConditionOperator.NotLike, "CompiledWorkflow%");
                var filter = query.Criteria.AddFilter(LogicalOperator.Or);
                filter.AddCondition("customizationlevel", ConditionOperator.Null);
                filter.AddCondition("customizationlevel", ConditionOperator.NotEqual, 0);
                filter.AddCondition("name", ConditionOperator.In, "Microsoft.Crm.ObjectModel", "Microsoft.Crm.ServiceBus");

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

            var major = int.Parse(version.Split('.')[0]);
            var minor = int.Parse(version.Split('.')[1]);

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

            t["major"] = major;
            t["minor"] = minor;

            return t;
        }

        private static PluginType GetCustomWorkflowTypeToRegister(Guid pluginAssemblyId, string pluginFullName, string displayName)
        {
            var t = new PluginType()
            {
                PluginAssemblyId = new EntityReference()
                {
                    LogicalName = PluginAssembly.EntityLogicalName,
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
                    LogicalName = PluginAssembly.EntityLogicalName,
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

            var entityName = step.EntityName;

            if (string.IsNullOrWhiteSpace(step.EntityName) && !string.IsNullOrWhiteSpace(step.UnsecureConfig))
            {
                try
                {
                    var stepConfiguration = JsonConvert.DeserializeObject<StepConfiguration>(step.UnsecureConfig);
                    entityName = stepConfiguration.RelationshipName;
                }
                catch
                {
                    // ignored
                }
            }

            var description = $"{step.PluginTypeName} : {step.Stage} {step.Message} of {entityName} ({step.MethodsDisplayName})";
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
                EventHandler = new EntityReference(PluginType.EntityLogicalName, pluginTypeId),
                FilteringAttributes = step.DoNotFilterAttributes ? null : string.Join(",", step.FilteringAttributes),
                ImpersonatingUserId = string.IsNullOrEmpty(step.ImpersonationUsername) ? null : new EntityReference("systemuser", _users.First(u => u.Key == step.ImpersonationUsername).Value),
#pragma warning disable 0612
                InvocationSource = new OptionSetValue((int)sdkmessageprocessingstep_invocationsource.Child),
#pragma warning restore 0612
                IsCustomizable = new BooleanManagedProperty(true),
                IsHidden = new BooleanManagedProperty(false),
                Mode = new OptionSetValue((int)step.Mode),
                Name = description,
#pragma warning disable 0612
                PluginTypeId = new EntityReference(PluginType.EntityLogicalName, pluginTypeId),
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
                ImageType = new OptionSetValue(isPreImage ? (int)sdkmessageprocessingstepimage_imagetype.PreImage : (int)sdkmessageprocessingstepimage_imagetype.PostImage),
                IsCustomizable = new BooleanManagedProperty(true),
                MessagePropertyName = messagePropertyName,
                Name = name,
                SdkMessageProcessingStepId = new EntityReference(SdkMessageProcessingStep.EntityLogicalName, stepId)
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
                        case PluginAssembly.EntityLogicalName:
                            s.ComponentType = (int)componenttype.PluginAssembly;
                            break;
                        case PluginType.EntityLogicalName:
                            s.ComponentType = (int)componenttype.PluginType;
                            break;
                        case SdkMessageProcessingStep.EntityLogicalName:
                            s.ComponentType = (int)componenttype.SDKMessageProcessingStep;
                            break;
                        case SdkMessageProcessingStepImage.EntityLogicalName:
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

            var query = new QueryExpression(SdkMessageFilter.EntityLogicalName);
            query.ColumnSet.AddColumns("sdkmessagefilterid", "sdkmessageid", "primaryobjecttypecode");
            query.Criteria.AddCondition("iscustomprocessingstepallowed", ConditionOperator.Equal, true);
            query.Criteria.AddCondition("isvisible", ConditionOperator.Equal, true);

            var filters = RetrieveAll(service, query);

            //Console.WriteLine($"Retrieved {filters.Count} message filters in {sw.Elapsed}");

            _filters.Clear();
            _filters.AddRange(filters.Select(f => f.ToEntity<SdkMessageFilter>()));

            sw.Restart();
            query = new QueryExpression(SdkMessage.EntityLogicalName);
            query.ColumnSet.AddColumns("sdkmessageid", "name");

            var messages = RetrieveAll(service, query).Select(e => e.ToEntity<SdkMessage>());

            //Console.WriteLine($"Retrieved {messages.Count()} messages in {sw.Elapsed}");

            _messages.Clear();
            foreach (SdkMessage e in messages)
            {
                _messages.Add(e.Name, e.ToEntityReference());
            }

            sw.Restart();

            query = new QueryExpression(Solution.EntityLogicalName);
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition("uniquename", ConditionOperator.Equal, solutionName);

            _solution = RetrieveAll(service, query).Select(s => s.ToEntity<Solution>()).FirstOrDefault();

            //Console.WriteLine($"Retrieved solution by name in {sw.Elapsed}");

            if (_solution == null)
            {
                Console.WriteLine("The solution {0} does not exist in the CRM, modify App.config to point to an existing solution.", solutionName);
                Console.WriteLine("\r\nAppuyez sur une touche pour arrêter.");
                Console.ReadKey();
                System.Environment.Exit(1);
            }
            else if (_solution.GetAttributeValue<bool>("ismanaged"))
            {
                Console.WriteLine("The solution {0} is managed in the CRM, modify App.config to point to a development environment.", solutionName);
                System.Environment.Exit(1);
            }
            else
            {
                _publisher = service.Retrieve(Publisher.EntityLogicalName, _solution.PublisherId.Id, new ColumnSet(true)).ToEntity<Publisher>();


                query = new QueryExpression(SolutionComponent.EntityLogicalName);
                query.ColumnSet.AllColumns = true;
                query.Criteria.AddCondition("solutionid", ConditionOperator.Equal, _solution.Id);

                var components = RetrieveAll(service, query).Select(s => s.ToEntity<SolutionComponent>());

                _components.AddRange(components);
            }

            query = new QueryExpression("systemuser");
            query.ColumnSet.AddColumn("domainname");
            query.Criteria.AddCondition("accessmode", ConditionOperator.NotEqual, 3);
            query.Criteria.AddCondition("isdisabled", ConditionOperator.Equal, false);

            foreach (var user in RetrieveAll(service, query))
            {
                _users.Add(new KeyValuePair<string, Guid>(user.GetAttributeValue<string>("domainname"), user.Id));
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



        private static Solution _solution = null;
        private static Publisher _publisher = null;

        private static readonly List<SolutionComponent> _components = new();
        private static readonly List<SdkMessageFilter> _filters = new();
        private static readonly Dictionary<string, EntityReference> _messages = new();
        private static readonly List<KeyValuePair<string, Guid>> _users = new();
    }
}