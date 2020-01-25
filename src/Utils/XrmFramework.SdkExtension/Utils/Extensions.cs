// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugins
{
    public static class Extensions
    {
        public static Entity ToEntity(this EntityReference objectRef)
        {
            if (objectRef == null)
            {
                return null;
            }

            var entity = new Entity(objectRef.LogicalName);
            entity.Id = objectRef.Id;

            if (objectRef.KeyAttributes != null)
            {
                foreach (var key in objectRef.KeyAttributes.Keys)
                {
                    entity.KeyAttributes[key] = objectRef.KeyAttributes[key];
                }
            }

            return entity;
        }

        public static Money ToMoney(this decimal? dec)
            => dec == null ? null : new Money(dec.Value);

        public static Money ToMoney(this decimal dec)
            => ToMoney((decimal?)dec);
    }
}
