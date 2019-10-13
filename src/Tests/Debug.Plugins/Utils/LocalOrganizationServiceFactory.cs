// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Xrm.Sdk;
using Plugins;
using XrmFramework.Debugger;

namespace Debug.Plugins
{
    public class LocalOrganizationServiceFactory : IOrganizationServiceFactory
    {
        private readonly RemoteServiceProvider.RequestHandler _onRequestSent;
        public RemoteDebugPluginExecutionContext Context { get; }

        public LocalOrganizationServiceFactory(RemoteDebugPluginExecutionContext context, RemoteServiceProvider.RequestHandler onRequestSent)
        {
            _onRequestSent = onRequestSent;
            Context = context;
        }

        public IOrganizationService CreateOrganizationService(Guid? userId)
        {
            return new DebuggerOrganizationService(Context, userId, _onRequestSent);
        }
    }
}