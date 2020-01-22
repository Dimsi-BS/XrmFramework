// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
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

        string OrganizationName { get; }
    }
}
