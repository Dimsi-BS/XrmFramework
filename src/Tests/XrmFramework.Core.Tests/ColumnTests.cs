using Newtonsoft.Json;
using NUnit.Framework;
using Microsoft.Xrm.Sdk.Metadata;

namespace XrmFramework.Core.Tests
{
    [TestFixture]
    public class ColumnTests
    {
        [Test]
        public void DeserializeDatBehav_CurrentIntegerShape()
        {
            var column = JsonConvert.DeserializeObject<Column>("{ \"DatBehav\": 1 }");

            Assert.AreEqual(DateTimeBehavior.DateOnly, column.DateTimeBehavior);
        }

        [Test]
        public void DeserializeDatBehav_LegacyObjectShape()
        {
            // 2.* .table files sometimes carry the raw Dataverse SDK DateTimeBehavior object
            // instead of the plain enum: { "DatBehav": { "Value": "UserLocal" } }.
            var column = JsonConvert.DeserializeObject<Column>("{ \"DatBehav\": { \"Value\": \"UserLocal\" } }");

            Assert.AreEqual(DateTimeBehavior.UserLocal, column.DateTimeBehavior);
        }

        [Test]
        public void SerializeDatBehav_AlwaysWritesPlainInteger()
        {
            var column = new Column { DateTimeBehavior = DateTimeBehavior.TimeZoneIndependent };

            var json = JsonConvert.SerializeObject(column);

            Assert.IsTrue(json.Replace(" ", string.Empty).Contains("\"DatBehav\":2"));
        }

        [Test]
        public void ObjectInitialization()
        {
            var column = new Column();

            Assert.IsFalse(column.Selected);
            column.Selected = true;
            Assert.IsTrue(column.Selected);

            Assert.IsFalse(column.IsMultiSelect);
            column.IsMultiSelect = true;
            Assert.IsTrue(column.IsMultiSelect);

            Assert.IsNull(column.Name);
            column.Name = "Name";
            Assert.AreEqual("Name", column.Name);

            Assert.IsNull(column.LogicalName);
            column.LogicalName = "LogicalName";
            Assert.AreEqual("LogicalName", column.LogicalName);

            Assert.IsNull(column.EnumName);
            column.EnumName = "EnumName";
            Assert.AreEqual("EnumName", column.EnumName);

            Assert.IsNull(column.MaxRange);
            column.MaxRange = 123;
            Assert.AreEqual(123, column.MaxRange);

            Assert.IsNull(column.MinRange);
            column.MinRange = 123;
            Assert.AreEqual(123, column.MinRange);

            Assert.IsNull(column.StringLength);
            column.StringLength = 123;
            Assert.AreEqual(123, column.StringLength);

            Assert.AreEqual(AttributeCapabilities.None, column.Capabilities);
            column.Capabilities = AttributeCapabilities.AdvancedFind;
            Assert.AreEqual(AttributeCapabilities.AdvancedFind, column.Capabilities);

            Assert.AreEqual(PrimaryType.None, column.PrimaryType);
            column.PrimaryType = PrimaryType.Id;
            Assert.AreEqual(PrimaryType.Id, column.PrimaryType);

            Assert.AreEqual(AttributeTypeCode.Boolean, column.Type);
            column.Type = AttributeTypeCode.Double;
            Assert.AreEqual(AttributeTypeCode.Double, column.Type);
            
            column.DateTimeBehavior = DateTimeBehavior.DateOnly;
            Assert.AreEqual(DateTimeBehavior.DateOnly, column.DateTimeBehavior);

            Assert.IsNotNull(column.Labels);
            Assert.AreEqual(0, column.Labels.Count);
        }

        // ═════════════════════════════════════════════════════════════════════
        //  Multi-select choices
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// The three shapes a <c>.table</c> carries a multi-select choice in, and the one type the
        /// generated code has to declare for all of them: anything but
        /// <c>MultiSelectPicklist</c> sends the runtime mappers to the readers that expect a single
        /// <c>OptionSetValue</c>, and reading an <c>OptionSetValueCollection</c> through those
        /// throws.
        /// </summary>
        [TestCase("Picklist", true, TestName = "EffectiveType_PicklistFlaggedMultiSelect")]
        [TestCase("Virtual", true, TestName = "EffectiveType_VirtualFlaggedMultiSelect")]
        [TestCase("MultiSelectPicklist", false, TestName = "EffectiveType_TypedMultiSelectPicklist")]
        public void EffectiveType_OfAMultiSelectColumn_IsMultiSelectPicklist(string type, bool flagged)
        {
            var json = flagged
                ? $"{{ \"Type\": \"{type}\", \"IsMultiSelect\": true }}"
                : $"{{ \"Type\": \"{type}\" }}";

            var column = JsonConvert.DeserializeObject<Column>(json);

            Assert.IsTrue(column.IsMultiSelectChoice);
            Assert.AreEqual(AttributeTypeCode.MultiSelectPicklist, column.EffectiveType);
        }

        /// <summary>A choice holding one option at a time is left alone.</summary>
        [Test]
        public void EffectiveType_OfASinglePicklist_IsThePicklistItself()
        {
            var column = JsonConvert.DeserializeObject<Column>("{ \"Type\": \"Picklist\" }");

            Assert.IsFalse(column.IsMultiSelectChoice);
            Assert.AreEqual(AttributeTypeCode.Picklist, column.EffectiveType);
        }

        /// <summary>
        /// Neither derived property may be written back out. A <c>.table</c> is round-tripped
        /// verbatim — <c>TsGenerator</c> serializes a column straight into the TypeScript
        /// definitions, and <c>TableFileStore</c> rewrites the files in place — so a property that
        /// serialized would churn every generated file and every checked-in table at once.
        /// </summary>
        [Test]
        public void TheDerivedMultiSelectProperties_AreNotSerialized()
        {
            var json = JsonConvert.SerializeObject(new Column
            {
                LogicalName = "xrm_tags",
                Type = AttributeTypeCode.Picklist,
                IsMultiSelect = true
            });

            Assert.IsFalse(json.Contains(nameof(Column.IsMultiSelectChoice)));
            Assert.IsFalse(json.Contains(nameof(Column.EffectiveType)));
            Assert.IsTrue(json.Replace(" ", string.Empty).Contains("\"IsMultiSelect\":true"));
        }
    }
}
