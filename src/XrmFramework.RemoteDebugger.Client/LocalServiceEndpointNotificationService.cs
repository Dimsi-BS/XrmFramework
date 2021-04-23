// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Xrm.Sdk;

namespace XrmFramework.RemoteDebugger.Common
{
    public class LocalServiceEndpointNotificationService : IServiceEndpointNotificationService
    {
        public LocalServiceEndpointNotificationService(RemoteDebugExecutionContext context)
        {
        }

        public string Execute(EntityReference serviceEndpoint, IExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}