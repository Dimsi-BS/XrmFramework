// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;

namespace XrmFramework.DeployUtils.Tests.TableSync.Fixtures;

/// <summary>
/// Localisation des fichiers du dépôt depuis le répertoire de sortie des tests, pour les
/// tests qui s'exécutent sur les ressources réellement livrées par le framework.
/// </summary>
public static class RepositoryPaths
{
    /// <summary>Répertoire des fichiers .table livrés avec le package XrmFramework.</summary>
    public static string ShippedDefinitionsDirectory
        => FindDirectory(Path.Combine("src", "XrmFramework", "Definitions"));

    /// <summary>
    /// Remonte depuis le répertoire de sortie des tests jusqu'à trouver la racine du dépôt,
    /// identifiée par la présence du chemin relatif recherché.
    /// </summary>
    public static string FindDirectory(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory != null)
        {
            var candidate = Path.Combine(directory.FullName, relativePath);
            if (Directory.Exists(candidate))
                return candidate;

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            $"Impossible de localiser « {relativePath} » en remontant depuis {AppContext.BaseDirectory}.");
    }
}
