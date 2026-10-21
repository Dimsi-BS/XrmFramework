// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace XrmFramework
{
    /// <summary>
    /// Marker interface identifying the utility services provided by the framework itself — such as
    /// <see cref="IDateTimeProvider"/> — as opposed to the data access services extending
    /// <see cref="IService"/>.
    /// <para>
    /// It documents intent only : implementing it is <b>not</b> what makes a service injectable.
    /// Injection is driven by the dependency container, so any interface it can provide can be
    /// declared as a parameter of a plugin method, a Custom Api method or a custom workflow activity
    /// action — including the services a consuming project registers through
    /// <c>InternalDependencyProvider.RegisterCustomService</c> without implementing this interface.
    /// </para>
    /// <example>
    /// <code>
    /// public void HandleCreate(IPluginContext context, IDateTimeProvider clock)
    /// {
    ///     var expiryDate = clock.UtcNow.AddDays(30);
    ///     // ...
    /// }
    /// </code>
    /// </example>
    /// </summary>
    public interface IXrmFrameworkService
    {
    }
}
