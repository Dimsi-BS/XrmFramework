// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace XrmFramework
{
    /// <summary>
    /// Implémentation de <see cref="IDateTimeProvider"/> qui délègue à l'horloge système.
    /// Utilisée en production et lors du débogage distant en temps réel.
    /// </summary>
    public class SystemDateTimeProvider : IDateTimeProvider
    {
        /// <summary>Instance singleton sans état — partage possible entre contextes.</summary>
        public static readonly SystemDateTimeProvider Instance = new SystemDateTimeProvider();

        /// <inheritdoc />
        public DateTime Now => DateTime.Now;

        /// <inheritdoc />
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
