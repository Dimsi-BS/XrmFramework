// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace XrmFramework
{
    /// <summary>
    /// Interface marqueur pour les services internes XrmFramework injectables
    /// comme paramètres de méthodes de plugins et d'activités workflow.
    /// <para>
    /// Tout service implémentant cette interface peut être déclaré comme paramètre
    /// d'une méthode de plugin ou d'activité custom workflow et sera résolu
    /// automatiquement depuis le conteneur de dépendances.
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
