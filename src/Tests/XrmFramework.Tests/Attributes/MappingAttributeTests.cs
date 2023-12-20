using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Security.Cryptography.Xml;
using XrmFramework.BindingModel;

namespace XrmFramework.Tests.Attributes
{
    [TestClass]
    public class MappingAttributeTests
    {
        [TestMethod]
        public void MappingAttribute_ConstructorWithRelativePath_SetsRelativePath()
        {
            // Arrange
            string relativePath = "TestPath";

            // Act
            MappingAttribute attribute = new MappingAttributeTestClass(relativePath);

            // Assert
            Assert.AreEqual(relativePath, attribute.RelativePath);
        }

        [TestMethod]
        public void MappingAttribute_ConstructorWithRelativePathAndAlternatePath_SetsRelativePathAndAlternatePath()
        {
            // Arrange
            string relativePath = "TestPath";
            string alternateRelativePath = "AlternatePath";

            // Act
            MappingAttribute attribute = new MappingAttributeTestClass(relativePath, alternateRelativePath);

            // Assert
            Assert.AreEqual(relativePath, attribute.RelativePath);
            Assert.AreEqual(alternateRelativePath, attribute.AlternateRelativePath);
        }

        [TestMethod]
        public void MappingAttribute_ConstructorWithRelativePathAndConverterType_SetsRelativePathAndConverterType()
        {
            // Arrange
            string relativePath = "TestPath";
            Type converterType = typeof(TestConverter);

            // Act
            MappingAttribute attribute = new MappingAttributeTestClass(relativePath, converterType);

            // Assert
            Assert.AreEqual(relativePath, attribute.RelativePath);
            Assert.AreEqual(converterType, attribute.ConverterType);
        }

        [TestMethod]
        public void DtoObjectMappingAttribute_ConstructorWithRelativePathAndConverterType_SetsRelativePathAndConverterType()
        {
            // Arrange
            string relativePath = "TestPath";
            Type converterType = typeof(TestConverter);

            // Act
            DtoObjectMappingAttribute attribute = new(relativePath, converterType);

            // Assert
            Assert.AreEqual(relativePath, attribute.RelativePath);
            Assert.AreEqual(converterType, attribute.ConverterType);
        }

        [TestMethod]
        public void MappingAttribute_ConstructorWithRelativePathAlternatePathAndConverterType_SetsPathsAndConverterType()
        {
            // Arrange
            string relativePath = "TestPath";
            string alternateRelativePath = "AlternatePath";
            Type converterType = typeof(TestConverter);

            // Act
            MappingAttribute attribute = new MappingAttributeTestClass(relativePath, alternateRelativePath, converterType);

            // Assert
            Assert.AreEqual(relativePath, attribute.RelativePath);
            Assert.AreEqual(alternateRelativePath, attribute.AlternateRelativePath);
            Assert.AreEqual(converterType, attribute.ConverterType);
        }

        [TestMethod]
        public void MappingAttribute_ValidForExportProperty_CanBeSetAndGet()
        {
            // Arrange
            MappingAttribute attribute = new MappingAttributeTestClass("TestPath");

            // Act
            attribute.ValidForExport = false;

            // Assert
            Assert.IsFalse(attribute.ValidForExport);
        }

        [TestMethod]
        public void XmlMappingAttribute_IsAttributeProperty_CanBeSetAndGet()
        {
            // Arrange
            XmlMappingAttribute attribute = new XmlMappingAttribute("TestPath");

            // Act
            attribute.IsAttribute = true;

            // Assert
            Assert.IsTrue(attribute.IsAttribute);
        }

        [TestMethod]
        public void DtoFieldMappingAttribute_ConstructorWithDtoFieldName_SetsDtoFieldName()
        {
            // Arrange
            string dtoFieldName = "TestDtoField";

            // Act
            DtoFieldMappingAttribute attribute = new DtoFieldMappingAttribute(dtoFieldName);

            // Assert
            Assert.AreEqual(dtoFieldName, attribute.RelativePath);
        }

        [TestMethod]
        public void DtoObjectMappingAttribute_ConstructorWithDtoClassName_SetsDtoClassName()
        {
            // Arrange
            string dtoClassName = "TestDtoClass";

            // Act
            DtoObjectMappingAttribute attribute = new DtoObjectMappingAttribute(dtoClassName);

            // Assert
            Assert.AreEqual(dtoClassName, attribute.RelativePath);
        }
    }

    // Helper class for testing with a custom converter type
    public class TestConverter { }

    // Helper class to expose protected members for testing
    public class MappingAttributeTestClass : MappingAttribute
    {
        public MappingAttributeTestClass(string relativePath) : base(relativePath) { }

        public MappingAttributeTestClass(string relativePath, string alternateRelativePath) : base(relativePath, alternateRelativePath) { }

        public MappingAttributeTestClass(string relativePath, Type converterType) : base(relativePath, converterType) { }

        public MappingAttributeTestClass(string relativePath, string alternateRelativePath, Type converterType) : base(relativePath, alternateRelativePath, converterType) { }
    }
}
