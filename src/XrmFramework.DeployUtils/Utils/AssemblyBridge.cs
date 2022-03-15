using Deploy;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using XrmFramework.Definitions;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Utils
{
    public static class AssemblyBridge
    {

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

        public static List<T> CreateInstanceOfTypeList<T>(List<Type> types, PluginRegistrationType kind, IRegistrationContext context)
        {
            List<T> list = new List<T>();
            foreach (var type in types)
            {
                dynamic temp = CreateInstanceOfType(type, kind);
                switch (kind)
                {
                    case PluginRegistrationType.Plugin:
                        list.Add(FromXrmFrameworkPlugin(temp, false));
                        break;

                    case PluginRegistrationType.Workflow:
                        list.Add(FromXrmFrameworkPlugin(temp, true));
                        break;

                    case PluginRegistrationType.CustomApi:
                        list.Add(FromXrmFrameworkCustomApi(temp, context.Publisher.CustomizationPrefix));
                        break;

                    default:
                        throw new InvalidEnumArgumentException("Unknown PluginRegistrationType");
                }
            }
            return list;
        }

        public static Plugin FromXrmFrameworkPlugin(dynamic plugin, bool isWorkflow = false)
        {
            var pluginTemp = !isWorkflow ? new Plugin(plugin.GetType().FullName) : new Plugin(plugin.GetType().FullName, plugin.DisplayName);

            if (!isWorkflow)
            {
                foreach (var step in plugin.Steps)
                {
                    pluginTemp.Steps.Add(FromXrmFrameworkStep(step));
                }
            }

            return pluginTemp;
        }

        public static Step FromXrmFrameworkStep(dynamic s)
        {
            var step = new Step(s.Plugin.GetType().Name, s.Message.ToString(), (Stages)(int)s.Stage, (Modes)(int)s.Mode, s.EntityName);

            step.FilteringAttributes.AddRange(s.FilteringAttributes);
            step.ImpersonationUsername = s.ImpersonationUsername;
            step.Order = s.Order;
            step.PostImageAllAttributes = s.PostImageAllAttributes;
            step.PostImageAttributes.AddRange(s.PostImageAttributes);
            step.PreImageAllAttributes = s.PreImageAllAttributes;
            step.PreImageAttributes.AddRange(s.PreImageAttributes);
            step.UnsecureConfig = s.UnsecureConfig;

            step.MethodNames.AddRange(s.MethodNames);

            return step;
        }

        public static CustomApi FromXrmFrameworkCustomApi(dynamic record, string prefix)
        {
            var type = (Type)record.GetType();

            dynamic customApiAttribute = type.GetCustomAttributes().FirstOrDefault(a => a.GetType().FullName == "XrmFramework.CustomApiAttribute");

            if (customApiAttribute == null)
            {
                throw new Exception($"The custom api type {type.FullName} must have a CustomApiAttribute defined");
            }

            var name = string.IsNullOrWhiteSpace(customApiAttribute.Name) ? type.Name : customApiAttribute.Name;

            var customApi = new CustomApi
            {
                DisplayName = string.IsNullOrWhiteSpace(customApiAttribute.DisplayName) ? name : customApiAttribute.DisplayName,
                Name = name,
                AllowedCustomProcessingStepType = new OptionSetValue((int)customApiAttribute.AllowedCustomProcessing),
                BindingType = new OptionSetValue((int)customApiAttribute.BindingType),
                BoundEntityLogicalName = customApiAttribute.BoundEntityLogicalName,
                Description = string.IsNullOrWhiteSpace(customApiAttribute.Description) ? name : customApiAttribute.Description,
                ExecutePrivilegeName = customApiAttribute.ExecutePrivilegeName,
                IsFunction = customApiAttribute.IsFunction,
                IsPrivate = customApiAttribute.IsPrivate,
                UniqueName = $"{prefix}_{name}",
                WorkflowSdkStepEnabled = customApiAttribute.WorkflowSdkStepEnabled,
                FullName = type.FullName
            };

            foreach (var argument in record.Arguments)
            {
                if (argument.IsInArgument)
                {
                    customApi.InArguments.Add(FromXrmFrameworkArgument<CustomApiRequestParameter>(customApi.Name, argument));
                }
                else
                {
                    customApi.OutArguments.Add(FromXrmFrameworkArgument<CustomApiResponseProperty>(customApi.Name, argument));
                }
            }

            return customApi;
        }

        public static T FromXrmFrameworkArgument<T>(string customApiName, dynamic argument) where T : ICustomApiComponent, new()
        {
            var res = new T
            {
                Description = string.IsNullOrWhiteSpace(argument.Description) ? $"{customApiName}.{argument.ArgumentName}" : argument.Description,
                UniqueName = $"{customApiName}.{argument.ArgumentName}",
                DisplayName = string.IsNullOrWhiteSpace(argument.DisplayName) ? $"{customApiName}.{argument.ArgumentName}" : argument.DisplayName,
                LogicalEntityName = argument.LogicalEntityName,
                Type = new OptionSetValue((int)argument.ArgumentType),
                Name = argument.ArgumentName
            };

            if (typeof(T).IsAssignableFrom(typeof(CustomApiRequestParameter)))
            {
                res.IsOptional = argument.IsOptional;
            }
            return res;
        }
        public static PluginType ToRegisterCustomWorkflowType(Guid pluginAssemblyId, string pluginFullName, string displayName)
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

        public static SdkMessageProcessingStepImage ToRegisterImage(Guid stepId, Model.Step step, bool isPreImage)
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

        public static PluginType ToRegisterPluginType(Guid pluginAssemblyId, string pluginFullName)
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

        public static SdkMessageProcessingStep ToRegisterStep(Guid pluginTypeId, Model.Step step, IRegistrationContext context)
        {
            // Issue with CRM SDK / Description field max length = 256 characters
            var descriptionAttributeMaxLength = 256;
            var description = $"{step.PluginTypeName} : {step.Stage} {step.Message} of {step.EntityName} ({step.MethodsDisplayName})";
            description = description.Length <= descriptionAttributeMaxLength ? description : description.Substring(0, descriptionAttributeMaxLength - 4) + "...)";

            if (!string.IsNullOrEmpty(step.ImpersonationUsername))
            {
                var count = context.Users.Count(u => u.Key == step.ImpersonationUsername);

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
                ImpersonatingUserId = string.IsNullOrEmpty(step.ImpersonationUsername)
                     ? null :
                       new EntityReference(SystemUserDefinition.EntityName,
                                           context.Users.First(u => u.Key == step.ImpersonationUsername).Value),

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
                SdkMessageId = context.Messages[step.Message], //GetSdkMessageRef(service, step.Message),
                SdkMessageFilterId = context.Filters.Where(f => f.SdkMessageId.Name == step.Message
                                                             && f.PrimaryObjectTypeCode == step.EntityName)
                                             .Select(f => f.ToEntityReference()).FirstOrDefault(), //GetSdkMessageFilterRef(service, step),
                //SdkMessageProcessingStepSecureConfigId = GetSdkMessageProcessingStepSecureConfigRef(service, step),
                Stage = new OptionSetValue((int)step.Stage),
                SupportedDeployment = new OptionSetValue((int)sdkmessageprocessingstep_supporteddeployment.ServerOnly),
                Configuration = step.UnsecureConfig
            };

            return t;
        }
    
        public static void AttachStepToPlugin(Guid pluginTypeId, SdkMessageProcessingStep step)
        {
            var t = new EntityReference(PluginTypeDefinition.EntityName, pluginTypeId);
            step.EventHandler = t;
#pragma warning disable 0612
            step.PluginTypeId = t;
#pragma warning restore 0612
        }
    }
}
