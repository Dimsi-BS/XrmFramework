using NUnit.Framework;

namespace XrmFramework.Core.Tests
{
    [TestFixture]
    public class KeyTests
    {
        [Test]
        public void ObjectInitialization()
        {
            var key = new Key();

            Assert.IsNull(key.Name);
            key.Name = "Name";
            Assert.AreEqual("Name", key.Name);

            Assert.IsNotNull(key.FieldNames);
            Assert.AreEqual(0, key.FieldNames.Count);
        }
    }
}