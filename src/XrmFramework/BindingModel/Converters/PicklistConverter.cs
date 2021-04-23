// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Xml.Linq;

namespace XrmFramework.BindingModel
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
