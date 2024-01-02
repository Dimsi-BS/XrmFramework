// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Xrm.Sdk;

namespace XrmFramework.RemoteDebugger.Common
{
    public class LocalOrganizationServiceFactory : IOrganizationServiceFactory
    {
        private readonly LocalServiceProvider.RequestHandler _onRequestSent;
        private RemoteDebugExecutionContext Context { get; }

        public LocalOrganizationServiceFactory(RemoteDebugExecutionContext context, LocalServiceProvider.RequestHandler onRequestSent)
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