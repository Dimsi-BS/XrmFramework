// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace XrmFramework
{
    /// <summary>
    /// Implementation of <see cref="IDateTimeProvider"/> that delegates to the system clock.
    /// Used in production and during real-time remote debugging.
    /// </summary>
    public class SystemDateTimeProvider : IDateTimeProvider
    {
        /// <summary>Stateless singleton instance — safe to share across contexts.</summary>
        public static readonly SystemDateTimeProvider Instance = new SystemDateTimeProvider();

        /// <inheritdoc />
        public DateTime Now => DateTime.Now;

        /// <inheritdoc />
        public DateTime UtcNow => DateTime.UtcNow;

        /// <inheritdoc />
        public DateTime Today => DateTime.Today;
    }
}
