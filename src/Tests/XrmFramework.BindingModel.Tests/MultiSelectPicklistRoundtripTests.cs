// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using NUnit.Framework;
using XrmFramework.BindingModel.Tests.Fakes;

namespace XrmFramework.BindingModel.Tests
{
    /// <summary>
    /// A multi-select choice read and written through a hand-written binding model, over the
    /// definitions this project generates from <c>Definitions/*.table</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// End to end on purpose. The mappers always handled <c>MultiSelectPicklist</c>; what broke was
    /// the table generator never emitting it — a column written <c>"Type": "Picklist"</c> with
    /// <c>"IsMultiSelect": true</c> came out declared a plain <c>Picklist</c>, which routed the read
    /// to <c>Entity.GetAttributeValue&lt;OptionSetValue&gt;</c> and threw
    /// <c>InvalidCastException: Unable to cast object of type 'OptionSetValueCollection' to type
    /// 'OptionSetValue'</c> out of the middle of <c>RetrieveAll&lt;T&gt;</c>.
    /// </para>
    /// <para>
    /// So these tests go through the generator rather than a hand-written definition: a fake
    /// carrying <c>[AttributeMetadata(AttributeTypeCode.MultiSelectPicklist)]</c> would assert the
    /// mapper's half, which was never the broken one.
    /// </para>
    /// </remarks>
    [TestFixture]
    public class MultiSelectPicklistRoundtripTests
    {
        private static readonly IOrganizationService NullService = null!;

        private static Entity CreateActivity()
            => new(MultiSelectActivityDefinition.EntityName, Guid.NewGuid());

        // ──────────────────────────────────────────────────────────────────────
        // What the table generator declares
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>
        /// The one assertion the whole bug comes down to: the generated definition says
        /// <c>MultiSelectPicklist</c>, which is what both mappers branch on.
        /// </summary>
        [Test]
        public void TheGeneratedDefinition_DeclaresTheColumnMultiSelectPicklist()
        {
            var definition = DefinitionCache.GetEntityDefinition(MultiSelectActivityDefinition.EntityName);

            Assert.AreEqual(
                AttributeTypeCode.MultiSelectPicklist,
                definition.GetAttributeType(MultiSelectActivityDefinition.Columns.ReportTypes),
                "A Picklist column flagged IsMultiSelect holds an OptionSetValueCollection.");
        }

        /// <summary>
        /// And the shape an older pull wrote — <c>Virtual</c> next to the same flag — reaches the
        /// same declaration, so no project has to re-pull to be fixed.
        /// </summary>
        [Test]
        public void AVirtualColumnFlaggedMultiSelect_IsDeclaredTheSameWay()
        {
            var definition = DefinitionCache.GetEntityDefinition(MultiSelectProjectDefinition.EntityName);

            Assert.AreEqual(
                AttributeTypeCode.MultiSelectPicklist,
                definition.GetAttributeType(MultiSelectProjectDefinition.Columns.BillingTypes));
        }

        /// <summary>A single-option choice on the same tables is untouched.</summary>
        [Test]
        public void AColumnThatIsNotMultiSelect_KeepsItsOwnType()
        {
            var definition = DefinitionCache.GetEntityDefinition(MultiSelectActivityDefinition.EntityName);

            Assert.AreEqual(
                AttributeTypeCode.String,
                definition.GetAttributeType(MultiSelectActivityDefinition.Columns.Name));
        }

        // ──────────────────────────────────────────────────────────────────────
        // Reading
        // ──────────────────────────────────────────────────────────────────────

        [Test]
        public void ReadingAMultiSelectColumn_FillsAnIntCollection()
        {
            var entity = CreateActivity();
            entity[MultiSelectActivityDefinition.Columns.ReportTypes] =
                new OptionSetValueCollection(new List<OptionSetValue> { new(1), new(3) });

            var model = entity.ToBindingModel<MultiSelectActivityModel>();

            Assert.IsNotNull(model);
            Assert.That(model!.ReportTypes, Is.EquivalentTo(new[] { 1, 3 }));
        }

        /// <summary>
        /// The same read into the enum the generator emits for the column. It needs a genuinely
        /// typed list: a <c>List&lt;object&gt;</c> of boxed members is assignable to no property
        /// anyone declares, which used to leave <c>ICollection&lt;int&gt;</c> as the only usable
        /// type.
        /// </summary>
        [Test]
        public void ReadingAMultiSelectColumn_FillsAnEnumCollection()
        {
            var entity = CreateActivity();
            entity[MultiSelectActivityDefinition.Columns.ReportTypes] =
                new OptionSetValueCollection(new List<OptionSetValue> { new(2), new(3) });

            var model = entity.ToBindingModel<MultiSelectActivityEnumModel>();

            Assert.IsNotNull(model);
            Assert.That(model!.ReportTypes, Is.EquivalentTo(
                new[] { MultiSelectReportType.Client, MultiSelectReportType.Audit }));
        }

        [Test]
        public void ReadingAnEmptyMultiSelectColumn_LeavesTheCollectionNull()
        {
            var entity = CreateActivity();
            entity[MultiSelectActivityDefinition.Columns.ReportTypes] = null;

            var model = entity.ToBindingModel<MultiSelectActivityModel>();

            Assert.IsNotNull(model);
            Assert.IsNull(model!.ReportTypes);
        }

        /// <summary>
        /// The multi-select on the table at the far end of a lookup, as a query linking to it brings
        /// it back: an <see cref="AliasedValue" /> under <c>&lt;lookup&gt;.&lt;column&gt;</c>.
        /// </summary>
        [Test]
        public void ReadingAMultiSelectColumnThroughALinkEntity_FillsAnIntCollection()
        {
            var entity = CreateActivity();
            const string aliased = MultiSelectActivityDefinition.Columns.ProjectId
                                 + "." + MultiSelectProjectDefinition.Columns.BillingTypes;

            // The lookup itself, as the query that linked through it brings it back: the mapper
            // reads a property only for an attribute the entity carries, and the attribute this
            // property maps to is the lookup, not the alias.
            entity[MultiSelectActivityDefinition.Columns.ProjectId] = new EntityReference(
                MultiSelectProjectDefinition.EntityName, Guid.NewGuid());

            entity[aliased] = new AliasedValue(
                MultiSelectProjectDefinition.EntityName,
                MultiSelectProjectDefinition.Columns.BillingTypes,
                new OptionSetValueCollection(new List<OptionSetValue> { new(10), new(20) }));

            var model = entity.ToBindingModel<MultiSelectActivityModel>();

            Assert.IsNotNull(model);
            Assert.That(model!.ProjectBillingTypes, Is.EquivalentTo(new[] { 10, 20 }));
        }

        // ──────────────────────────────────────────────────────────────────────
        // Writing
        // ──────────────────────────────────────────────────────────────────────

        [Test]
        public void WritingAnIntCollection_SetsAnOptionSetValueCollection()
        {
            var model = new MultiSelectActivityModel
            {
                Id = Guid.NewGuid(),
                ReportTypes = new List<int> { 1, 2 }
            };

            var entity = model.ToEntity(NullService);

            var written = entity.GetAttributeValue<OptionSetValueCollection>(
                MultiSelectActivityDefinition.Columns.ReportTypes);

            Assert.IsNotNull(written);
            Assert.That(written.Select(o => o.Value), Is.EquivalentTo(new[] { 1, 2 }));
        }

        /// <summary>
        /// The same write from the enum-typed property. The element type is what says whether a
        /// value is an enum member — asking the property type, which is the collection, answered no
        /// every time and then unboxed a member straight to <see cref="int" />, which throws.
        /// </summary>
        [Test]
        public void WritingAnEnumCollection_SetsAnOptionSetValueCollection()
        {
            var model = new MultiSelectActivityEnumModel
            {
                Id = Guid.NewGuid(),
                ReportTypes = new List<MultiSelectReportType>
                {
                    MultiSelectReportType.Internal,
                    MultiSelectReportType.Audit
                }
            };

            var entity = model.ToEntity(NullService);

            var written = entity.GetAttributeValue<OptionSetValueCollection>(
                MultiSelectActivityDefinition.Columns.ReportTypes);

            Assert.IsNotNull(written);
            Assert.That(written.Select(o => o.Value), Is.EquivalentTo(new[] { 1, 3 }));
        }

        /// <summary>Read, write, read: the options survive the round trip unchanged.</summary>
        [Test]
        public void AMultiSelectColumn_SurvivesARoundTrip()
        {
            var entity = CreateActivity();
            entity[MultiSelectActivityDefinition.Columns.ReportTypes] =
                new OptionSetValueCollection(new List<OptionSetValue> { new(1), new(2), new(3) });

            var model = entity.ToBindingModel<MultiSelectActivityModel>();
            var written = model!.ToEntity(NullService);
            var reread = written.ToBindingModel<MultiSelectActivityModel>();

            Assert.That(reread!.ReportTypes, Is.EquivalentTo(new[] { 1, 2, 3 }));
        }
    }
}
