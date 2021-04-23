// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace XrmFramework.BindingModel
{
    interface IBindingModelAction<T>
     where T : IBindingModel
    {
        string PostConvertion(T model);
    }
}
