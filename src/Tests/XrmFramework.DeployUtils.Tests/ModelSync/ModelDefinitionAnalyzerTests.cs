// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Linq;
using System.Reflection;
using NUnit.Framework;
using XrmFramework.Core;
using XrmFramework.DeployUtils.ModelSync;
using XrmFramework.DeployUtils.Tests.ModelSync.Fixtures;
using XrmFramework.Model;
using CoreModel = XrmFramework.Core.Model;

namespace XrmFramework.DeployUtils.Tests.ModelSync;

[TestFixture]
public class ModelDefinitionAnalyzerTests
{
    /// <summary>
    /// The assembly containing our local fixtures. Used directly rather than loading an external
    /// .dll — ModelDefinitionAnalyzer.ExtractModels(Assembly) is the same code path either way.
    /// </summary>
    private static Assembly TestAssembly => typeof(ModelSyncTestContactModel).Assembly;

    private static CoreModel Contact()
        => ModelDefinitionAnalyzer.ExtractModels(TestAssembly)
            .Single(m => m.Name == nameof(ModelSyncTestContactModel));

    private static ModelProperty Prop(CoreModel model, string name)
        => model.Properties.Single(p => p.Name == name);

    // ──────────────────────────────────────────────────────────────────────────
    // Which classes get extracted at all
    // ──────────────────────────────────────────────────────────────────────────

    [Test]
    public void ExtractModels_FindsAClassImplementingIBindingModelWithCrmEntity()
    {
        var names = ModelDefinitionAnalyzer.ExtractModels(TestAssembly).Select(m => m.Name).ToList();

        Assert.That(names, Has.Member(nameof(ModelSyncTestContactModel)));
        Assert.That(names, Has.Member(nameof(ModelSyncTestContactExtraModel)));
    }

    [Test]
    public void ExtractModels_SkipsAClassWithNoCrmEntity()
    {
        var names = ModelDefinitionAnalyzer.ExtractModels(TestAssembly).Select(m => m.Name).ToList();

        Assert.That(names, Does.Not.Contain(nameof(ModelSyncTestPlainModel)));
    }

    /// <summary>Already produced by a source generator: nothing hand-written to recover.</summary>
    [Test]
    public void ExtractModels_SkipsAnAlreadyGeneratedClass()
    {
        var names = ModelDefinitionAnalyzer.ExtractModels(TestAssembly).Select(m => m.Name).ToList();

        Assert.That(names, Does.Not.Contain(nameof(ModelSyncTestGeneratedModel)));
    }

    /// <summary>
    /// ModelSourceFileGenerator always emits <c>partial class X : BindingModelBase</c>: a class
    /// extending a different hand-written base by hand (the shape of DIMSI-ERP's
    /// <c>NamingEntityModel</c>) cannot be converted without breaking the build, so it is left
    /// out of the result and reported as skipped instead.
    /// </summary>
    [Test]
    public void ExtractModels_SkipsAClassWithACustomHandWrittenBase()
    {
        var names = ModelDefinitionAnalyzer.ExtractModels(TestAssembly).Select(m => m.Name).ToList();

        Assert.That(names, Does.Not.Contain(nameof(ModelSyncTestCustomBaseModel)));
    }

    [Test]
    public void ExtractModels_ReportsAClassWithACustomHandWrittenBase_AsSkipped()
    {
        ModelDefinitionAnalyzer.ExtractModels(TestAssembly, out var skipped);

        var skip = skipped.Single(s => s.ClassName == nameof(ModelSyncTestCustomBaseModel));
        Assert.That(skip.Reason, Does.Contain(nameof(ModelSyncTestCustomBase)));
    }

    /// <summary>
    /// A class in the one namespace the framework itself ships a hand-written binding model under
    /// (<c>XrmFramework.Model</c>) is shipped straight into the consumer's own assembly via
    /// <c>contentFiles</c> — writing a <c>.model</c> file for it would regenerate an incomplete
    /// shadow of a class the framework already provides in full.
    /// </summary>
    [Test]
    public void ExtractModels_SkipsAClassInTheFrameworksOwnModelNamespace()
    {
        var names = ModelDefinitionAnalyzer.ExtractModels(TestAssembly).Select(m => m.Name).ToList();

        Assert.That(names, Does.Not.Contain(nameof(ModelSyncTestFrameworkShippedModel)));
    }

    [Test]
    public void ExtractModels_ReportsAFrameworkOwnedClass_AsSkipped()
    {
        ModelDefinitionAnalyzer.ExtractModels(TestAssembly, out var skipped);

        var skip = skipped.Single(s => s.ClassName == nameof(ModelSyncTestFrameworkShippedModel));
        Assert.That(skip.Reason, Does.Contain("XrmFramework.Model"));
    }

    // ──────────────────────────────────────────────────────────────────────────
    // [CrmEntity] — both forms
    // ──────────────────────────────────────────────────────────────────────────

    [Test]
    public void ExtractModels_ReadsTheEntityName_FromTheTypeofForm()
    {
        Assert.AreEqual(ModelSyncTestContactDefinition.EntityName, Contact().TableLogicalName);
    }

    [Test]
    public void ExtractModels_ReadsTheEntityName_FromTheStringForm()
    {
        var extra = ModelDefinitionAnalyzer.ExtractModels(TestAssembly)
            .Single(m => m.Name == nameof(ModelSyncTestContactExtraModel));

        Assert.AreEqual(ModelSyncTestContactDefinition.EntityName, extra.TableLogicalName);
    }

    [Test]
    public void ExtractModels_ReadsTheNamespace()
    {
        Assert.AreEqual(typeof(ModelSyncTestContactModel).Namespace, Contact().ModelNamespace);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // [CrmMapping]
    // ──────────────────────────────────────────────────────────────────────────

    [Test]
    public void ExtractModels_ReadsTheColumnAndType_OfAPlainMappedProperty()
    {
        var fullName = Prop(Contact(), nameof(ModelSyncTestContactModel.FullName));

        Assert.AreEqual(ModelSyncTestContactDefinition.Columns.FullName, fullName.LogicalName);
        Assert.AreEqual("string", fullName.TypeFullName);
    }

    [Test]
    public void ExtractModels_ReadsIsValidForUpdateFalse()
    {
        var email = Prop(Contact(), nameof(ModelSyncTestContactModel.Email));

        Assert.IsFalse(email.IsValidForUpdate);
    }

    [Test]
    public void ExtractModels_DefaultsIsValidForUpdateToTrue()
    {
        var fullName = Prop(Contact(), nameof(ModelSyncTestContactModel.FullName));

        Assert.IsTrue(fullName.IsValidForUpdate);
    }

    [Test]
    public void ExtractModels_ReadsFollowLink()
    {
        var accountName = Prop(Contact(), nameof(ModelSyncTestContactModel.AccountName));

        Assert.IsTrue(accountName.FollowLink);
    }

    [Test]
    public void ExtractModels_APropertyWithNoRecognizedAttribute_IsNotExtracted()
    {
        var names = Contact().Properties.Select(p => p.Name).ToList();

        Assert.That(names, Does.Not.Contain(nameof(ModelSyncTestContactModel.Computed)));
        Assert.That(names, Does.Not.Contain(nameof(ModelSyncTestContactModel.Id)));
    }

    // ──────────────────────────────────────────────────────────────────────────
    // [CrmLookup] — projection through the typeof(...) target form
    // ──────────────────────────────────────────────────────────────────────────

    [Test]
    public void ExtractModels_ReadsTheLookupTargetTable_FromTheTypeofForm()
    {
        var accountName = Prop(Contact(), nameof(ModelSyncTestContactModel.AccountName));

        Assert.AreEqual(ModelSyncTestAccountDefinition.EntityName, accountName.LookupTargetTableLogicalName);
    }

    [Test]
    public void ExtractModels_ReadsTheProjectedColumn()
    {
        var accountName = Prop(Contact(), nameof(ModelSyncTestContactModel.AccountName));

        Assert.AreEqual(ModelSyncTestAccountDefinition.Columns.Name, accountName.LookupTargetColumnLogicalName);
    }

    [Test]
    public void ExtractModels_AProjectedLookup_IsNotFlaggedAsAnEmbeddedModel()
    {
        var accountName = Prop(Contact(), nameof(ModelSyncTestContactModel.AccountName));

        Assert.IsFalse(accountName.LookupTargetModel);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // LookupTargetModel — no [CrmLookup], the property's own type is a binding model
    // ──────────────────────────────────────────────────────────────────────────

    [Test]
    public void ExtractModels_APropertyTypedAsABindingModel_WithNoCrmLookup_IsAnEmbeddedModel()
    {
        var parent = Prop(Contact(), nameof(ModelSyncTestContactModel.Parent));

        Assert.IsTrue(parent.LookupTargetModel);
        Assert.AreEqual(ModelSyncTestContactDefinition.Columns.ParentContactId, parent.LogicalName);
    }

    [Test]
    public void ExtractModels_TheEmbeddedModelType_IsFullyQualified()
    {
        var parent = Prop(Contact(), nameof(ModelSyncTestContactModel.Parent));

        Assert.AreEqual(typeof(ModelSyncTestContactModel).FullName, parent.TypeFullName);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // [ExtendBindingModel]
    // ──────────────────────────────────────────────────────────────────────────

    [Test]
    public void ExtractModels_AnExtensionProperty_CarriesNoColumn()
    {
        var extra = Prop(Contact(), nameof(ModelSyncTestContactModel.Extra));

        Assert.IsTrue(extra.ExtendBindingModel);
        Assert.IsNull(extra.LogicalName);
    }

    [Test]
    public void ExtractModels_AnExtensionProperty_ReadsItsFullyQualifiedType()
    {
        var extra = Prop(Contact(), nameof(ModelSyncTestContactModel.Extra));

        Assert.AreEqual(typeof(ModelSyncTestContactExtraModel).FullName, extra.TypeFullName);
    }

    [Test]
    public void ExtractModels_AnExtensionProperty_ReadsItsJsonPropertyName()
    {
        var extra = Prop(Contact(), nameof(ModelSyncTestContactModel.Extra));

        Assert.AreEqual("extra", extra.JsonPropertyName);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // [ChildRelationship]
    // ──────────────────────────────────────────────────────────────────────────

    [Test]
    public void ExtractModels_AChildRelationshipProperty_ReadsTheSchemaNameAsLogN()
    {
        var children = Prop(Contact(), nameof(ModelSyncTestContactModel.Children));

        Assert.AreEqual("msync_contact_children", children.LogicalName);
    }

    [Test]
    public void ExtractModels_AChildRelationshipProperty_ReadsTheListElementTypeFullyQualified()
    {
        var children = Prop(Contact(), nameof(ModelSyncTestContactModel.Children));

        Assert.AreEqual($"List<{typeof(ModelSyncTestContactModel).FullName}>", children.TypeFullName);
    }

    [Test]
    public void ExtractModels_AChildRelationshipProperty_ReadsItsIsValidForUpdate()
    {
        var children = Prop(Contact(), nameof(ModelSyncTestContactModel.Children));

        Assert.IsFalse(children.IsValidForUpdate);
    }

    /// <summary>
    /// A relationship exposed as <c>ICollection&lt;T&gt;</c> rather than the concrete <c>List&lt;T&gt;</c>
    /// — the shape <see cref="ModelDefinition.IsCollectionProperty"/> already accepts at runtime —
    /// must be extracted too, not silently dropped.
    /// </summary>
    [Test]
    public void ExtractModels_AnICollectionTypedChildRelationship_IsNotDropped()
    {
        var childrenReadOnly = Prop(Contact(), nameof(ModelSyncTestContactModel.ChildrenReadOnly));

        Assert.AreEqual("msync_contact_children_ro", childrenReadOnly.LogicalName);
        Assert.AreEqual($"List<{typeof(ModelSyncTestContactModel).FullName}>", childrenReadOnly.TypeFullName);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // [JsonProperty] / [JsonIgnore]
    // ──────────────────────────────────────────────────────────────────────────

    [Test]
    public void ExtractModels_ReadsTheJsonPropertyRename()
    {
        var email = Prop(Contact(), nameof(ModelSyncTestContactModel.Email));

        Assert.AreEqual("mail", email.JsonPropertyName);
    }
}
