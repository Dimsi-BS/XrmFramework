using System.Xml.Linq;
using NUnit.Framework;
using XrmFramework.BindingModel;

namespace XrmFramework.Tests.BindingModel.Converters;

[TestFixture]
public class PicklistConverterTests
{
    [Test]
    public void ConvertFromXElement_NonNullElementWithId_ReturnsIntegerValue()
    {
        // Arrange
        var converter = new PicklistConverter();
        var element = XElement.Parse("<Data><Id>42</Id></Data>");

        // Act
        var result = converter.ConvertFromXElement(element);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOf(typeof(int?), result);
        Assert.AreEqual(42, (int?)result);
    }

    [Test]
    public void ConvertFromXElement_NonNullElementWithoutId_ReturnsNull()
    {
        // Arrange
        var converter = new PicklistConverter();
        var element = XElement.Parse("<Data><Id></Id></Data>");

        // Act
        var result = converter.ConvertFromXElement(element);

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public void FillXElement_NullValue_SetsEmptyStringInElement()
    {
        // Arrange
        var converter = new PicklistConverter();
        var element = XElement.Parse("<Data></Data>");

        // Act
        converter.FillXElement(element, null);

        // Assert
        Assert.AreEqual("<Data><Id></Id></Data>", element.ToString(SaveOptions.DisableFormatting));
    }

    [Test]
    public void FillXElement_NonNullValue_SetsIntegerValueInElement()
    {
        // Arrange
        var converter = new PicklistConverter();
        var element = new XElement("Id");

        // Act
        converter.FillXElement(element, 42);

        // Assert
        Assert.AreEqual("42", element.Value);
    }
}
