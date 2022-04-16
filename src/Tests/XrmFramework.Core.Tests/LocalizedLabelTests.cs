using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XrmFramework.Core.Tests
{
    [TestClass]
    public class LocalizedLabelTests
    {
        [TestMethod]
        public void ObjectInitialization()
        {
            var obj = new LocalizedLabel();

            Assert.IsNull(obj.Label);
            obj.Label = "Label";
            Assert.AreEqual("Label", obj.Label);

            Assert.AreEqual(0, obj.LangId);
            obj.LangId = 543;
            Assert.AreEqual(543, obj.LangId);
        }
    }
}