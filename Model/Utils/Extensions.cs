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
        {
            return dec == null ? null : new Money(dec.Value);
        }
    }
}
