using Microsoft.Xrm.Sdk;
using Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plugins
{
    public interface IServiceContext
    {
        IOrganizationService AdminOrganizationService { get; }
        
        IOrganizationService OrganizationService { get; }

        EntityReference BusinessUnitRef { get; }

        Guid UserId { get; }

        Guid InitiatingUserId { get; }

        void Log(string message, params object[] paramsObject);
        
        void ThrowInvalidPluginException(string messageId, params object[] args);

        IOrganizationService GetService(Guid userId);

        Logger Logger { get; }

        Guid CorrelationId { get; }
    }
}
