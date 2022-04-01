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
            { AttributeTypeCode.Decimal, "float"},
            { AttributeTypeCode.DateTime, "DateTime"},
            { AttributeTypeCode.Double, "double"},
            { AttributeTypeCode.Integer, "int"},
        };
    }
}
