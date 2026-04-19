// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace XrmFramework
{
    /// <summary>
    /// Implémentation de <see cref="IDateTimeProvider"/> qui retourne une date fixe.
    /// Utilisée lors du rejouage des sessions de test pour garantir la reproductibilité
    /// des calculs de dates relatives (ex : "ajouter 3 jours à maintenant").
    /// </summary>
    public class FixedDateTimeProvider : IDateTimeProvider
    {
        private readonly DateTime _utcNow;

        /// <summary>
        /// Initialise le fournisseur avec la date d'exécution enregistrée.
        /// </summary>
        /// <param name="executionDate">
        /// Date d'exécution originale. Peut être locale ou UTC ;
        /// elle est normalisée en UTC en interne.
        /// </param>
        public FixedDateTimeProvider(DateTime executionDate)
        {
            _utcNow = executionDate.Kind == DateTimeKind.Utc
                ? executionDate
                : executionDate.ToUniversalTime();
        }

        /// <inheritdoc />
        /// <remarks>Retourne l'heure locale correspondant à la date d'exécution enregistrée.</remarks>
        public DateTime Now => _utcNow.ToLocalTime();

        /// <inheritdoc />
        /// <remarks>Retourne la date UTC d'exécution enregistrée.</remarks>
        public DateTime UtcNow => _utcNow;
    }
}
