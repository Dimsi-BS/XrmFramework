// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Deploy;
using Microsoft.Xrm.Sdk;
using Moq;
using NUnit.Framework;
using XrmFramework.DeployUtils.Configuration;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Converters;
using XrmFramework.DeployUtils.Exporters;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Model.Interfaces;
using XrmFramework.DeployUtils.Service;
using CustomApi = XrmFramework.DeployUtils.Model.CustomApi;

namespace XrmFramework.DeployUtils.Tests.Exporters;

/// <summary>
///     Covers the first deployment of an assembly into an organization, where the <c>pluginassembly</c>
///     row and everything that references it are created in the same batch.
/// </summary>
[TestFixture]
public class AssemblyExporterCreationTests
{
    private const string AssemblyName = "MyProject.Plugins";

    private readonly List<Entity> _created = new();
    private AssemblyExporter _exporter = null!;
    private Mock<IRegistrationService> _service = null!;

    [SetUp]
    public void SetUp()
    {
        _created.Clear();

        var solutionContext = new Mock<ISolutionContext>();
        solutionContext.SetupGet(c => c.SolutionName).Returns("MySolution");

        _service = new Mock<IRegistrationService>();

        // Dataverse assigns the id, which is the whole point: whatever the local model carried is
        // thrown away, so anything that references the created row has to be told the new id.
        _service.Setup(s => s.Create(It.IsAny<Entity>()))
            .Returns((Entity e) =>
            {
                _created.Add(e);
                return Guid.NewGuid();
            });

        _service.Setup(s => s.GetIntEntityTypeCode(It.IsAny<string>())).Returns(1);

        _exporter = new AssemblyExporter(solutionContext.Object, _service.Object,
            new CrmComponentConverter(solutionContext.Object, new CrmMapper()));
    }

    private static IAssemblyContext LocalAssembly()
    {
        var context = new AssemblyContext
        {
            AssemblyInfo = new AssemblyInfo
            {
                Name = AssemblyName,
                Version = "1.0.0.0",
                Culture = "neutral",
                PublicKeyToken = "null"
            }
        };

        context.AddChild(new Plugin($"{AssemblyName}.OnAccountUpdate"));

        context.AddChild(new CustomApi
        {
            FullName = $"{AssemblyName}.DoSomething",
            DisplayName = "Do Something",
            UniqueName = "dim_DoSomething"
        });

        return context;
    }

    private Guid CreatedAssemblyId()
        => _created.Single(e => e.LogicalName == PluginAssemblyDefinition.EntityName).Id;

    private IEnumerable<EntityReference> CreatedPluginTypeAssemblyRefs()
        => _created.Where(e => e.LogicalName == PluginTypeDefinition.EntityName)
            .Select(e => e.GetAttributeValue<EntityReference>(PluginTypeDefinition.Columns.PluginAssemblyId));

    /// <summary>
    ///     A plugin type is registered against its assembly by id. Nothing used to carry the id
    ///     Dataverse assigned to a freshly created <c>pluginassembly</c> row over to the plugins:
    ///     <see cref="AssemblyInfo" /> has no children of its own, and the only hand-over,
    ///     <see cref="IAssemblyContext.SetAssemblyId" />, is called by the diff - which on a first
    ///     deployment has no registered assembly to align with. Every plugin type was then created
    ///     against the id the local model had invented, and Dataverse answered
    ///     "Entity 'pluginassembly' With Id = ... Does Not Exist".
    /// </summary>
    [Test]
    public void FirstDeployment_RegistersPluginTypesAgainstTheCreatedAssembly()
    {
        var context = LocalAssembly();

        _exporter.CreateAllComponents(context, context.ComponentsOrderedPool);

        var assemblyId = CreatedAssemblyId();

        Assert.That(assemblyId, Is.Not.EqualTo(Guid.Empty));
        Assert.That(CreatedPluginTypeAssemblyRefs().Select(r => r.Id), Is.All.EqualTo(assemblyId));
    }

    /// <summary>
    ///     The plugin type backing a custom API is created on its own, off
    ///     <see cref="CustomApi.AssemblyId" />, and has to point at the created assembly too.
    /// </summary>
    [Test]
    public void FirstDeployment_HandsTheCreatedAssemblyIdToCustomApis()
    {
        var context = LocalAssembly();

        _exporter.CreateAllComponents(context, context.ComponentsOrderedPool);

        Assert.That(context.CustomApis.Select(c => c.AssemblyId), Is.All.EqualTo(CreatedAssemblyId()));
    }

    /// <summary>
    ///     The plugins and the assembly must agree, whichever way round it is read.
    /// </summary>
    [Test]
    public void FirstDeployment_HandsTheCreatedAssemblyIdToPlugins()
    {
        var context = LocalAssembly();

        _exporter.CreateAllComponents(context, context.ComponentsOrderedPool);

        Assert.That(context.AssemblyInfo.Id, Is.EqualTo(CreatedAssemblyId()));
        Assert.That(context.Plugins.Select(p => p.ParentId), Is.All.EqualTo(CreatedAssemblyId()));
    }

    /// <summary>
    ///     In package mode the <c>pluginassembly</c> row is Dataverse's, created out of the pushed
    ///     package and never part of the create batch. The ids the diff read off that registered row
    ///     are the right ones, so nothing must overwrite them.
    /// </summary>
    [Test]
    public void AssemblyOutOfTheBatch_LeavesTheRegisteredAssemblyIdAlone()
    {
        var context = new Mock<IAssemblyContext>();
        var registeredAssemblyId = Guid.NewGuid();

        var plugin = new Plugin($"{AssemblyName}.OnAccountUpdate") {ParentId = registeredAssemblyId};

        _exporter.CreateAllComponents(context.Object, new ICrmComponent[] {plugin});

        context.Verify(c => c.SetAssemblyId(It.IsAny<Guid>()), Times.Never);
        Assert.That(CreatedPluginTypeAssemblyRefs().Single().Id, Is.EqualTo(registeredAssemblyId));
    }
}
