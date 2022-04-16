using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk.Metadata;

namespace XrmFramework.Core.Tests
{
    [TestClass]
    public class ColumnTests
    {
        [TestMethod]
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

            Assert.IsNull(column.DateTimeBehavior);
            column.DateTimeBehavior = DateTimeBehavior.DateOnly;
            Assert.AreEqual(DateTimeBehavior.DateOnly, column.DateTimeBehavior);

            Assert.IsNotNull(column.Labels);
            Assert.AreEqual(0, column.Labels.Count);
        }
    }
}