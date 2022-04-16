using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XrmFramework.Core.Tests
{
    [TestClass]
    public class OptionSetEnumValueTests
    {
        [TestMethod]
        public void ObjectInitialization()
        {
            var obj = new OptionSetEnumValue();

            Assert.IsNull(obj.ExternalValue);
            obj.ExternalValue = "ExternalValue";
            Assert.AreEqual("ExternalValue", obj.ExternalValue);

            Assert.IsNull(obj.Name);
            obj.Name = "Name";
            Assert.AreEqual("Name", obj.Name);

            Assert.AreEqual(0, obj.Value);
            obj.Value = 543;
            Assert.AreEqual(543, obj.Value);

            Assert.IsNotNull(obj.Labels);
            Assert.AreEqual(0, obj.Labels.Count);
        }
    }
}