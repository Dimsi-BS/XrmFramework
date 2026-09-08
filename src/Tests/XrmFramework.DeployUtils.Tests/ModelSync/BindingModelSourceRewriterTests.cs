// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using NUnit.Framework;
using XrmFramework.DeployUtils.ModelSync;

namespace XrmFramework.DeployUtils.Tests.ModelSync;

/// <summary>
/// Stripping a hand-written binding model class of the properties <c>ModelSourceFileGenerator</c>
/// now re-emits from its <c>.model</c> file: <c>[CrmMapping]</c> / <c>[ChildRelationship]</c> /
/// <c>[ExtendBindingModel]</c> members must go, everything else the project wrote by hand must
/// survive.
/// </summary>
[TestFixture]
public class BindingModelSourceRewriterTests
{
    /// <summary>Nothing but mapped properties, not even a hand-declared Id: entirely redundant.</summary>
    private const string FullyMappedSource = """
        using XrmFramework;
        using XrmFramework.BindingModel;

        namespace MyProject.Core.Model
        {
            [CrmEntity(typeof(ContactDefinition))]
            public partial class ContactModel : BindingModelBase
            {
                [CrmMapping(ContactDefinition.Columns.FullName)]
                public string FullName { get; set; }

                [CrmMapping(ContactDefinition.Columns.Email, IsValidForUpdate = false)]
                public string Email { get; set; }
            }
        }
        """;

    /// <summary>The mapped shape plus a hand-declared Id — Id alone is enough to keep the class alive.</summary>
    private const string MappedWithIdSource = """
        using System;
        using XrmFramework;
        using XrmFramework.BindingModel;

        namespace MyProject.Core.Model
        {
            [CrmEntity(typeof(ContactDefinition))]
            public partial class ContactModel : IBindingModel
            {
                public Guid Id { get; set; }

                [CrmMapping(ContactDefinition.Columns.FullName)]
                public string FullName { get; set; }
            }
        }
        """;

    /// <summary>The same shape, plus a hand-written helper method — the case that survives.</summary>
    private const string HandEditedSource = """
        using System;
        using XrmFramework;
        using XrmFramework.BindingModel;

        namespace MyProject.Core.Model
        {
            [GeneratedCode("XrmFramework", "2.0")]
            [ExcludeFromCodeCoverage]
            [CrmEntity(typeof(ContactDefinition))]
            public class ContactModel : IBindingModel
            {
                public Guid Id { get; set; }

                [CrmMapping(ContactDefinition.Columns.FullName)]
                public string FullName { get; set; }

                [ExtendBindingModel]
                public ContactExtraModel Extra { get; set; }

                public string Display() => $"{FullName} <{Extra?.Label}>";
            }
        }
        """;

    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void FullyMappedClass_IsDeleted()
    {
        var result = BindingModelSourceRewriter.Rewrite(FullyMappedSource, "ContactModel");

        Assert.AreEqual(ModelSourceRewriteOutcome.Delete, result.Outcome);
        Assert.That(result.RemovedMembers, Has.Member("FullName"));
        Assert.That(result.RemovedMembers, Has.Member("Email"));
    }

    /// <summary>An unattributed Id — or any other hand-written member — is enough to keep the class alive.</summary>
    [Test]
    public void AHandDeclaredIdProperty_IsKept_AndKeepsTheClassAlive()
    {
        var result = BindingModelSourceRewriter.Rewrite(MappedWithIdSource, "ContactModel");

        Assert.That(result.KeptMembers, Has.Member("Id"));
        Assert.AreEqual(ModelSourceRewriteOutcome.Rewrite, result.Outcome);
        Assert.That(result.NewText, Does.Contain("public Guid Id { get; set; }"));
        Assert.That(result.NewText, Does.Not.Contain("FullName"));
    }

    [Test]
    public void HandEditedClass_RemovesOnlyTheMappedProperties()
    {
        var result = BindingModelSourceRewriter.Rewrite(HandEditedSource, "ContactModel");

        Assert.AreEqual(ModelSourceRewriteOutcome.Rewrite, result.Outcome);
        Assert.That(result.RemovedMembers, Has.Member("FullName"));
        Assert.That(result.RemovedMembers, Has.Member("Extra"));
        // The property *declarations* are gone; Display() keeps referencing FullName/Extra by
        // name, since the generated partial still supplies them — only the [CrmMapping] /
        // [ExtendBindingModel] property statements themselves must disappear.
        Assert.That(result.NewText, Does.Not.Contain("public string FullName"));
        Assert.That(result.NewText, Does.Not.Contain("public ContactExtraModel Extra"));
        Assert.That(result.NewText, Does.Contain("public string Display()"));
    }

    [Test]
    public void HandEditedClass_KeepsTheHandWrittenMethodAndId()
    {
        var result = BindingModelSourceRewriter.Rewrite(HandEditedSource, "ContactModel");

        Assert.That(result.KeptMembers, Has.Member("Id"));
        Assert.That(result.KeptMembers, Has.Member("Display"));
        Assert.That(result.NewText, Does.Contain("public string Display()"));
        Assert.That(result.NewText, Does.Contain("public Guid Id { get; set; }"));
    }

    [Test]
    public void HandEditedClass_BecomesPartial()
    {
        var result = BindingModelSourceRewriter.Rewrite(HandEditedSource, "ContactModel");

        Assert.That(result.NewText, Does.Contain("public partial class ContactModel"));
    }

    /// <summary>
    /// [CrmEntity] and the generator's own class attributes would collide with the generated
    /// partial's copy (CS0579, since none allows multiple use) if left in place.
    /// </summary>
    [Test]
    public void HandEditedClass_StripsTheGeneratorsOwnClassAttributes()
    {
        var result = BindingModelSourceRewriter.Rewrite(HandEditedSource, "ContactModel");

        Assert.That(result.NewText, Does.Not.Contain("CrmEntity"));
        Assert.That(result.NewText, Does.Not.Contain("GeneratedCode"));
        Assert.That(result.NewText, Does.Not.Contain("ExcludeFromCodeCoverage"));
    }

    [Test]
    public void AlreadyPartialClass_IsNotGivenASecondPartialKeyword()
    {
        // MappedWithIdSource's class is already declared partial.
        var result = BindingModelSourceRewriter.Rewrite(MappedWithIdSource, "ContactModel");

        Assert.AreEqual(ModelSourceRewriteOutcome.Rewrite, result.Outcome);
        Assert.That(result.NewText, Does.Not.Contain("partial partial"));
    }

    [Test]
    public void NoMatchingClass_IsSkipped()
    {
        var result = BindingModelSourceRewriter.Rewrite(HandEditedSource, "NoSuchModel");

        Assert.AreEqual(ModelSourceRewriteOutcome.Skipped, result.Outcome);
    }

    [Test]
    public void MatchingIsCaseSensitive()
    {
        var result = BindingModelSourceRewriter.Rewrite(HandEditedSource, "contactmodel");

        Assert.AreEqual(ModelSourceRewriteOutcome.Skipped, result.Outcome);
    }
}
