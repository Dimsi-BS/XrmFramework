// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
namespace Model
{
    interface IBindingModelAction<T>
     where T : Model.IBindingModel
    {
        string PostConvertion(T model);
    }
}
