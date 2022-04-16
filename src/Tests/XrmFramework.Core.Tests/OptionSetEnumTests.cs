using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XrmFramework.Core.Tests
{
    [TestClass]
    public class OptionSetEnumTests
    {
        [TestMethod]
        public void ObjectInitialization()
        {
            var obj = new OptionSetEnum();

            Assert.IsNull(obj.Name);
            obj.Name = "Name";
            Assert.AreEqual("Name", obj.Name);

            Assert.IsNull(obj.LogicalName);
            obj.LogicalName = "LogicalName";
            Assert.AreEqual("LogicalName", obj.LogicalName);

            Assert.IsFalse(obj.IsGlobal);
            obj.IsGlobal = true;
            Assert.IsTrue(obj.IsGlobal);

            Assert.IsFalse(obj.HasNullValue);
            obj.HasNullValue = true;
            Assert.IsTrue(obj.HasNullValue);

            Assert.IsNotNull(obj.Values);
            Assert.AreEqual(0, obj.Values.Count);
        }
    }
}