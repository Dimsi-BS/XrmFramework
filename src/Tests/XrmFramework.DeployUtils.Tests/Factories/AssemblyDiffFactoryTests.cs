// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Linq;
using NUnit.Framework;
using XrmFramework.DeployUtils.Comparers;
using XrmFramework.DeployUtils.Configuration;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Factories;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Model.Interfaces;

namespace XrmFramework.DeployUtils.Tests.Factories;

[TestFixture]
public class AssemblyDiffFactoryTests
{
    private AssemblyDiffFactory _factory = null!;

    [SetUp]
    public void SetUp()
    {
        _factory = new AssemblyDiffFactory(new CrmComponentComparer(), new CrmMapper());
    }

    private static IAssemblyContext LocalAssembly()
    {
        var context = new AssemblyContext
        {
            AssemblyInfo = new AssemblyInfo
            {
                Name = "MyProject.Plugins",
                Version = "1.0.0.0",
                Culture = "neutral",
                PublicKeyToken = "null"
            }
        };

        var plugin = new Plugin("MyProject.Plugins.OnAccountUpdate");
        plugin.AddChild(new Step("OnAccountUpdate", Messages.Update, Stages.PostOperation,
            Modes.Synchronous, "account")
        {
            PluginTypeFullName = plugin.FullName
        });

        context.AddChild(plugin);

        return context;
    }

    /// <summary>
    /// Nothing on the server: every local component has to be created.
    /// </summary>
    [Test]
    public void NoRemoteContext_FlagsEverythingToCreate()
    {
        var strategy = _factory.ComputeDiffPatch(LocalAssembly(), null);

        Assert.That(strategy.ComponentsOrderedPool.Select(c => c.RegistrationState),
            Is.All.EqualTo(RegistrationState.ToCreate));
    }

    /// <summary>
    /// The remote factory used to hand over a context without its <c>pluginassembly</c> row when the
    /// assembly had never been deployed. That was read as "already deployed" and the diff threw as
    /// soon as it reached the assembly id, so a first deployment could not go through.
    /// </summary>
    [Test]
    public void RemoteContextWithoutAssemblyRow_FlagsEverythingToCreate()
    {
        var notDeployed = new AssemblyContext {AssemblyInfo = null};

        var strategy = _factory.ComputeDiffPatch(LocalAssembly(), notDeployed);

        Assert.Multiple(() =>
        {
            Assert.That(strategy.AssemblyInfo, Is.Not.Null);
            Assert.That(strategy.ComponentsOrderedPool.Select(c => c.RegistrationState),
                Is.All.EqualTo(RegistrationState.ToCreate));
        });
    }

    /// <summary>
    /// An assembly already deployed with the same content: the ids travel from the remote side and
    /// nothing is created a second time.
    /// </summary>
    [Test]
    public void IdenticalRemoteAssembly_TransfersIdsAndCreatesNothing()
    {
        var local = LocalAssembly();
        var remote = LocalAssembly();
        remote.AssemblyInfo.Id = Guid.NewGuid();
        remote.Plugins.First().Id = Guid.NewGuid();

        var strategy = _factory.ComputeDiffPatch(local, remote);

        Assert.Multiple(() =>
        {
            Assert.That(strategy.AssemblyInfo.Id, Is.EqualTo(remote.AssemblyInfo.Id));
            Assert.That(strategy.Plugins.First().Id, Is.EqualTo(remote.Plugins.First().Id));
            Assert.That(strategy.ComponentsOrderedPool.Select(c => c.RegistrationState),
                Has.None.EqualTo(RegistrationState.ToCreate));
        });
    }
}
