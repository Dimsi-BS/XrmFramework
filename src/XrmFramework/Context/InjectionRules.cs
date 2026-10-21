// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using BoDi;
using Microsoft.Xrm.Sdk;

namespace XrmFramework
{
    /// <summary>
    /// Centralizes the rules governing the parameters a plugin step method, a Custom Api method or a
    /// custom workflow activity action can declare, and their resolution from the dependency container.
    /// <para>
    /// The dependency container is the single source of truth : any interface it can provide is
    /// injectable, whether it is registered by the framework (<see cref="IService"/>,
    /// <see cref="IDateTimeProvider"/>, …) or by the consuming project through
    /// <c>InternalDependencyProvider.RegisterCustomService</c>. No marker interface is required.
    /// </para>
    /// </summary>
    internal static class InjectionRules
    {
        /// <summary>
        /// Returns <c>true</c> when <paramref name="type"/> can be declared as a parameter.
        /// <para>
        /// Only interfaces are accepted : BoDi implicitly registers and constructs any unknown
        /// concrete type, which would silently inject an object nobody registered. Whether the
        /// interface is actually available is decided by the container at resolution time, in
        /// <see cref="Resolve"/>.
        /// </para>
        /// </summary>
        /// <param name="type">The declared parameter type.</param>
        public static bool IsInjectableParameter(Type type) => type != null && type.IsInterface;

        /// <summary>
        /// Resolves <paramref name="parameterType"/> from <paramref name="container"/>, translating a
        /// missing registration into a message naming the member that cannot be injected.
        /// </summary>
        /// <param name="container">The dependency container of the current execution.</param>
        /// <param name="parameterType">The type to resolve.</param>
        /// <param name="ownerName">Name of the plugin or activity declaring the method.</param>
        /// <param name="methodName">Name of the invoked method.</param>
        /// <param name="parameterName">Name of the parameter being resolved.</param>
        /// <exception cref="InvalidPluginExecutionException">
        /// The container, or one of the dependencies it had to resolve in turn, holds no registration
        /// for the requested type.
        /// </exception>
        public static object Resolve(IObjectContainer container, Type parameterType, string ownerName, string methodName, string parameterName)
        {
            try
            {
                return container.Resolve(parameterType);
            }
            catch (ObjectContainerException ex)
            {
                throw new InvalidPluginExecutionException(
                    $"{ownerName}.{methodName} parameter : {parameterName}. {parameterType.Name} is not registered in the dependency container. " +
                    $"Register it in InternalDependencyProvider.RegisterCustomService, or declare it as an IService. Container details : {ex.Message}",
                    ex);
            }
        }
    }
}
