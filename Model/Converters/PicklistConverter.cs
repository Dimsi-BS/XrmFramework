using System.Xml.Linq;

namespace Model
{
    public class PicklistConverter : IXmlConverter
    {
        public object ConvertFromXElement(XElement element)
        {
            var el = element.Element("Id");
            return string.IsNullOrEmpty(el.Value) ? null : (int?) int.Parse(el.Value);
        }

        public void FillXElement(XElement element, object value)
        {
            element.Add(new XElement("Id", value?.ToString() ?? string.Empty));
        }
    }
}
