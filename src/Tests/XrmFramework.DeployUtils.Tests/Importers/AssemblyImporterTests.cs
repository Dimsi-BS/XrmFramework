// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Linq;
using NUnit.Framework;
using XrmFramework.DeployUtils.Importers;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Tests.Importers;

[TestFixture]
public class AssemblyImporterTests
{
    /// <summary>
    /// <see cref="AssemblyImporter.CreateAssemblyFromLocal(string)" /> must describe an assembly
    /// from its path (metadata), without loading it into the runtime — this is what allows
    /// the net8 CLI to describe a net462 plugin assembly.
    /// </summary>
    [Test]
    public void CreateAssemblyFromLocal_ReadsIdentityFromPath()
    {
        var assembly = typeof(AssemblyImporterTests).Assembly;
        var expected = assembly.GetName();
        var token = expected.GetPublicKeyToken();
        var expectedToken = token is { Length: > 0 }
            ? string.Concat(token.Select(b => b.ToString("x2")))
            : "null";

        // Dependencies are not used by CreateAssemblyFromLocal.
        var importer = new AssemblyImporter(null, null);

        var info = importer.CreateAssemblyFromLocal(assembly.Location);

        Assert.Multiple(() =>
        {
            Assert.That(info.Name, Is.EqualTo(expected.Name));
            Assert.That(info.Version, Is.EqualTo(expected.Version!.ToString()));
            Assert.That(info.Culture, Is.EqualTo("neutral"));
            Assert.That(info.PublicKeyToken, Is.EqualTo(expectedToken));
            Assert.That(info.Description, Is.EqualTo($"{expected.Name} plugin assembly"));
            Assert.That(info.Content, Is.Not.Null.And.Length.GreaterThan(0));
            Assert.That(info.SourceType, Is.EqualTo(SourceType.Database));
            Assert.That(info.IsolationMode, Is.EqualTo(IsolationMode.Sandbox));
        });
    }
}
