// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Linq;
using Deploy;
using Microsoft.Xrm.Sdk;
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

    /// <summary>
    /// An assembly that is not registered yet has no context at all. Returning a context wrapping a
    /// null <see cref="AssemblyInfo" /> made the diff believe the assembly was deployed, and the
    /// first deployment of any assembly failed on reading its id.
    /// </summary>
    [Test]
    public void CreateAssemblyFromRemote_WithoutAssemblyInfo_ReturnsNoContext()
    {
        var importer = new AssemblyImporter(null, null);

        Assert.That(importer.CreateAssemblyFromRemote((AssemblyInfo) null), Is.Null);
    }

    /// <summary>
    /// A step registered on the server carries its own pre- and post-image. Writing both onto
    /// <see cref="Step.PreImage" /> left the post-image without an id, so the diff saw it as missing
    /// and registered a second post-image on every deployment.
    /// </summary>
    [Test]
    public void CreateStepImageFromRemote_FillsTheImageOfTheRequestedType()
    {
        var step = NewStep();
        var preImageId = Guid.NewGuid();
        var postImageId = Guid.NewGuid();
        var images = new[]
        {
            RemoteImage(preImageId, step.Id, sdkmessageprocessingstepimage_imagetype.PreImage, "firstname"),
            RemoteImage(postImageId, step.Id, sdkmessageprocessingstepimage_imagetype.PostImage, "lastname")
        };

        var importer = new AssemblyImporter(null, null);

        importer.CreateStepImageFromRemote(step, true, images);
        importer.CreateStepImageFromRemote(step, false, images);

        Assert.Multiple(() =>
        {
            Assert.That(step.PreImage.Id, Is.EqualTo(preImageId));
            Assert.That(step.PreImage.ParentId, Is.EqualTo(step.Id));
            Assert.That(step.PreImage.JoinedAttributes, Is.EqualTo("firstname"));
            Assert.That(step.PostImage.Id, Is.EqualTo(postImageId));
            Assert.That(step.PostImage.ParentId, Is.EqualTo(step.Id));
            Assert.That(step.PostImage.JoinedAttributes, Is.EqualTo("lastname"));
        });
    }

    /// <summary>
    /// An image registered without an attribute list covers every attribute: the imported image
    /// must say so, and hold no attribute of its own.
    /// </summary>
    [Test]
    public void CreateStepImageFromRemote_WithoutAttributeList_CoversAllAttributes()
    {
        var step = NewStep();
        var images = new[]
        {
            RemoteImage(Guid.NewGuid(), step.Id, sdkmessageprocessingstepimage_imagetype.PostImage, null)
        };

        new AssemblyImporter(null, null).CreateStepImageFromRemote(step, false, images);

        Assert.Multiple(() =>
        {
            Assert.That(step.PostImage.AllAttributes, Is.True);
            Assert.That(step.PostImage.Attributes, Is.Empty);
        });
    }

    /// <summary>
    /// The images are read for the whole assembly: an image of another step must be ignored, and the
    /// step's own image stay unused — an unused image is not exposed as a child of the step, which is
    /// how the diff knows it is not registered yet.
    /// </summary>
    [Test]
    public void CreateStepImageFromRemote_IgnoresTheImagesOfAnotherStep()
    {
        var step = NewStep();
        var images = new[]
        {
            RemoteImage(Guid.NewGuid(), Guid.NewGuid(), sdkmessageprocessingstepimage_imagetype.PostImage, "lastname")
        };

        new AssemblyImporter(null, null).CreateStepImageFromRemote(step, false, images);

        Assert.Multiple(() =>
        {
            Assert.That(step.PostImage.Attributes, Is.Empty);
            Assert.That(step.PostImage.AllAttributes, Is.False);
            Assert.That(step.PostImage.IsUsed, Is.False);
        });
    }

    private static Step NewStep() =>
        new("MyPlugin", Messages.Update, Stages.PostOperation, Modes.Synchronous, "contact")
        {
            Id = Guid.NewGuid()
        };

    private static SdkMessageProcessingStepImage RemoteImage(Guid id, Guid stepId,
        sdkmessageprocessingstepimage_imagetype imageType, string attributes) =>
        new()
        {
            Id = id,
            SdkMessageProcessingStepId = new EntityReference(SdkMessageProcessingStep.EntityLogicalName, stepId),
            ImageTypeEnum = imageType,
            Attributes1 = attributes
        };
}
