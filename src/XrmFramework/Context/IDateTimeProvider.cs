// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace XrmFramework
{
    /// <summary>
    /// Fournit l'heure courante. Injectable comme paramètre de méthode dans les plugins
    /// et activités custom workflow pour permettre des tests unitaires déterministes.
    /// <para>
    /// En production, retourne l'heure système réelle. En rejouage de session de test,
    /// retourne la date d'exécution enregistrée, ce qui rend reproductibles les calculs
    /// de dates relatives (ex : "ajouter 3 jours à maintenant").
    /// </para>
    /// <example>
    /// Au lieu de :
    /// <code>
    /// var expiryDate = DateTime.UtcNow.AddDays(30);
    /// </code>
    /// Utiliser :
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
        /// Retourne la date et l'heure locales courantes.
        /// Équivalent déterministe de <see cref="DateTime.Now"/>.
        /// </summary>
        DateTime Now { get; }

        /// <summary>
        /// Retourne la date et l'heure UTC courantes.
        /// Équivalent déterministe de <see cref="DateTime.UtcNow"/>.
        /// </summary>
        DateTime UtcNow { get; }
    }
}
