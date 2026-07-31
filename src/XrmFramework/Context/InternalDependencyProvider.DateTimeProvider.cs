// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using BoDi;

namespace XrmFramework
{
    public static partial class InternalDependencyProvider
    {
        static partial void RegisterCustomService(IObjectContainer container)
        {
            // Registers SystemDateTimeProvider as the default implementation of IDateTimeProvider.
            // This registration can be overridden by InitializeDateTimeProvider()
            // in the RemoteDebugger context to use FixedDateTimeProvider during replays.
            container.RegisterInstanceAs<IDateTimeProvider>(SystemDateTimeProvider.Instance);
        }
    }
}
