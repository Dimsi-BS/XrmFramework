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
        private static List<PluginAssembly> _list = new List<PluginAssembly>();

        public static void RegisterPluginsAndWorkflows<TPlugin>(string projectName)
        {
            //GetProjectConfig
            //Get configuration info for the project

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


            //  ------------------------------------------------------------------------
            // Get name of the solution for which the plugins are made
            var pluginSolutionUniqueName = projectConfig.TargetSolution;
            //Get the string required to connect to the CRM project
            var connectionString = ConfigurationManager.ConnectionStrings[xrmFrameworkConfigSection.SelectedConnection].ConnectionString;

            Console.WriteLine($"You are about to deploy on {connectionString} organization. If ok press any key.");
            Console.ReadKey();
            Console.WriteLine("Connecting to CRM...");

            CrmServiceClient.MaxConnectionTimeout = TimeSpan.FromMinutes(10);

            //Create the CrmServiceClient to interact with the CrmProject 
            var service = new CrmServiceClient(connectionString);

            //Kind of deprecated, allow use of early bound classes like in DeployUtils.Model.Entities, to make a strongly typed object from a table 
            service.OrganizationServiceProxy?.EnableProxyTypes();

            //------------------------------------------------------------------------------------------------
            // Get all users, messages, messages filters for the CRM solution 
            InitMetadata(service, pluginSolutionUniqueName);
            //------------------------------------------------------------------------------------------------
            // Get the assembly that contains the classes Plugin, CustomWorkflowActivity and CustomApi that are needed in order to understand what will be registered
            var pluginAssembly = typeof(TPlugin).Assembly;
            //Get each possible type of plugin
            var pluginType = pluginAssembly.GetType("XrmFramework.Plugin");
            var customApiType = pluginAssembly.GetType("XrmFramework.CustomApi");
            var workflowType = pluginAssembly.GetType("XrmFramework.Workflow.CustomWorkflowActivity");


            //-----------------------------------------------------------------------
            var pluginList = new List<Plugin>();
            var customApis = new List<CustomApi>();
            // Get all plugin types that were developped by the users
            var pluginTypes = pluginAssembly.GetTypes().Where(t => pluginType.IsAssignableFrom(t) && !customApiType.IsAssignableFrom(t) && t.IsPublic && !t.IsAbstract).ToList();

            var workflowTypes = pluginAssembly.GetTypes().Where(t => workflowType.IsAssignableFrom(t) && !t.IsAbstract && t.IsPublic).ToList();

            var customApiTypes = pluginAssembly.GetTypes().Where(t => customApiType.IsAssignableFrom(t) && t.IsPublic && !t.IsAbstract).ToList();



            //------------------------------------------------------------------------------------
            // Get each plugin data, the plugin class in deployUtils is not the same as in Plugin, it is metadata
            GetPluginData(pluginTypes, pluginList);
            GetWorkflowData(workflowTypes, pluginList);
            GetCustomApiData(customApiTypes, customApis);

            // ------------------------------------------------------------------------------------------
            // 
            //GetCrmData();
            // Get the plugin assembly from the CRM                                                                                
            var assembly = GetAssemblyByName(service, pluginAssembly.GetName().Name);
            //Get plugin profiler assembly from the CRM                                                                            
            var profilerAssembly = GetProfilerAssembly(service);
            var registeredPluginTypes = new List<PluginType>();
            var registeredCustomApis = new List<CustomApi>();
            var registeredCustomApiRequestParameters = new List<CustomApiRequestParameter>();
            var registeredCustomApiResponseProperties = new List<CustomApiResponseProperty>();
            var profiledSteps = new List<SdkMessageProcessingStep>();
            //Collection of the steps for when a plugin will need to execute 
            ICollection<SdkMessageProcessingStep> registeredSteps = Enumerable.Empty<SdkMessageProcessingStep>().ToList();

            var assemblyPath = pluginAssembly.Location;

            GetCrmData(ref assembly, ref pluginAssembly, ref profilerAssembly, ref registeredPluginTypes,
                       ref registeredCustomApis, ref registeredCustomApiRequestParameters, ref registeredCustomApiResponseProperties,
                       ref profiledSteps, ref registeredSteps, ref assemblyPath, ref service, ref pluginList, ref customApis);

            // Adding assembly to solution
            AddSolutionComponentToSolution(service, pluginSolutionUniqueName, assembly.ToEntityReference());

            // Get preimages of assembly
            var registeredImages = GetRegisteredImages(service, assembly.Id);

            Console.WriteLine();
            Console.WriteLine(@"Registering Plugins");

            RegisterPlugins(ref pluginList, ref registeredPluginTypes, ref service,
                            assembly, ref registeredSteps, ref profiledSteps, pluginSolutionUniqueName, ref registeredImages);


            Console.WriteLine();
            Console.WriteLine(@"Registering Custom Workflow activities");
            // Register the customWorkflows
            RegisterWorkflow(ref pluginList, ref registeredPluginTypes, ref service, ref assembly);

            // Add relevant custom api
            Console.WriteLine();
            Console.WriteLine(@"Registering Custom Apis");

            var customApiEntityTypeCode = GetEntityTypeCode("customapi", service);
            var customApiParameterEntityTypeCode = GetEntityTypeCode("customapirequestparameter", service);
            var customApiResponseEntityTypeCode = GetEntityTypeCode("customapiresponseproperty", service);

            RegisterCustomApi(ref customApis, ref registeredPluginTypes, ref registeredCustomApis,
            ref service, assembly, ref registeredCustomApiRequestParameters, pluginSolutionUniqueName,
             customApiParameterEntityTypeCode, ref registeredCustomApiResponseProperties, customApiResponseEntityTypeCode, customApiEntityTypeCode);

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

        public static List<Plugin> UpdateCrmData<TPlugin>(string projectName)
        {

            Console.WriteLine("Starting Crm update");
            var xrmFrameworkConfigSection = ConfigHelper.GetSection();
            var projectConfig = xrmFrameworkConfigSection.Projects.OfType<ProjectElement>()
                .FirstOrDefault(p => p.Name == projectName);

            if (projectConfig == null)
            {
                var defaultColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"No reference to the project {projectName} has been found in the xrmFramework.config file.");
                Console.ForegroundColor = defaultColor;
                return null;
            }
            string RD_PluginName = "XrmFramework.RemoteDebuggerPlugin";
            //  ------------------------------------------------------------------------
            // Get name of the solution for which the plugins are made
            var pluginSolutionUniqueName = projectConfig.TargetSolution;
            //Get the string required to connect to the CRM project
            var connectionString = ConfigurationManager.ConnectionStrings[xrmFrameworkConfigSection.SelectedConnection].ConnectionString;

            Console.WriteLine($"In order to start remote debugging you need to update the Crm data at {connectionString}. If ok press any key.");
            Console.ReadKey();
            Console.WriteLine("Connecting to CRM...");

            CrmServiceClient.MaxConnectionTimeout = TimeSpan.FromMinutes(10);

            //Create the CrmServiceClient to interact with the CrmProject 
            var service = new CrmServiceClient(connectionString);

            //Kind of deprecated, allow use of early bound classes like in DeployUtils.Model.Entities, to make a strongly typed object from a table 
            //service.OrganizationServiceProxy?.EnableProxyTypes();

            InitMetadata(service, pluginSolutionUniqueName);
            // Get the assembly that contains the classes Plugin, CustomWorkflowActivity and CustomApi that are needed in order to understand what will be registered
            var pluginAssembly = typeof(TPlugin).Assembly;
            //Get each possible type of plugin
            var pluginType = pluginAssembly.GetType("XrmFramework.Plugin");
            var customApiType = pluginAssembly.GetType("XrmFramework.CustomApi");
            var workflowType = pluginAssembly.GetType("XrmFramework.Workflow.CustomWorkflowActivity");

            Console.WriteLine(pluginType.Name);
            var pluginList = new List<Plugin>();
            var customApis = new List<CustomApi>();
            // Get all plugin types that were developped by the users
            var pluginTypes = pluginAssembly.GetTypes().Where(t => pluginType.IsAssignableFrom(t) && !customApiType.IsAssignableFrom(t) && t.IsPublic && !t.IsAbstract).ToList();

            foreach (var t in pluginTypes)
            {
                Console.WriteLine(t.Name);
            }
            GetPluginData(pluginTypes, pluginList);


            var profilerAssembly = GetProfilerAssembly(service);
            var registeredPluginTypes = new List<PluginType>();
            var registeredCustomApis = new List<CustomApi>();
            var registeredCustomApiRequestParameters = new List<CustomApiRequestParameter>();
            var registeredCustomApiResponseProperties = new List<CustomApiResponseProperty>();
            var profiledSteps = new List<SdkMessageProcessingStep>();
            //Collection of the steps for when a plugin will need to execute 
            ICollection<SdkMessageProcessingStep> registeredSteps = Enumerable.Empty<SdkMessageProcessingStep>().ToList();

            var assembly = GetAssemblyByName(service, pluginAssembly.GetName().Name);

            var assemblyPath = pluginAssembly.Location;

            if (assembly == null)
            {
                throw new Exception("Error, remote debugger cannot be used on non deployed plugins (no corresponding assembly found)");
            }
            else
            {
                Console.WriteLine("Updating plugin assembly");

                var updatedAssembly = new Entity("pluginassembly")
                {
                    Id = assembly.Id,
                    ["content"] = Convert.ToBase64String(File.ReadAllBytes(assemblyPath))
                };

                registeredPluginTypes = GetRegisteredPluginTypes(service, assembly.Id).ToList();
                //registeredCustomApis = GetRegisteredCustomApis(service, assembly.Id).ToList();

                //registeredCustomApiRequestParameters = GetRegisteredCustomApiRequestParameters(service, assembly.Id).ToList();
                //registeredCustomApiResponseProperties = GetRegisteredCustomApiResponseProperties(service, assembly.Id).ToList();
                registeredSteps = GetRegisteredSteps(service, assembly.Id);

                if (profilerAssembly != null)
                {
                    profiledSteps = GetRegisteredSteps(service, profilerAssembly.Id).ToList();
                }

                foreach (var registeredType in registeredPluginTypes)
                {
                    bool isDebuggingPlugin = false;
                    if (registeredType.Name == "FrameworkTests.Plugins.RD_Plugin")
                    {
                        Console.WriteLine("Found debugging plugin");
                        isDebuggingPlugin = true;

                    }
                    Console.WriteLine("Plugin name");
                    Console.WriteLine(registeredType.Name);
                    var registeredStepsForPluginType = registeredSteps.Where(s => s.EventHandler.Id == registeredType.Id).ToList();
                    if (isDebuggingPlugin)
                    {
                        // Delete each step before assigning the ones used for this session
                        foreach (var step in registeredStepsForPluginType)
                        {
                            service.Delete(SdkMessageProcessingStep.EntityLogicalName, step.Id);
                            registeredSteps.Remove(step);
                        }

                    }
                    if (pluginList.All(p => p.FullName != registeredType.Name) && pluginList.Where(p => p.IsWorkflow).All(c => c.FullName != registeredType.TypeName) && customApis.All(c => c.FullName != registeredType.TypeName))
                    {

                        {
                            //
                        }

                        foreach (var customApi in registeredCustomApis.Where(c => c.PluginTypeId?.Id == registeredType.Id))
                        {
                            //service.Delete(customApi.LogicalName, customApi.Id);
                        }

                        //service.Delete(PluginType.EntityLogicalName, registeredType.Id);
                    }
                }

                service.Update(updatedAssembly);
            }
            AddSolutionComponentToSolution(service, pluginSolutionUniqueName, assembly.ToEntityReference());

            var registeredImages = GetRegisteredImages(service, assembly.Id);

            //-------------------------------------------------------------------------------------------------
            List<Step> stepsToBeRegistered = new List<Step>();
            Plugin debugPlugin;
            PluginType debugPluginType = null;
            foreach (var plugin in pluginList.Where(p => !p.IsWorkflow))
            {
                bool isDebuggingPlugin = false;
                if (plugin.FullName == "FrameworkTests.Plugins.RD_Plugin")
                {
                    Console.WriteLine("Found debugging plugin");
                    isDebuggingPlugin = true;
                    debugPlugin = plugin;

                }
                Console.WriteLine("Plugin name");
                Console.WriteLine(plugin.FullName);

                Console.WriteLine($@"  - {plugin.FullName}");
                if (isDebuggingPlugin)
                {
                    var registeredPluginType = registeredPluginTypes.FirstOrDefault(p => p.Name == plugin.FullName);
                    debugPluginType = registeredPluginType;
                    if (registeredPluginType == null)
                    {
                        registeredPluginType = GetPluginTypeToRegister(assembly.Id, plugin.FullName);
                        registeredPluginType.Id = service.Create(registeredPluginType);
                    }
                }
                else
                {
                    foreach (var convertedStep in plugin.Steps)
                    {
                        stepsToBeRegistered.Add(convertedStep);


                        if (convertedStep.Message != Messages.Associate.ToString() && convertedStep.Message != Messages.Lose.ToString() && convertedStep.Message != Messages.Win.ToString())
                        {

                        }
                    }
                }
                /*
                

                // We get this plugin from the CRM

                // If it is not yet registered, create it with the service
                <
                // Get the registered steps corresponding to the plugin
                var registeredStepsForPluginType = registeredSteps.Where(s => s.EventHandler.Id == registeredPluginType.Id).ToList();

                var comparer = new SdkMessageStepComparer();

                */
            }


            // Now register each step but for the RD_Plugin

            foreach (var s in stepsToBeRegistered)
            {
                Console.WriteLine("A step has to be debbuged");
                Console.WriteLine(s.PluginTypeName);
                if (debugPluginType == null)
                {
                    Console.WriteLine("Error no debugPluginType");
                }
                else
                {
                    var stepToRegister = GetStepToRegister(debugPluginType.Id, s);
                    Console.WriteLine(s.ImpersonationUsername);
                    stepToRegister.Id = service.Create(stepToRegister);
                    AddSolutionComponentToSolution(service, pluginSolutionUniqueName, stepToRegister.ToEntityReference());

                }


            }

            return pluginList;



        }

        public static void UpdateRemoteDebuggerPlugin<TPlugin>(string projectName)
        {
            Console.WriteLine(projectName);
            // Connect a service to the debug session
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
            //var pluginSolutionUniqueName = "dimsi_debugsession";
            var pluginSolutionUniqueName = projectConfig.TargetSolution;
            var connectionString = ConfigurationManager.ConnectionStrings[xrmFrameworkConfigSection.SelectedConnection].ConnectionString;

            Console.WriteLine($"You are about to modify the debug session");
            Console.WriteLine($"Do you want to register new steps to debug ? (y/n)");
            var r = Console.ReadLine();
            while (r != "y" && r != "n")
            {
                Console.WriteLine($"Do you want to register new steps to debug ? (y/n)");
                r = Console.ReadLine();
            }
            if (r == "n")
            {
                return;
            }

            Console.WriteLine("Connecting to CRM...");
            CrmServiceClient.MaxConnectionTimeout = TimeSpan.FromMinutes(10);

            //Create the CrmServiceClient to interact with the CrmProject 
            var service = new CrmServiceClient(connectionString);
            Console.WriteLine("Created serviceClient");
            //Kind of deprecated, allow use of early bound classes like in DeployUtils.Model.Entities, to make a strongly typed object from a table 
            //service.OrganizationServiceProxy?.EnableProxyTypes();
            var debugAssembly = GetAssemblyByName(service, "XrmFramework.RemoteDebuggerPlugin");

            //InitMetadata(service, pluginSolutionUniqueName);
            InitStepMetadata(service, pluginSolutionUniqueName);


            //Now get the local assembly for the plugin(s) to be debugged
            var pluginAssembly = typeof(TPlugin).Assembly;
            //Get each possible type of plugin
            var pluginType = pluginAssembly.GetType("XrmFramework.Plugin");
            var customApiType = pluginAssembly.GetType("XrmFramework.CustomApi");
            var workflowType = pluginAssembly.GetType("XrmFramework.Workflow.CustomWorkflowActivity");
            Console.WriteLine(pluginType.Name);
            var pluginList = new List<Plugin>();
            var customApis = new List<CustomApi>();
            // Get all plugin types that were developped by the users
            var pluginTypes = pluginAssembly.GetTypes().Where(t => pluginType.IsAssignableFrom(t) && !customApiType.IsAssignableFrom(t) && t.IsPublic && !t.IsAbstract).ToList();
            // Now for each local plugin we assign steps to the debug plugin
            GetPluginData(pluginTypes, pluginList);


            // Now we get the remoteDebugger plugin
            var debuggerPlugin = GetRegisteredPluginTypes(service, debugAssembly.Id).ToList()[0];
            Console.WriteLine("name of debugger plugin is {0}", debuggerPlugin.Name);
            var registeredSteps = GetRegisteredSteps(service, debugAssembly.Id);
            var registeredImages = GetRegisteredImages(service, debugAssembly.Id);

            var registeredStepsForPluginType = registeredSteps.Where(s => s.EventHandler.Id == debuggerPlugin.Id).ToList();
            foreach (var step in registeredStepsForPluginType)
            {

                service.Delete(SdkMessageProcessingStep.EntityLogicalName, step.Id);
                //registeredStepsForPluginType.Remove(step);
            }
            foreach (var image in registeredImages)
            {
                service.Delete(SdkMessageProcessingStepImage.EntityLogicalName, image.Id);
                //registeredImages.Remove(image);
            }

            foreach (var plugin in pluginTypes)
            {
                foreach (var pluginData in pluginList)
                {
                    if (pluginData.FullName == plugin.FullName)
                    {
                        foreach (var convertedStep in pluginData.Steps)
                        {
                            Console.WriteLine("There is a step here");

                            if (convertedStep.Message != Messages.Associate.ToString() && convertedStep.Message != Messages.Lose.ToString() && convertedStep.Message != Messages.Win.ToString())
                            {
                                convertedStep.UnsecureConfig = plugin.AssemblyQualifiedName;
                                //convertedStep.SecuredConfig = "oulalalalolo";
                                var stepToRegister = GetStepToRegister(debuggerPlugin.Id, convertedStep);

                                //Console.WriteLine(s.ImpersonationUsername);
                                //stepToRegister.

                                stepToRegister.Id = service.Create(stepToRegister);
                                AddSolutionComponentToSolution(service, pluginSolutionUniqueName, stepToRegister.ToEntityReference());

                                if (convertedStep.PostImageUsed && convertedStep.Message != Messages.Delete.ToString())
                                {

                                    var postImage = GetImageToRegister(service, stepToRegister.Id, convertedStep, false);
                                    postImage.Id = service.Create(postImage);
                                    AddSolutionComponentToSolution(service, pluginSolutionUniqueName, postImage.ToEntityReference());


                                }


                                //Add the relevant preimages for each step
                                //var registeredPreImage = registeredImages.FirstOrDefault(i => i.Name == "PreImage" && i.SdkMessageProcessingStepId.Id == stepToRegister.Id);

                                if (convertedStep.PreImageUsed)
                                {
                                    var preImage = GetImageToRegister(service, stepToRegister.Id, convertedStep, true);
                                    preImage.Id = service.Create(preImage);
                                    AddSolutionComponentToSolution(service, pluginSolutionUniqueName, preImage.ToEntityReference());



                                }

                            }
                        }
                    }
                }

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
                query.ColumnSet.AddColumns("pluginassemblyid", "name");
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
                FilteringAttributes = step.FilteringAttributes.Any() ? string.Join(",", step.FilteringAttributes) : null,
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


        private static void InitStepMetadata(IOrganizationService service, string solutionName)
        {
            Console.WriteLine("Metadata initialization");

            var sw = Stopwatch.StartNew();

            // Get the message filters that allow custom processing steps and are visible ?
            var query = new QueryExpression(SdkMessageFilter.EntityLogicalName);
            query.ColumnSet.AddColumns("sdkmessagefilterid", "sdkmessageid", "primaryobjecttypecode");
            query.Criteria.AddCondition("iscustomprocessingstepallowed", ConditionOperator.Equal, true);
            query.Criteria.AddCondition("isvisible", ConditionOperator.Equal, true);

            var filters = RetrieveAll(service, query);

            //Console.WriteLine($"Retrieved {filters.Count} message filters in {sw.Elapsed}");

            // Clear filters from the collection (est ce que c'est _filters qui contient tous les filtres utilisés ?, ce qui voudrait dire qu'on pourrait s'en servir pour le debugging ?)
            _filters.Clear();
            _filters.AddRange(filters.Select(f => f.ToEntity<SdkMessageFilter>()));

            //Get all the messages in the project (update, Create, Delete etc ??)
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
            /*
            //Get the solution entity the plugins will be added to 
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
                //If the solution is real and usable, get its publisher
                _publisher = service.Retrieve(Publisher.EntityLogicalName, _solution.PublisherId.Id, new ColumnSet(true)).ToEntity<Publisher>();


                query = new QueryExpression(SolutionComponent.EntityLogicalName);
                query.ColumnSet.AllColumns = true;
                query.Criteria.AddCondition("solutionid", ConditionOperator.Equal, _solution.Id);

                var components = RetrieveAll(service, query).Select(s => s.ToEntity<SolutionComponent>());

                _components.AddRange(components);
            }*/

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

        private static void InitMetadata(IOrganizationService service, string solutionName)
        {
            Console.WriteLine("Metadata initialization");

            var sw = Stopwatch.StartNew();

            // Get the message filters that allow custom processing steps and are visible ?
            var query = new QueryExpression(SdkMessageFilter.EntityLogicalName);
            query.ColumnSet.AddColumns("sdkmessagefilterid", "sdkmessageid", "primaryobjecttypecode");
            query.Criteria.AddCondition("iscustomprocessingstepallowed", ConditionOperator.Equal, true);
            query.Criteria.AddCondition("isvisible", ConditionOperator.Equal, true);

            var filters = RetrieveAll(service, query);

            //Console.WriteLine($"Retrieved {filters.Count} message filters in {sw.Elapsed}");

            // Clear filters from the collection (est ce que c'est _filters qui contient tous les filtres utilisés ?, ce qui voudrait dire qu'on pourrait s'en servir pour le debugging ?)
            _filters.Clear();
            _filters.AddRange(filters.Select(f => f.ToEntity<SdkMessageFilter>()));

            //Get all the messages in the project (update, Create, Delete etc ??)
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

            //Get the solution entity the plugins will be added to 
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
                //If the solution is real and usable, get its publisher
                _publisher = service.Retrieve(Publisher.EntityLogicalName, _solution.PublisherId.Id, new ColumnSet(true)).ToEntity<Publisher>();


                query = new QueryExpression(SolutionComponent.EntityLogicalName);
                query.ColumnSet.AllColumns = true;
                query.Criteria.AddCondition("solutionid", ConditionOperator.Equal, _solution.Id);

                var components = RetrieveAll(service, query).Select(s => s.ToEntity<SolutionComponent>());

                _components.AddRange(components);
            }

            //Get users (pourquoi lui il est pas réinitialisé ?)
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

        private static void GetPluginData(List<Type> pluginTypes, List<Plugin> plugins)
        {
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

                plugins.Add(plugin);
            }
        }

        private static void GetWorkflowData(List<Type> workflowTypes, List<Plugin> pluginList)
        {
            foreach (var type in workflowTypes)
            {
                dynamic pluginTemp = Activator.CreateInstance(type, new object[] { });

                var plugin = Plugin.FromXrmFrameworkPlugin(pluginTemp, true);

                pluginList.Add(plugin);
            }

        }
        private static void GetCustomApiData(List<Type> customApiTypes, List<CustomApi> customApis)
        {
            foreach (var type in customApiTypes)
            {
                dynamic customApiTemp;
                if (type.GetConstructor(new[] { typeof(string), typeof(string) }) != null)
                {
                    customApiTemp = Activator.CreateInstance(type, new object[] { null, null });
                }
                else
                {
                    Console.WriteLine("We are using this version of the constructor");
                    customApiTemp = Activator.CreateInstance(type, new object[] { });

                }

                var customApi = CustomApi.FromXrmFrameworkCustomApi(customApiTemp, _publisher.CustomizationPrefix);

                customApis.Add(customApi);
            }
        }

        public static void GetCrmData(ref PluginAssembly assembly, ref Assembly pluginAssembly, ref PluginAssembly profilerAssembly, ref List<PluginType> registeredPluginTypes,
           ref List<CustomApi> registeredCustomApis, ref List<CustomApiRequestParameter> registeredCustomApiRequestParameters, ref List<CustomApiResponseProperty> registeredCustomApiResponseProperties,
            ref List<SdkMessageProcessingStep> profiledSteps, ref ICollection<SdkMessageProcessingStep> registeredSteps, ref string assemblyPath, ref CrmServiceClient service, ref List<Plugin> pluginList, ref List<CustomApi> customApis)
        {


            if (assembly == null)
            {
                Console.WriteLine("Registering assembly");

                assembly = GetAssemblyToRegister(pluginAssembly, assemblyPath);

                assembly.Id = service.Create(assembly);
            }
            else
            {
                Console.WriteLine("Updating plugin assembly");

                var updatedAssembly = new Entity("pluginassembly")
                {
                    Id = assembly.Id,
                    ["content"] = Convert.ToBase64String(File.ReadAllBytes(assemblyPath))
                };

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

        }


        private static void RegisterPlugins(ref List<Plugin> pluginList, ref List<PluginType> registeredPluginTypes, ref CrmServiceClient service,
            PluginAssembly assembly, ref ICollection<SdkMessageProcessingStep> registeredSteps, ref List<SdkMessageProcessingStep> profiledSteps, string pluginSolutionUniqueName, ref ICollection<SdkMessageProcessingStepImage> registeredImages)
        {
            foreach (var plugin in pluginList.Where(p => !p.IsWorkflow))
            {
                Console.WriteLine($@"  - {plugin.FullName}");
                // We get this plugin from the CRM
                var registeredPluginType = registeredPluginTypes.FirstOrDefault(p => p.Name == plugin.FullName);

                // If it is not yet registered, create it with the service
                if (registeredPluginType == null)
                {
                    registeredPluginType = GetPluginTypeToRegister(assembly.Id, plugin.FullName);
                    registeredPluginType.Id = service.Create(registeredPluginType);
                }
                // Get the registered steps corresponding to the plugin
                var registeredStepsForPluginType = registeredSteps.Where(s => s.EventHandler.Id == registeredPluginType.Id).ToList();

                var comparer = new SdkMessageStepComparer();

                foreach (var convertedStep in plugin.Steps)
                {
                    var stepToRegister = GetStepToRegister(registeredPluginType.Id, convertedStep);

                    var registeredStep = registeredStepsForPluginType.FirstOrDefault(s => comparer.Equals(s, stepToRegister));

                    //If the step is not yet registered, meaning it is a new one, create it in the CRM
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
                    // Add the new registered steps to the CRM
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

                        //Add the relevant preimages for each step
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
        }
        private static void RegisterWorkflow(ref List<Plugin> pluginList, ref List<PluginType> registeredPluginTypes, ref CrmServiceClient service, ref PluginAssembly assembly)
        {
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
        }

        private static void RegisterCustomApi(ref List<CustomApi> customApis, ref List<PluginType> registeredPluginTypes, ref List<CustomApi> registeredCustomApis,
            ref CrmServiceClient service, PluginAssembly assembly, ref List<CustomApiRequestParameter> registeredCustomApiRequestParameters, string pluginSolutionUniqueName,
            int customApiParameterEntityTypeCode, ref List<CustomApiResponseProperty> registeredCustomApiResponseProperties, int customApiResponseEntityTypeCode, int customApiEntityTypeCode)
        {
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
        }

        private static Solution _solution = null;
        private static Publisher _publisher = null;

        private static readonly List<SolutionComponent> _components = new();
        private static readonly List<SdkMessageFilter> _filters = new();
        private static readonly Dictionary<string, EntityReference> _messages = new();
        private static readonly List<KeyValuePair<string, Guid>> _users = new();
    }


}