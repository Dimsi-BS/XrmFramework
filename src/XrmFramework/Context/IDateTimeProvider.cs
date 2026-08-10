// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace XrmFramework
{
    /// <summary>
    /// Provides the current time. Injectable as a method parameter in plugins
    /// and custom workflow activities to enable deterministic unit tests.
    /// <para>
    /// In production, returns the real system time. During test session replay,
    /// returns the recorded execution date, which makes relative date calculations
    /// reproducible (e.g. "add 3 days to now").
    /// </para>
    /// <example>
    /// Instead of:
    /// <code>
    /// var expiryDate = DateTime.UtcNow.AddDays(30);
    /// </code>
    /// Use:
    /// <code>
    /// public void HandleCreate(IPluginContext context, IDateTimeProvider clock)
    /// {
    ///     var expiryDate = clock.UtcNow.AddDays(30);
    /// }
    /// </code>
    /// </example>
    /// </summary>
    public interface IDateTimeProvider : IXrmFrameworkService
    {
        /// <summary>
        /// Returns the current local date and time.
        /// Deterministic equivalent of <see cref="DateTime.Now"/>.
        /// </summary>
        DateTime Now { get; }

        /// <summary>
        /// Returns the current UTC date and time.
        /// Deterministic equivalent of <see cref="DateTime.UtcNow"/>.
        /// </summary>
        DateTime UtcNow { get; }

        /// <summary>
        /// Returns the current local date, with the time component set to 00:00:00.
        /// Deterministic equivalent of <see cref="DateTime.Today"/>.
        /// </summary>
        DateTime Today { get; }
    }
}
