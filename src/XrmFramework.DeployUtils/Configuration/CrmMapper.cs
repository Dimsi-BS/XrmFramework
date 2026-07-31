using System;
using Deploy;
using Microsoft.Xrm.Sdk;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;
using CustomApi = XrmFramework.DeployUtils.Model.CustomApi;
using CustomApiRequestParameter = XrmFramework.DeployUtils.Model.CustomApiRequestParameter;
using CustomApiResponseProperty = XrmFramework.DeployUtils.Model.CustomApiResponseProperty;
using PluginPackage = XrmFramework.DeployUtils.Model.PluginPackage;

namespace XrmFramework.DeployUtils.Configuration;

/// <summary>
///     Hand-written mapper that replaces AutoMapper for all CRM component conversions.
/// </summary>
public class CrmMapper : ICrmMapper
{
    // ═══════════════════════════════════════════════════════════════════════════
    // Local-to-Local (deep clones)
    // ═══════════════════════════════════════════════════════════════════════════

    public AssemblyInfo Clone(AssemblyInfo src)
    {
        if (src == null) return null;

        var dest = new AssemblyInfo
        {
            Id               = src.Id,
            ParentId         = src.ParentId,
            RegistrationState = src.RegistrationState,
            Name             = src.Name,
            SourceType       = src.SourceType,
            IsolationMode    = src.IsolationMode,
            Culture          = src.Culture,
            PublicKeyToken   = src.PublicKeyToken,
            Version          = src.Version,
            Description      = src.Description,
            Content          = src.Content
        };

        // Deep-clone the Package to avoid RegistrationState mutation across copies
        if (src.Package != null)
            dest.AddChild(Clone(src.Package));

        return dest;
    }

    public PluginPackage Clone(PluginPackage src)
    {
        if (src == null) return null;

        return new PluginPackage
        {
            Id                = src.Id,
            ParentId          = src.ParentId,
            RegistrationState = src.RegistrationState,
            Name              = src.Name,
            UniqueName        = src.UniqueName,
            Version           = src.Version,
            Content           = src.Content
        };
    }

    public Plugin Clone(Plugin src)
    {
        if (src == null) return null;

        var dest = src.IsWorkflow
            ? new Plugin(src.FullName, src.DisplayName)
            : new Plugin(src.FullName);

        dest.Id               = src.Id;
        dest.ParentId         = src.ParentId;
        dest.RegistrationState = src.RegistrationState;

        // Clone steps (Children of Plugin) and re-add them
        foreach (var step in src.Steps)
        {
            dest.AddChild(Clone(step));
        }

        return dest;
    }

    public Step Clone(Step src)
    {
        if (src == null) return null;

        var dest = new Step(src.PluginTypeName, src.Message, src.Stage, src.Mode, src.EntityName)
        {
            Id                = src.Id,
            ParentId          = src.ParentId,
            RegistrationState = src.RegistrationState,
            PluginTypeFullName = src.PluginTypeFullName,
            MessageId         = src.MessageId,
            DoNotFilterAttributes = src.DoNotFilterAttributes,
            Order             = src.Order,
            ImpersonationUsername = src.ImpersonationUsername,
            StepConfiguration = CloneStepConfiguration(src.StepConfiguration)
        };

        dest.FilteringAttributes.UnionWith(src.FilteringAttributes);

        // Clone pre/post images
        dest.PreImage  = Clone(src.PreImage);
        dest.PreImage.FatherStep = dest;
        dest.PostImage = Clone(src.PostImage);
        dest.PostImage.FatherStep = dest;

        return dest;
    }

    public StepImage Clone(StepImage src)
    {
        if (src == null) return null;

        var dest = new StepImage(src.Message, src.IsPreImage, src.Stage)
        {
            Id                = src.Id,
            ParentId          = src.ParentId,
            RegistrationState = src.RegistrationState,
            AllAttributes     = src.AllAttributes
        };

        dest.Attributes.UnionWith(src.Attributes);

        // FatherStep is set by the caller (Step.Clone)
        return dest;
    }

    public CustomApi Clone(CustomApi src)
    {
        if (src == null) return null;

        var dest = new CustomApi
        {
            Id                              = src.Id,
            ParentId                        = src.ParentId,
            RegistrationState               = src.RegistrationState,
            AssemblyId                      = src.AssemblyId,
            FullName                        = src.FullName,
            DisplayName                     = src.DisplayName,
            Name                            = src.Name,
            Prefix                          = src.Prefix,
            AllowedCustomProcessingStepType = src.AllowedCustomProcessingStepType,
            BoundEntityLogicalName          = src.BoundEntityLogicalName,
            BindingType                     = src.BindingType,
            Description                     = src.Description,
            ExecutePrivilegeName            = src.ExecutePrivilegeName,
            IsFunction                      = src.IsFunction,
            IsPrivate                       = src.IsPrivate,
            WorkflowSdkStepEnabled          = src.WorkflowSdkStepEnabled
        };

        foreach (var child in src.Children)
        {
            switch (child)
            {
                case CustomApiRequestParameter req:
                    dest.AddChild(Clone(req));
                    break;
                case CustomApiResponseProperty resp:
                    dest.AddChild(Clone(resp));
                    break;
            }
        }

        return dest;
    }

    public CustomApiRequestParameter Clone(CustomApiRequestParameter src)
    {
        if (src == null) return null;

        return new CustomApiRequestParameter
        {
            Id                = src.Id,
            ParentId          = src.ParentId,
            RegistrationState = src.RegistrationState,
            Description       = src.Description,
            DisplayName       = src.DisplayName,
            IsOptional        = src.IsOptional,
            Type              = src.Type,
            Name              = src.Name,
            UniqueName        = src.UniqueName
        };
    }

    public CustomApiResponseProperty Clone(CustomApiResponseProperty src)
    {
        if (src == null) return null;

        return new CustomApiResponseProperty
        {
            Id                = src.Id,
            ParentId          = src.ParentId,
            RegistrationState = src.RegistrationState,
            Description       = src.Description,
            DisplayName       = src.DisplayName,
            IsOptional        = src.IsOptional,
            Type              = src.Type,
            Name              = src.Name,
            UniqueName        = src.UniqueName
        };
    }

    /// <summary>
    ///     Deep-clones an entire <see cref="IAssemblyContext" /> tree,
    ///     preserving the hierarchy (Plugins with Steps/Images, CustomApis with args, Workflows).
    /// </summary>
    public IAssemblyContext Clone(IAssemblyContext src)
    {
        if (src == null) return new AssemblyContext();

        var dest = new AssemblyContext
        {
            AssemblyInfo = Clone(src.AssemblyInfo)
        };

        foreach (var child in src.Children)
        {
            switch (child)
            {
                case Plugin plugin:
                    dest.AddChild(Clone(plugin));
                    break;
                case CustomApi customApi:
                    dest.AddChild(Clone(customApi));
                    break;
            }
        }

        return dest;
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // Remote-to-Local
    // ═══════════════════════════════════════════════════════════════════════════

    public AssemblyInfo FromRemote(PluginAssembly src)
    {
        if (src == null) return null;

        return new AssemblyInfo
        {
            Id               = src.Id,
            Name             = src.Name,
            Culture          = src.Culture,
            PublicKeyToken   = src.PublicKeyToken,
            Version          = src.Version,
            Description      = src.Description,
            IsolationMode    = src.IsolationMode != null
                                   ? (IsolationMode) src.IsolationMode.Value
                                   : IsolationMode.Sandbox,
            SourceType       = src.SourceType != null
                                   ? (SourceType) src.SourceType.Value
                                   : SourceType.Database
            // Content is not mapped from remote (no need to round-trip it)
        };
    }

    public PluginPackage FromRemote(Deploy.PluginPackage src)
    {
        if (src == null) return null;

        return new PluginPackage
        {
            Id         = src.Id,
            Name       = src.Name,
            UniqueName = src.UniqueName,
            Version    = src.Version
            // Content is not mapped from remote
        };
    }

    public CustomApi FromRemote(Deploy.CustomApi src)
    {
        if (src == null) return null;

        var dest = new CustomApi
        {
            Id                              = src.Id,
            // ParentId = PluginTypeId.Id (the plugin type that implements the custom api)
            ParentId                        = src.PluginTypeId?.Id ?? Guid.Empty,
            AllowedCustomProcessingStepType = src.AllowedCustomProcessingStepType,
            BoundEntityLogicalName          = src.BoundEntityLogicalName,
            BindingType                     = src.BindingType,
            Description                     = src.Description,
            DisplayName                     = src.DisplayName,
            ExecutePrivilegeName            = src.ExecutePrivilegeName,
            IsFunction                      = src.IsFunction,
            IsPrivate                       = src.IsPrivate,
            WorkflowSdkStepEnabled          = src.WorkflowSdkStepEnabled
        };

        // UniqueName setter splits on '_' to populate Prefix and Name
        dest.UniqueName = src.UniqueName;

        return dest;
    }

    public CustomApiRequestParameter FromRemote(Deploy.CustomApiRequestParameter src)
    {
        if (src == null) return null;

        return new CustomApiRequestParameter
        {
            Id          = src.Id,
            ParentId    = src.CustomApiId?.Id ?? Guid.Empty,
            Description = src.Description,
            DisplayName = src.DisplayName,
            IsOptional  = src.IsOptional,
            Type        = src.Type,
            // Name and UniqueName are swapped between remote and local
            UniqueName  = src.Name,
            Name        = src.UniqueName
        };
    }

    public CustomApiResponseProperty FromRemote(Deploy.CustomApiResponseProperty src)
    {
        if (src == null) return null;

        return new CustomApiResponseProperty
        {
            Id          = src.Id,
            ParentId    = src.CustomApiId?.Id ?? Guid.Empty,
            Description = src.Description,
            DisplayName = src.DisplayName,
            IsOptional  = true,
            Type        = src.Type,
            // Name and UniqueName are swapped between remote and local
            UniqueName  = src.Name,
            Name        = src.UniqueName
        };
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // Local-to-Remote
    // ═══════════════════════════════════════════════════════════════════════════

    public PluginAssembly ToRemote(AssemblyInfo src)
    {
        if (src == null) return null;

        var dest = new PluginAssembly
        {
            Name           = src.Name,
            Culture        = src.Culture,
            PublicKeyToken = src.PublicKeyToken,
            Version        = src.Version,
            Description    = src.Description,
            // Content must be Base64-encoded for the CRM API
            Content        = src.Content != null ? Convert.ToBase64String(src.Content) : null,
            IsolationMode  = new OptionSetValue((int) src.IsolationMode),
            SourceType     = new OptionSetValue((int) src.SourceType)
            // PackageId is intentionally left null — managed by AssemblyExporter separately
        };

        if (src.Id != Guid.Empty) dest.Id = src.Id;

        return dest;
    }

    public Deploy.PluginPackage ToRemote(PluginPackage src)
    {
        if (src == null) return null;

        var dest = new Deploy.PluginPackage
        {
            Name       = src.Name,
            UniqueName = src.UniqueName,
            Version    = src.Version,
            Content    = src.Content != null ? Convert.ToBase64String(src.Content) : null
        };

        if (src.Id != Guid.Empty) dest.Id = src.Id;

        return dest;
    }

    public Deploy.CustomApi ToRemote(CustomApi src)
    {
        if (src == null) return null;

        var dest = new Deploy.CustomApi
        {
            Name                            = src.Name,
            UniqueName                      = src.UniqueName,
            DisplayName                     = src.DisplayName,
            AllowedCustomProcessingStepType = src.AllowedCustomProcessingStepType,
            BoundEntityLogicalName          = src.BoundEntityLogicalName,
            BindingType                     = src.BindingType,
            Description                     = src.Description,
            ExecutePrivilegeName            = src.ExecutePrivilegeName,
            IsFunction                      = src.IsFunction,
            IsPrivate                       = src.IsPrivate,
            WorkflowSdkStepEnabled          = src.WorkflowSdkStepEnabled,
            PluginTypeId                    = new EntityReference(PluginTypeDefinition.EntityName, src.ParentId)
        };

        if (src.Id != Guid.Empty) dest.Id = src.Id;

        return dest;
    }

    public Deploy.CustomApiRequestParameter ToRemote(CustomApiRequestParameter src)
    {
        if (src == null) return null;

        var dest = new Deploy.CustomApiRequestParameter
        {
            Description      = src.Description,
            DisplayName      = src.DisplayName,
            IsOptional       = src.IsOptional,
            Type             = src.Type,
            // Name and UniqueName are swapped
            UniqueName       = src.Name,
            Name             = src.UniqueName,
            CustomApiId      = new EntityReference(CustomApiDefinition.EntityName, src.ParentId),
            // LogicalEntityName is intentionally omitted
        };

        if (src.Id != Guid.Empty) dest.Id = src.Id;

        return dest;
    }

    public Deploy.CustomApiResponseProperty ToRemote(CustomApiResponseProperty src)
    {
        if (src == null) return null;

        var dest = new Deploy.CustomApiResponseProperty
        {
            Description      = src.Description,
            DisplayName      = src.DisplayName,
            Type             = src.Type,
            // Name and UniqueName are swapped
            UniqueName       = src.Name,
            Name             = src.UniqueName,
            CustomApiId      = new EntityReference(CustomApiDefinition.EntityName, src.ParentId),
            // LogicalEntityName is intentionally omitted
        };

        if (src.Id != Guid.Empty) dest.Id = src.Id;

        return dest;
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // Private helpers
    // ═══════════════════════════════════════════════════════════════════════════

    private static StepConfiguration CloneStepConfiguration(StepConfiguration src)
    {
        if (src == null) return new StepConfiguration();

        var dest = new StepConfiguration
        {
            RelationshipName = src.RelationshipName
        };

        dest.RegisteredMethods.UnionWith(src.RegisteredMethods);

        return dest;
    }
}
