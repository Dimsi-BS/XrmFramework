// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using NUnit.Framework;
using XrmFramework.DeployUtils.Configuration;
using PluginPackage = XrmFramework.DeployUtils.Model.PluginPackage;

namespace XrmFramework.DeployUtils.Tests.Configuration;

[TestFixture]
public class PluginPackageMappingTests
{
    private static PluginPackage LocalPackage() => new()
    {
        Id = Guid.NewGuid(),
        Name = "dim_MyProject.Plugins",
        UniqueName = "dim_MyProject.Plugins",
        Version = "1.2.3.4",
        Content = new byte[] {1, 2, 3}
    };

    /// <summary>
    /// A first push is the only chance to set the name and the version, so they all travel.
    /// </summary>
    [Test]
    public void ToRemote_ForACreate_CarriesTheIdentity()
    {
        var local = LocalPackage();

        var remote = new CrmMapper().ToRemote(local);

        Assert.Multiple(() =>
        {
            Assert.That(remote.Name, Is.EqualTo(local.Name));
            Assert.That(remote.UniqueName, Is.EqualTo(local.UniqueName));
            Assert.That(remote.Version, Is.EqualTo(local.Version));
            Assert.That(remote.Content, Is.EqualTo(Convert.ToBase64String(local.Content)));
        });
    }

    /// <summary>
    /// The name and version of a plugin package are immutable once it exists: sending them back
    /// makes Dataverse reject the whole update, so an update carries the id and the content only.
    /// </summary>
    [Test]
    public void ToRemoteContentUpdate_SendsTheContentOnly()
    {
        var local = LocalPackage();

        var remote = new CrmMapper().ToRemoteContentUpdate(local);

        Assert.Multiple(() =>
        {
            Assert.That(remote.Id, Is.EqualTo(local.Id));
            Assert.That(remote.Content, Is.EqualTo(Convert.ToBase64String(local.Content)));

            Assert.That(remote.Contains(PluginPackageDefinition.Columns.Name), Is.False,
                "the name is immutable and must not be sent again");
            Assert.That(remote.Contains(PluginPackageDefinition.Columns.UniqueName), Is.False,
                "the unique name is immutable and must not be sent again");
            Assert.That(remote.Contains(PluginPackageDefinition.Columns.Version), Is.False,
                "the version is immutable and must not be sent again");
        });
    }

    [Test]
    public void ToRemoteContentUpdate_OfNull_IsNull()
    {
        Assert.That(new CrmMapper().ToRemoteContentUpdate(null), Is.Null);
    }
}
