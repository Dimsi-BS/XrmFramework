using Deploy;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;
using XrmFramework.Definitions;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Utils
{
    public class CrmComponentConverter : ICrmComponentConverter
    {
        private readonly ISolutionContext _context;
        public CrmComponentConverter(ISolutionContext context)
        {
            _context = context;
        }
        public PluginType ToRegisterPluginType(Plugin plugin)
        {
            var t = new PluginType
            {
                PluginAssemblyId = new EntityReference
                {
                    LogicalName = PluginAssemblyDefinition.EntityName,
                    Id = plugin.ParentId
                },
                TypeName = plugin.FullName,
                FriendlyName = plugin.FullName,
                Name = plugin.FullName,
                Description = plugin.FullName
            };
            if (plugin.Id != Guid.Empty) t.Id = plugin.Id;

            return t;
        }

        public PluginType ToRegisterCustomWorkflowType(Plugin plugin)
        {
            var t = new PluginType
            {
                PluginAssemblyId = new EntityReference
                {
                    LogicalName = PluginAssemblyDefinition.EntityName,
                    Id = plugin.ParentId
                },
                TypeName = plugin.FullName,
                FriendlyName = plugin.FullName,
                Name = plugin.DisplayName,
                Description = string.Empty,
                WorkflowActivityGroupName = "Workflows"
            };

            return t;
        }

        public SdkMessageProcessingStep ToRegisterStep(Step step)
        {
            // Issue with CRM SDK / Description field max length = 256 characters
            var descriptionAttributeMaxLength = 256;
            var description = step.Description;
            description = description.Length <= descriptionAttributeMaxLength
            ? description
            : description.Substring(0, descriptionAttributeMaxLength - 4) + "...)";

            if (!string.IsNullOrEmpty(step.ImpersonationUsername))
            {
                var count = _context.Users.Count(u => u.Key == step.ImpersonationUsername);

                if (count == 0)
                    throw new Exception($"{description} : No user have fullname '{step.ImpersonationUsername}' in CRM.");
                if (count > 1)
                    throw new Exception(
                        $"{description} : {count} users have the fullname '{step.ImpersonationUsername}' in CRM.");
            }

            var t = new SdkMessageProcessingStep
            {
                AsyncAutoDelete = step.Mode.Equals(Modes.Asynchronous),
                Description = description,
                EventHandler = new EntityReference(PluginTypeDefinition.EntityName, step.ParentId),
                FilteringAttributes = step.FilteringAttributes.Any() ? string.Join(",", step.FilteringAttributes) : null,
                ImpersonatingUserId = string.IsNullOrEmpty(step.ImpersonationUsername)
                    ? null
                    : new EntityReference(SystemUserDefinition.EntityName,
                        _context.Users.First(u => u.Key == step.ImpersonationUsername).Value),

#pragma warning disable 0612
                InvocationSource = new OptionSetValue((int)sdkmessageprocessingstep_invocationsource.Child),
#pragma warning restore 0612
                IsCustomizable = new BooleanManagedProperty(true),
                IsHidden = new BooleanManagedProperty(false),
                Mode = new OptionSetValue((int)step.Mode),
                Name = description,
#pragma warning disable 0612
                PluginTypeId = new EntityReference(PluginTypeDefinition.EntityName, step.ParentId),
#pragma warning restore 0612
                Rank = step.Order,
                SdkMessageId = _context.Messages[step.Message], //GetSdkMessageRef(service, step.Message),
                SdkMessageFilterId = _context.Filters.Where(f => f.SdkMessageId.Name == step.Message.ToString()
                                                                 && f.PrimaryObjectTypeCode == step.EntityName)
                    .Select(f => f.ToEntityReference()).FirstOrDefault(), //GetSdkMessageFilterRef(service, step),
                                                                          //SdkMessageProcessingStepSecureConfigId = GetSdkMessageProcessingStepSecureConfigRef(service, step),
                Stage = new OptionSetValue((int)step.Stage),
                SupportedDeployment = new OptionSetValue((int)sdkmessageprocessingstep_supporteddeployment.ServerOnly),
                Configuration = step.UnsecureConfig
            };

            if (step.Id != Guid.Empty) t.Id = step.Id;

            return t;
        }

        public SdkMessageProcessingStepImage ToRegisterImage(StepImage image)
        {
            var isAllColumns = image.AllAttributes;
            var columns = image.JoinedAttributes;
            var name = image.IsPreImage ? "PreImage" : "PostImage";

            var messagePropertyName = "Target";

            if (image.Message == Messages.Create && !image.IsPreImage)
                messagePropertyName = "Id";
#pragma warning disable 618
            else if (image.Message == Messages.SetState ||
                     image.Message == Messages.SetStateDynamicEntity)
#pragma warning restore 618
                messagePropertyName = "EntityMoniker";

            var t = new SdkMessageProcessingStepImage
            {
                Attributes1 = isAllColumns ? null : columns,
                EntityAlias = name,
                ImageType = new OptionSetValue(image.IsPreImage
                    ? (int)sdkmessageprocessingstepimage_imagetype.PreImage
                    : (int)sdkmessageprocessingstepimage_imagetype.PostImage),
                IsCustomizable = new BooleanManagedProperty(true),
                MessagePropertyName = messagePropertyName,
                Name = name,
                SdkMessageProcessingStepId =
                    new EntityReference(SdkMessageProcessingStepDefinition.EntityName, image.ParentId)
            };

            if (image.Id != Guid.Empty) t.Id = image.Id;

            return t;
        }
    }
}
