// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Xml.Linq;

namespace XrmFramework.BindingModel
{
    public interface IXmlConverter
    {
        object ConvertFromXElement(XElement element);

        void FillXElement(XElement element, object value);
    }
}
