// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Newtonsoft.Json;
using XrmFramework;
using XrmFramework.BindingModel;

namespace XrmFramework.DeployUtils.Tests.ModelSync.Fixtures
{
    // ──────────────────────────────────────────────────────────────────────────
    // ModelDefinitionAnalyzer matches attributes by SIMPLE NAME, but this test project already
    // references the real ones (via the linked sources XrmFramework.DeployUtils compiles in), so
    // they are used directly — CustomAttributeData is read the same way regardless.
    //
    // All fixtures are prefixed "ModelSyncTest" to avoid colliding with the TableSync fixtures
    // already declared in this test project.
    // ──────────────────────────────────────────────────────────────────────────

    public static class ModelSyncTestAccountDefinition
    {
        public const string EntityName = "msync_account";

        public static class Columns
        {
            public const string Id = "msync_accountid";
            public const string Name = "msync_name";
        }
    }

    public static class ModelSyncTestContactDefinition
    {
        public const string EntityName = "msync_contact";

        public static class Columns
        {
            public const string Id = "msync_contactid";
            public const string FullName = "msync_fullname";
            public const string Email = "msync_email";
            public const string AccountId = "msync_accountid_lookup";
            public const string ParentContactId = "msync_parentcontactid";
        }
    }

    /// <summary>The hand-written class every ModelDefinitionAnalyzerTests case reads from.</summary>
    [CrmEntity(typeof(ModelSyncTestContactDefinition))]
    public partial class ModelSyncTestContactModel : IBindingModel
    {
        public Guid Id { get; set; }

        [CrmMapping(ModelSyncTestContactDefinition.Columns.FullName)]
        public string FullName { get; set; }

        [CrmMapping(ModelSyncTestContactDefinition.Columns.Email, IsValidForUpdate = false)]
        [JsonProperty("mail")]
        public string Email { get; set; }

        // A projected lookup, disambiguated through the typeof(...) form of [CrmLookup].
        [CrmMapping(ModelSyncTestContactDefinition.Columns.AccountId, FollowLink = true)]
        [CrmLookup(typeof(ModelSyncTestAccountDefinition), ModelSyncTestAccountDefinition.Columns.Name)]
        public string AccountName { get; set; }

        // No [CrmLookup]: the property's own type is a binding model, so the target comes from its
        // [CrmEntity] instead — LookupTargetModel.
        [CrmMapping(ModelSyncTestContactDefinition.Columns.ParentContactId)]
        public ModelSyncTestContactModel Parent { get; set; }

        [ExtendBindingModel]
        [JsonProperty("extra")]
        public ModelSyncTestContactExtraModel Extra { get; set; }

        [ChildRelationship("msync_contact_children", IsValidForUpdate = false)]
        public List<ModelSyncTestContactModel> Children { get; set; }

        // ICollection<T>, not List<T> — a read-only-from-the-outside, initialized-once collection is
        // a common shape for a relationship property; ModelDefinitionAnalyzer must not silently drop
        // it the way List<T>-only detection would.
        [ChildRelationship("msync_contact_children_ro")]
        public ICollection<ModelSyncTestContactModel> ChildrenReadOnly { get; } = new List<ModelSyncTestContactModel>();

        // No [CrmMapping]/[ExtendBindingModel]/[ChildRelationship]: not part of the mapping, even
        // though it carries an attribute ModelDefinitionAnalyzer does look at elsewhere.
        [JsonIgnore]
        public string Computed => FullName + " <" + Email + ">";
    }

    /// <summary>Nested over the same record as <see cref="ModelSyncTestContactModel"/> — an extension target.</summary>
    [CrmEntity(ModelSyncTestContactDefinition.EntityName)] // the string form, for contrast with typeof(...)
    public partial class ModelSyncTestContactExtraModel : IBindingModel
    {
        public Guid Id { get; set; }

        [CrmMapping(ModelSyncTestContactDefinition.Columns.FullName)]
        public string FullName { get; set; }
    }

    /// <summary>Already produced by a source generator: ModelDefinitionAnalyzer must skip it.</summary>
    [GeneratedCode("XrmFramework", "2.0")]
    [CrmEntity(typeof(ModelSyncTestContactDefinition))]
    public partial class ModelSyncTestGeneratedModel : IBindingModel
    {
        public Guid Id { get; set; }
    }

    /// <summary>No [CrmEntity]: not a mapped binding model, even though it implements the interface.</summary>
    public class ModelSyncTestPlainModel : IBindingModel
    {
        public Guid Id { get; set; }
    }

    /// <summary>
    /// A project's own shared base — the shape of DIMSI-ERP's <c>NamingEntityModel</c>: hand-written,
    /// implements <see cref="IBindingModel"/> directly, not <c>BindingModelBase</c>.
    /// </summary>
    public abstract class ModelSyncTestCustomBase : IBindingModel
    {
        public Guid Id { get; set; }

        public abstract string DisplayName { get; }
    }

    /// <summary>
    /// Extends a hand-written base other than <c>BindingModelBase</c>: <c>ModelSourceFileGenerator</c>
    /// always emits <c>: BindingModelBase</c>, so converting this to a <c>.model</c> file would break
    /// the build (CS0263) rather than migrate it — ModelDefinitionAnalyzer must skip it instead.
    /// </summary>
    [CrmEntity(typeof(ModelSyncTestContactDefinition))]
    public partial class ModelSyncTestCustomBaseModel : ModelSyncTestCustomBase
    {
        [CrmMapping(ModelSyncTestContactDefinition.Columns.FullName)]
        public string FullName { get; set; }

        public override string DisplayName => FullName;
    }
}

namespace XrmFramework.Model
{
    // ──────────────────────────────────────────────────────────────────────────
    // The one real namespace the framework itself ships a hand-written binding model under
    // (EnvironmentVariable, at src/XrmFramework/Model/EnvironmentVariable.cs) — XrmFramework's own
    // .nuspec ships its **\*.cs as contentFiles, compiled directly into every consuming project's
    // assembly, so a fixture declared right here reproduces the shape ModelDefinitionAnalyzer must
    // recognize and skip.
    // ──────────────────────────────────────────────────────────────────────────

    [CrmEntity("msync_frameworkshipped")]
    public partial class ModelSyncTestFrameworkShippedModel : BindingModelBase
    {
        [CrmMapping("msync_name")]
        public string Name { get; set; }
    }
}
