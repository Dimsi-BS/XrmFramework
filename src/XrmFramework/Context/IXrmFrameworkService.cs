// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace XrmFramework
{
    /// <summary>
    /// Marker interface for the services that can be injected as parameters of plugin methods,
    /// Custom Api methods and custom workflow activities.
    /// <para>
    /// Any service implementing this interface can be declared as a parameter
    /// of a plugin method or custom workflow activity and will be resolved
    /// automatically from the dependency container.
    /// </para>
    /// <para>
    /// It covers the services provided by the framework itself, such as <see cref="IDateTimeProvider"/>,
    /// as well as the services a consuming project registers by completing the
    /// <c>InternalDependencyProvider.RegisterCustomService</c> partial method : implementing this
    /// interface is what authorizes such a custom service to be injected.
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
    /// <example>
    /// A custom service of the consuming project, registered once and injected like any built-in one :
    /// <code>
    /// public interface IPricingCalculator : IXrmFrameworkService
    /// {
    ///     decimal GetDiscountRate(Guid accountId);
    /// }
    ///
    /// namespace XrmFramework
    /// {
    ///     public static partial class InternalDependencyProvider
    ///     {
    ///         static partial void RegisterCustomService(IObjectContainer container)
    ///             =&gt; container.RegisterTypeAs&lt;PricingCalculator, IPricingCalculator&gt;();
    ///     }
    /// }
    ///
    /// public void HandleCreate(IPluginContext context, IPricingCalculator pricing)
    /// {
    ///     // ...
    /// }
    /// </code>
    /// </example>
    /// </summary>
    public interface IXrmFrameworkService
    {
    }
}
