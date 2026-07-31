// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace XrmFramework
{
    /// <summary>
    /// Marker interface for internal XrmFramework services that can be injected
    /// as parameters of plugin methods and workflow activities.
    /// <para>
    /// Any service implementing this interface can be declared as a parameter
    /// of a plugin method or custom workflow activity and will be resolved
    /// automatically from the dependency container.
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
