// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using NUnit.Framework;
using XrmFramework.DeployUtils.Comparers;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Model.Interfaces;

namespace XrmFramework.DeployUtils.Tests.Comparers;

[TestFixture]
public class CrmComponentComparerTests
{
    private CrmComponentComparer _comparer = null!;

    [SetUp]
    public void SetUp()
    {
        _comparer = new CrmComponentComparer();
    }

    // Helper
    private static Step MakeStep(
        string pluginTypeName = "MyPlugin",
        string pluginTypeFullName = "Assembly.MyPlugin",
        string entityName = "contact",
        Messages message = null!,
        Stages stage = Stages.PreOperation,
        Modes mode = Modes.Synchronous)
    {
        message ??= Messages.Create;
        return new Step(pluginTypeName, message, stage, mode, entityName)
        {
            PluginTypeFullName = pluginTypeFullName
        };
    }

    private static Plugin MakePlugin(string fullName = "Assembly.MyPlugin")
        => new Plugin(fullName);

    // ──────────────────────────────────────────────
    //  Equals — Steps
    // ──────────────────────────────────────────────

    [Test]
    public void Equals_IdenticalSteps_ReturnsTrue()
    {
        var x = MakeStep();
        var y = MakeStep();

        Assert.IsTrue(_comparer.Equals(x, y));
    }

    [Test]
    public void Equals_DifferentStepMessage_ReturnsFalse()
    {
        var x = MakeStep(message: Messages.Create);
        var y = MakeStep(message: Messages.Update);

        Assert.IsFalse(_comparer.Equals(x, y));
    }

    // ──────────────────────────────────────────────
    //  Equals — Plugins
    // ──────────────────────────────────────────────

    [Test]
    public void Equals_IdenticalPlugins_ReturnsTrue()
    {
        var x = MakePlugin("Assembly.MyPlugin");
        var y = MakePlugin("Assembly.MyPlugin");

        Assert.IsTrue(_comparer.Equals(x, y));
    }

    [Test]
    public void Equals_DifferentPluginFullName_ReturnsFalse()
    {
        var x = MakePlugin("Assembly.PluginA");
        var y = MakePlugin("Assembly.PluginB");

        Assert.IsFalse(_comparer.Equals(x, y));
    }

    // ──────────────────────────────────────────────
    //  Equals — types différents
    // ──────────────────────────────────────────────

    [Test]
    public void Equals_DifferentTypes_ReturnsFalse()
    {
        ICrmComponent step = MakeStep();
        ICrmComponent plugin = MakePlugin();

        Assert.IsFalse(_comparer.Equals(step, plugin));
    }

    [Test]
    public void Equals_XNull_ReturnsFalse()
    {
        Assert.IsFalse(_comparer.Equals(null!, MakePlugin()));
    }

    [Test]
    public void Equals_YNull_ReturnsFalse()
    {
        Assert.IsFalse(_comparer.Equals(MakePlugin(), null!));
    }

    // ──────────────────────────────────────────────
    //  NeedsUpdate — Steps
    // ──────────────────────────────────────────────

    [Test]
    public void NeedsUpdate_IdenticalSteps_ReturnsFalse()
    {
        var x = MakeStep();
        var y = MakeStep();

        Assert.IsFalse(_comparer.NeedsUpdate(x, y));
    }

    [Test]
    public void NeedsUpdate_StepDifferentOrder_ReturnsTrue()
    {
        var x = MakeStep();
        x.Order = 1;

        var y = MakeStep();
        y.Order = 5;

        Assert.IsTrue(_comparer.NeedsUpdate(x, y));
    }

    [Test]
    public void NeedsUpdate_DifferentComponents_ReturnsFalse()
    {
        // Non-equal components should not trigger an update
        var x = MakeStep(pluginTypeName: "PluginA");
        var y = MakeStep(pluginTypeName: "PluginB");

        Assert.IsFalse(_comparer.NeedsUpdate(x, y));
    }

    // ──────────────────────────────────────────────
    //  NeedsUpdate — CustomApi
    // ──────────────────────────────────────────────

    [Test]
    public void NeedsUpdate_IdenticalCustomApi_ReturnsFalse()
    {
        var x = new CustomApi
        {
            BindingType = new OptionSetValue(0),
            BoundEntityLogicalName = "contact",
            IsFunction = false,
            WorkflowSdkStepEnabled = false,
            AllowedCustomProcessingStepType = new OptionSetValue(0)
        };
        x.UniqueName = "new_MyApi";

        var y = new CustomApi
        {
            BindingType = new OptionSetValue(0),
            BoundEntityLogicalName = "contact",
            IsFunction = false,
            WorkflowSdkStepEnabled = false,
            AllowedCustomProcessingStepType = new OptionSetValue(0)
        };
        y.UniqueName = "new_MyApi";

        Assert.IsFalse(_comparer.NeedsUpdate(x, y));
    }

    [Test]
    public void NeedsUpdate_CustomApiDifferentIsFunction_ReturnsTrue()
    {
        var x = new CustomApi
        {
            BindingType = new OptionSetValue(0),
            IsFunction = true,
            WorkflowSdkStepEnabled = false,
            AllowedCustomProcessingStepType = new OptionSetValue(0)
        };
        x.UniqueName = "new_MyApi";

        var y = new CustomApi
        {
            BindingType = new OptionSetValue(0),
            IsFunction = false,
            WorkflowSdkStepEnabled = false,
            AllowedCustomProcessingStepType = new OptionSetValue(0)
        };
        y.UniqueName = "new_MyApi";

        Assert.IsTrue(_comparer.NeedsUpdate(x, y));
    }

    // ──────────────────────────────────────────────
    //  CorrespondingComponent
    // ──────────────────────────────────────────────

    [Test]
    public void CorrespondingComponent_MatchExists_ReturnsMatch()
    {
        var target = MakeStep();
        var collection = new List<ICrmComponent> { target };
        var query = MakeStep(); // identical to target

        var result = _comparer.CorrespondingComponent(query, collection);

        Assert.AreSame(target, result);
    }

    [Test]
    public void CorrespondingComponent_NoMatch_ReturnsNull()
    {
        var target = MakeStep(entityName: "contact");
        var collection = new List<ICrmComponent> { target };
        var query = MakeStep(entityName: "account"); // different entity

        var result = _comparer.CorrespondingComponent(query, collection);

        Assert.IsNull(result);
    }

    [Test]
    public void CorrespondingComponent_EmptyCollection_ReturnsNull()
    {
        var collection = new List<ICrmComponent>();
        var query = MakeStep();

        var result = _comparer.CorrespondingComponent(query, collection);

        Assert.IsNull(result);
    }
}
