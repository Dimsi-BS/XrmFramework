// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using BoDi;

namespace XrmFramework
{
    public static partial class InternalDependencyProvider
    {
        static partial void RegisterCustomService(IObjectContainer container)
        {
            // Enregistre SystemDateTimeProvider comme implémentation par défaut de IDateTimeProvider.
            // Cette registration peut être surchargée par InitializeDateTimeProvider()
            // dans le contexte RemoteDebugger pour utiliser FixedDateTimeProvider lors des rejouages.
            container.RegisterInstanceAs<IDateTimeProvider>(SystemDateTimeProvider.Instance);
        }
    }
}
