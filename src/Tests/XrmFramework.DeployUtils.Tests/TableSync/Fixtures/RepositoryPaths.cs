// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;

namespace XrmFramework.DeployUtils.Tests.TableSync.Fixtures;

/// <summary>
/// Locates repository files from the test output directory, for tests that run
/// against resources actually shipped by the framework.
/// </summary>
public static class RepositoryPaths
{
    /// <summary>Directory of .table files shipped with the XrmFramework package.</summary>
    public static string ShippedDefinitionsDirectory
        => FindDirectory(Path.Combine("src", "XrmFramework", "Definitions"));

    /// <summary>
    /// Walks up from the test output directory until it finds the repository root,
    /// identified by the presence of the sought-after relative path.
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
            $"Unable to locate \"{relativePath}\" while walking up from {AppContext.BaseDirectory}.");
    }
}
