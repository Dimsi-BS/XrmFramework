using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;
using XrmFramework.BindingModel;

namespace XrmFramework.Tests.BindingModel.Converters
{
    [TestClass]
    public class LookupConverterTests
    {
        [TestMethod]
        public void CanConvertFromString()
        {
            var converter = new LookupConverter();
            var result = converter.CanConvertFrom(null, typeof(string));
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanConvertFromEntityReference()
        {
            var converter = new LookupConverter();
            var result = converter.CanConvertFrom(null, typeof(EntityReference));
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanConvertToString()
        {
            var converter = new LookupConverter();
            var result = converter.CanConvertTo(null, typeof(string));
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanConvertToEntityReference()
        {
            var converter = new LookupConverter();
            var result = converter.CanConvertTo(null, typeof(EntityReference));
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ConvertFromString()
        {
            var converter = new LookupConverter();
            var result = converter.ConvertFrom(null, null, "TestLogicalName|CF998748-0E2E-4A3C-A254-0BB1AD850466|TestName") as EntityReference;

            Assert.IsNotNull(result);
            Assert.AreEqual("TestLogicalName", result.LogicalName);
            Assert.AreEqual(new Guid("CF998748-0E2E-4A3C-A254-0BB1AD850466"), result.Id);
            Assert.AreEqual("TestName", result.Name);
        }

        [TestMethod]
        public void ConvertFromEmptyString()
        {
            var converter = new LookupConverter();
            var result = converter.ConvertFrom(null, null, "") as EntityReference;

            Assert.IsNull(result);
        }

        [TestMethod]
        public void ConvertFromEntityReference()
        {
            var converter = new LookupConverter();
            var entityReference = new EntityReference("TestLogicalName", new Guid("CF998748-0E2E-4A3C-A254-0BB1AD850466"))
            {
                Name = "TestName"
            };

            var result = converter.ConvertFrom(entityReference) as string;

            Assert.AreEqual("TestLogicalName|cf998748-0e2e-4a3c-a254-0bb1ad850466|TestName", result);
        }

        [TestMethod]
        public void ConvertFromUnsupportedType()
        {
            var converter = new LookupConverter();
            var intValue = 0;

            Assert.ThrowsException<ArgumentException>(() =>
            {
                _ = converter.ConvertFrom(intValue) as string;
            });
        }
    }
}
