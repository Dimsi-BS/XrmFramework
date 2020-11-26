// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using Microsoft.Xrm.Sdk;
using Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XrmFramework.Utils;

namespace Plugins
{
    public interface IServiceContext : ILoggerContext
    {
        IOrganizationService AdminOrganizationService { get; }
        
        IOrganizationService OrganizationService { get; }

        EntityReference BusinessUnitRef { get; }

        void Log(string message, params object[] paramsObject);

        void ThrowInvalidPluginException(string messageId, params object[] args);

        IOrganizationService GetService(Guid userId);

        LogServiceMethod LogServiceMethod { get; }
    }
}
