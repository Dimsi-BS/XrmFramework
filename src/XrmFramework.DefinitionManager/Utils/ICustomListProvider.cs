// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace XrmFramework.DefinitionManager
{
    interface ICustomListProvider
    {
        T GetCustomList<T>();

        object GetCustomList(Type type);
    }
}
