using Microsoft.Xrm.Sdk;
using System;
using NUnit.Framework;
using XrmFramework.BindingModel;

namespace XrmFramework.Tests.BindingModel.Converters
{
    [TestFixture]
    public class LookupConverterTests
    {
        [Test]
        public void ConvertFromString()
        {
            var converter = new LookupConverter();
            var result = converter.ConvertFrom("TestLogicalName|CF998748-0E2E-4A3C-A254-0BB1AD850466|TestName") as EntityReference;

            Assert.IsNotNull(result);
            Assert.AreEqual("TestLogicalName", result.LogicalName);
            Assert.AreEqual(new Guid("CF998748-0E2E-4A3C-A254-0BB1AD850466"), result.Id);
            Assert.AreEqual("TestName", result.Name);
        }

        [Test]
        public void ConvertFromEmptyString()
        {
            var converter = new LookupConverter();
            var result = converter.ConvertFrom("") as EntityReference;

            Assert.IsNull(result);
        }

        [Test]
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

        [Test]
        public void ConvertFromUnsupportedType()
        {
            var converter = new LookupConverter();
            var intValue = 0;

            Assert.Throws<ArgumentException>(() =>
            {
                _ = converter.ConvertFrom(intValue) as string;
            });
        }
    }
}
