// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace XrmFramework.BindingModel
{
    [AttributeUsage(AttributeTargets.Class)]
    public class XmlTransformAttribute : Attribute
    {
        public XmlTransformAttribute(Type actionType)
        {
            ActionType = actionType;
        }

        public Type ActionType { get; private set; }
    }
}
