// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Xrm.Sdk;

namespace XrmFramework
{
    public interface IServiceContext : ILoggerContext
    {
        IOrganizationService AdminOrganizationService { get; }
        
        IOrganizationService OrganizationService { get; }

        EntityReference BusinessUnitRef { get; }

        void Log(string message, params object[] paramsObject);

        IOrganizationService GetService(Guid userId);

        LogServiceMethod LogServiceMethod { get; }
    }
}
