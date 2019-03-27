// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;

namespace Model
{
    public interface IBindingModel : IXmlModel
    {
        /// <summary>
        /// Id of the record
        /// </summary>
        Guid Id { get; set; }
    }
}