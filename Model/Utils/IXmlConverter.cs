using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model
{
    public interface IXmlConverter
    {
        object ConvertFromXElement(XElement element);

        void FillXElement(XElement element, object value);
    }
}
