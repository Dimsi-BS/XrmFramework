// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace XrmFramework.BindingModel
{
    public interface IXmlTransform<in T> : IXmlTransform where T : IXmlModel
    {
        void PostXmlConvertion(T model);
        void PreXmlConvertion(T model);
    }

    public interface IXmlTransform
    {
        void PostXmlConvertion(Type type, object model);
        void PreXmlConvertion(Type type, object model);
    }
}
