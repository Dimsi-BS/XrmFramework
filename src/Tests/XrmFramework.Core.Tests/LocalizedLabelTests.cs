using NUnit.Framework;

namespace XrmFramework.Core.Tests
{
    [TestFixture]
    public class LocalizedLabelTests
    {
        [Test]
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