using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using XrmFramework.BindingModel;

namespace XrmFramework.Tests.Attributes;

[TestClass]
public class CrmMappingAttributeTests
{
    [TestMethod]
    public void CrmMappingAttribute_ConstructorWithAttributeName_SetsAttributeName()
    {
        // Arrange
        string attributeName = "TestAttribute";

        // Act
        CrmMappingAttribute attribute = new CrmMappingAttribute(attributeName);

        // Assert
        Assert.AreEqual(attributeName, attribute.AttributeName);
    }

    [TestMethod]
    public void CrmMappingAttribute_ConstructorWithAttributeNameAndType_SetsAttributeNameAndType()
    {
        // Arrange
        string attributeName = "TestAttribute";
        AttributeTypeCode attributeType = AttributeTypeCode.String;

        // Act
        CrmMappingAttribute attribute = new CrmMappingAttribute(attributeName, attributeType);

        // Assert
        Assert.AreEqual(attributeName, attribute.AttributeName);
        Assert.AreEqual(attributeType, attribute.AttributeType);
    }

    [TestMethod]
    public void CrmMappingAttribute_FollowLinkProperty_CanBeSetAndGet()
    {
        // Arrange
        CrmMappingAttribute attribute = new CrmMappingAttribute("TestAttribute");

        Assert.IsFalse(attribute.FollowLink);

        // Act
        attribute.FollowLink = true;

        // Assert
        Assert.IsTrue(attribute.FollowLink);
    }

    [TestMethod]
    public void CrmMappingAttribute_IsValidForUpdateProperty_CanBeSetAndGet()
    {
        // Arrange
        CrmMappingAttribute attribute = new CrmMappingAttribute("TestAttribute");

        // Act
        attribute.IsValidForUpdate = false;

        // Assert
        Assert.IsFalse(attribute.IsValidForUpdate);
    }

    [TestMethod]
    public void CrmMappingAttribute_LookupInfoProperty_CanBeSetAndGet()
    {
        // Arrange
        CrmMappingAttribute attribute = new CrmMappingAttribute("TestAttribute");
        LookupAttributeInfo lookupInfo = LookupAttributeInfo.Id;

        // Act
        attribute.LookupInfo = lookupInfo;

        // Assert
        Assert.AreEqual(lookupInfo, attribute.LookupInfo);
    }

    [TestMethod]
    public void CrmMappingAttribute_DiffStringComparisonBehaviorProperty_DefaultsToInvariantCulture()
    {
        // Arrange
        CrmMappingAttribute attribute = new CrmMappingAttribute("TestAttribute");

        // Assert
        Assert.AreEqual(StringComparison.InvariantCulture, attribute.DiffStringComparisonBehavior);
    }
}
