// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using Microsoft.Xrm.Sdk;

namespace Model
{
    public interface IEntityModel
    {
        Entity Entity { get; set; }
    }
}
