using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrmFramework.Core
{
    

    public static class AttributeTypeTranslator
    {
        public static readonly Dictionary<AttributeTypeCode, string> dictionnary = new Dictionary<AttributeTypeCode, string>
        {
            { AttributeTypeCode.Boolean, "bool"},
            { AttributeTypeCode.Decimal, "decimal"},
            { AttributeTypeCode.DateTime, "DateTime"},
            { AttributeTypeCode.Double, "double"},
            { AttributeTypeCode.Integer, "int"},
            { AttributeTypeCode.String, "string"},
            { AttributeTypeCode.EntityName, "string"},
            {AttributeTypeCode.BigInt,"Long" },
            {AttributeTypeCode.Memo,"string" },

            //{AttributeTypeCode.Lookup,"" },
            //{AttributeTypeCode.ManagedProperty,"" },
            //{AttributeTypeCode.Customer,"" },
            //{AttributeTypeCode.PartyList,"" },

        };

        public static string Translate(AttributeTypeCode type)
        {
            if (dictionnary.ContainsKey(type))
            {
                return dictionnary[type];

            }
            else
            {
                return null;
            }
        }
    }
}
