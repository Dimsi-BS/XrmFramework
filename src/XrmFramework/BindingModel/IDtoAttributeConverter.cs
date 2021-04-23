// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace XrmFramework.BindingModel
{
    public interface IDtoAttributeConverter
    {
        object ConvertFromDtoAttribute(object value);

        object ConvertToDtoAttribute(object value);
    }
}
