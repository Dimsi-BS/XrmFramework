using Microsoft.Xrm.Sdk;
using Plugins;
using System;
namespace Plugins
{
    public interface IPluginContext : IContext
    {
        string PrimaryEntityName { get; }

        Guid PrimaryEntityId { get; }

        [Obsolete("You should use this property when you have no other alternative.")]
        int Depth { get; }

        Guid UserId { get; }
        Guid InitiatingUserId { get; }

        bool IsPostOperation();
        bool IsPreOperation();
        bool IsPreValidation();
        bool IsStage(Stages stage);

        Guid OrganizationId { get; }

        Guid CorrelationId { get; }

        IPluginContext ParentContext { get; }

        bool IsMultiplePrePostOperation { get; }
    }
}
