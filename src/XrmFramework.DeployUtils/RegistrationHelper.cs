// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Description;
using System.Text;
using System.Xml.Linq;
using Deploy;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using XrmFramework.DeployUtils.Comparers;
using XrmFramework.DeployUtils.Configuration;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils
{
    public static class RegistrationHelper
    {
        private static List<PluginAssembly> _list = new List<PluginAssembly>();

        public static void Register<T, U>(string projectName, string assemblyPath, Func<T, Model.Plugin> PluginConverter, Func<U, Model.Plugin> WorkflowConverter)
        {
            Assembly pluginAssembly = typeof(T).Assembly;

            var pluginList = new List<Model.Plugin>();

            ObjectHelper<T>.ApplyCode(new Type[] { typeof(string), typeof(string) }, new object[] { null, null }, (plugin, type, sb) => { pluginList.Add(PluginConverter(plugin)); return false; });

            ObjectHelper<U>.ApplyCode(new Type[] { }, null, (wf, type, sb) => { pluginList.Add(WorkflowConverter(wf)); return false; });

            if (!(ConfigurationManager.GetSection("xrmFramework") is XrmFrameworkSection deploySection))
            {
                return;
            }

            var pluginSolutionUniqueName = deploySection.Projects.OfType<ProjectElement>().Single(p => p.Name == projectName).TargetSolution;

            var organizationName = ConfigurationManager.ConnectionStrings[deploySection.SelectedConnection].ConnectionString;

            Console.WriteLine($"You are about to deploy on {organizationName} organization. If ok press any key.");
            Console.ReadKey();
            Console.WriteLine("Connecting to CRM...");

            var cs = ConnectionStringParser.Parse(organizationName);

            var service = new OrganizationServiceProxy(new Uri(new Uri(cs.Url), "/XRMServices/2011/Organization.svc"), null, new ClientCredentials { UserName = { UserName = cs.Username, Password = cs.Password } }, null);

            service.EnableProxyTypes();

            var assembly = GetAssemblyByName(service, pluginAssembly.GetName().Name);

            var profilerAssembly = GetProfilerAssembly(service);

            InitMetadata(service, pluginSolutionUniqueName);

            var registeredPluginTypes = new List<PluginType>();
            var profiledSteps = new List<SdkMessageProcessingStep>();
            ICollection<SdkMessageProcessingStep> registeredSteps = Enumerable.Empty<SdkMessageProcessingStep>().ToList();

            if (assembly == null)
            {
                Console.WriteLine("Registering assembly");

                assembly = GetAssemblyToRegister(pluginAssembly, assemblyPath);

                assembly.Id = service.Create(assembly);
            }
            else
            {
                Console.WriteLine("Updating plugin assembly");

                var updatedAssembly = new Entity("pluginassembly");
                updatedAssembly.Id = assembly.Id;
                updatedAssembly["content"] = Convert.ToBase64String(File.ReadAllBytes(assemblyPath));

                registeredPluginTypes = GetRegisteredPluginTypes(service, assembly.Id).ToList();
                registeredSteps = GetRegisteredSteps(service, assembly.Id);

                if (profilerAssembly != null)
                {
                    profiledSteps = GetRegisteredSteps(service, profilerAssembly.Id).ToList();
                }

                Console.WriteLine("{0} Steps profiled", profiledSteps.Count());

                foreach (var registeredType in registeredPluginTypes)
                {
                    if (pluginList.All(p => p.FullName != registeredType.Name) && pluginList.Where(p => p.IsWorkflow).All(c => c.FullName != registeredType.TypeName))
                    {
                        var registeredStepsForPluginType = registeredSteps.Where(s => s.EventHandler.Id == registeredType.Id).ToList();
                        foreach (var step in registeredStepsForPluginType)
                        {
                            service.Delete(SdkMessageProcessingStep.EntityLogicalName, step.Id);
                            registeredSteps.Remove(step);
                        }
                        service.Delete(PluginType.EntityLogicalName, registeredType.Id);
                    }
                }

                // TODO : Améliorer la composition de la DLL pour optimiser son upload
                service.Timeout = TimeSpan.FromMinutes(10); 
                service.Update(updatedAssembly);
            }

            AddSolutionComponentToSolution(service, pluginSolutionUniqueName, assembly.ToEntityReference());

            var registeredImages = GetRegisteredImages(service, assembly.Id);

            foreach (var plugin in pluginList.Where(p => !p.IsWorkflow))
            {
                Console.WriteLine("Registering pluginType {0}", plugin.FullName);

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
                            else if (registeredPostImage.Attributes1 != convertedStep.PostImageAttributes)
                            {
                                registeredPostImage.Attributes1 = convertedStep.PostImageAttributes;
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
                            else if (registeredPreImage.Attributes1 != convertedStep.PreImageAttributes)
                            {
                                registeredPreImage.Attributes1 = convertedStep.PreImageAttributes;
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

            foreach (var customWf in pluginList.Where(p => p.IsWorkflow))
            {
                var registeredPluginType = registeredPluginTypes.FirstOrDefault(p => p.TypeName == customWf.FullName);

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

            foreach (var step in registeredSteps)
            {
                service.Delete(SdkMessageProcessingStep.EntityLogicalName, step.Id);
            }

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

        private static ICollection<SdkMessageProcessingStep> GetRegisteredSteps(IOrganizationService service, Guid assemblyId)
        {
            var list = new List<SdkMessageProcessingStep>();

            var query = new QueryExpression("sdkmessageprocessingstep");
            query.ColumnSet.AllColumns = true;
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
                EventHandler = new EntityReference(PluginType.EntityLogicalName, pluginTypeId),
                FilteringAttributes = string.IsNullOrEmpty(step.FilteredAttributes) ? null : step.FilteredAttributes,
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
            var columns = isPreImage ? step.PreImageAttributes : step.PostImageAttributes;
            var name = isPreImage ? "PreImage" : "PostImage";

            var messagePropertyName = "Target";

            if (step.Message == Model.Messages.Create.ToString() && !isPreImage)
            {
                messagePropertyName = "Id";
            }
            else if (step.Message == Model.Messages.SetState.ToString() || step.Message == Messages.SetStateDynamicEntity.ToString())
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

        private static void AddSolutionComponentToSolution(IOrganizationService service, string solutionUniqueName, EntityReference objectRef)
        {
            if (GetRegisteredSolutionComponent(objectRef) == null)
            {
                var s = new AddSolutionComponentRequest
                {
                    AddRequiredComponents = false,
                    ComponentId = objectRef.Id,
                    SolutionUniqueName = solutionUniqueName
                };

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

                service.Execute(s);
            }
        }

        private static SolutionComponent GetRegisteredSolutionComponent(EntityReference objectRef)
        {
            return _components.FirstOrDefault(c => c.ObjectId.Equals(objectRef.Id));
        }

        private static void InitMetadata(IOrganizationService service, string solutionName)
        {
            var query = new QueryExpression(SdkMessageFilter.EntityLogicalName);
            query.ColumnSet.AddColumns("sdkmessagefilterid", "sdkmessageid", "primaryobjecttypecode");
            query.Criteria.AddCondition("iscustomprocessingstepallowed", ConditionOperator.Equal, true);
            query.Criteria.AddCondition("isvisible", ConditionOperator.Equal, true);

            var filters = RetrieveAll(service, query);

            _filters.Clear();
            _filters.AddRange(filters.Select(f => f.ToEntity<SdkMessageFilter>()));

            query = new QueryExpression(SdkMessage.EntityLogicalName);
            query.ColumnSet.AddColumns("sdkmessageid", "name");

            var messages = RetrieveAll(service, query).Select(e => e.ToEntity<SdkMessage>());

            _messages.Clear();
            foreach (SdkMessage e in messages)
            {
                _messages.Add(e.Name, e.ToEntityReference());
            }

            query = new QueryExpression(Solution.EntityLogicalName);
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition("uniquename", ConditionOperator.Equal, solutionName);

            _solution = RetrieveAll(service, query).Select(s => s.ToEntity<Solution>()).FirstOrDefault();

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
        private static List<SolutionComponent> _components = new List<SolutionComponent>();
        private static List<SdkMessageFilter> _filters = new List<SdkMessageFilter>();
        private static readonly Dictionary<string, EntityReference> _messages = new Dictionary<string, EntityReference>();
        private static readonly List<KeyValuePair<string, Guid>> _users = new List<KeyValuePair<string, Guid>>();
    }
}