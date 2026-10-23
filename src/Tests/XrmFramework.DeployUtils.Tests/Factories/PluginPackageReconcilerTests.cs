// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Linq;
using NUnit.Framework;
using XrmFramework.DeployUtils.Factories;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Tests.Factories;

[TestFixture]
public class PluginPackageReconcilerTests
{
    private static Plugin PluginWithStep(string fullName, bool withPreImage = false)
    {
        var plugin = new Plugin(fullName);
        var step = new Step(fullName.Split('.').Last(), Messages.Update, Stages.PostOperation,
            Modes.Synchronous, "account");

        if (withPreImage)
        {
            step.PreImage.AllAttributes = true;
        }

        plugin.AddChild(step);
        return plugin;
    }

    /// <summary>
    /// A plugin type still carried by the package keeps its steps: they are the normal diff's job.
    /// </summary>
    [Test]
    public void KeptPluginTypes_AreLeftAlone()
    {
        var registered = new[] {PluginWithStep("MyProject.Plugins.OnAccountUpdate")};

        var orphaned = PluginPackageReconciler.FindStepsOfRemovedPluginTypes(
            registered, new[] {"MyProject.Plugins.OnAccountUpdate"});

        Assert.That(orphaned, Is.Empty);
    }

    /// <summary>
    /// A plugin type the new package drops has its steps returned for deletion. Without this, the
    /// package update is rejected by the platform and the deployment stops with nothing applied.
    /// </summary>
    [Test]
    public void RemovedPluginTypes_YieldTheirSteps()
    {
        var kept = PluginWithStep("MyProject.Plugins.OnAccountUpdate");
        var removed = PluginWithStep("MyProject.Plugins.OnContactCreate");

        var orphaned = PluginPackageReconciler.FindStepsOfRemovedPluginTypes(
            new[] {kept, removed}, new[] {"MyProject.Plugins.OnAccountUpdate"});

        Assert.Multiple(() =>
        {
            Assert.That(orphaned.OfType<Step>().Count(), Is.EqualTo(1));
            Assert.That(orphaned, Does.Not.Contain(kept.Steps.Single()));
            Assert.That(orphaned, Contains.Item(removed.Steps.Single()));
        });
    }

    /// <summary>
    /// Images go with the step they belong to, and before it: a step deleted without its images
    /// leaves rows the next deployment would have to clean up.
    /// </summary>
    [Test]
    public void RemovedPluginTypes_YieldImagesBeforeTheirStep()
    {
        var removed = PluginWithStep("MyProject.Plugins.OnContactCreate", withPreImage: true);
        var step = removed.Steps.Single();

        Assume.That(step.Children, Is.Not.Empty, "the step should carry a pre-image");

        var orphaned = PluginPackageReconciler.FindStepsOfRemovedPluginTypes(
            new[] {removed}, Array.Empty<string>());

        Assert.That(orphaned.Last(), Is.SameAs(step));
        Assert.That(orphaned, Has.Count.EqualTo(1 + step.Children.Count()));
    }

    /// <summary>
    /// Comparison is case-sensitive and exact, like .NET type names: a near-match is a different
    /// type, and treating it as the same one would strand its steps.
    /// </summary>
    [Test]
    public void TypeNameMatching_IsExact()
    {
        var registered = new[] {PluginWithStep("MyProject.Plugins.OnAccountUpdate")};

        var orphaned = PluginPackageReconciler.FindStepsOfRemovedPluginTypes(
            registered, new[] {"myproject.plugins.onaccountupdate"});

        Assert.That(orphaned, Is.Not.Empty);
    }

    /// <summary>
    /// First deployment: nothing is registered yet, so there is nothing to unregister.
    /// </summary>
    [Test]
    public void NothingRegistered_YieldsNothing()
    {
        Assert.Multiple(() =>
        {
            Assert.That(PluginPackageReconciler.FindStepsOfRemovedPluginTypes(null, new[] {"A"}), Is.Empty);
            Assert.That(
                PluginPackageReconciler.FindStepsOfRemovedPluginTypes(Array.Empty<Plugin>(), new[] {"A"}),
                Is.Empty);
        });
    }
}
