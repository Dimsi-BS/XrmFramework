// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace XrmFramework
{
    public abstract class LoggedServiceBase : ILoggedService
    {
        protected IService Service { get; }
        protected LogServiceMethod Log { get; }

        protected LoggedServiceBase(IServiceContext context, IService service)
        {
            Log = context.LogServiceMethod;
            Service = service;
        }
    }
}