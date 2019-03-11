using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [AttributeUsage(AttributeTargets.Field)]
    public class OptionSetAttribute : Attribute
    {
        public Type EnumType { get; private set; }

        public OptionSetAttribute(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Type should be an enum", "enumType");
            }
            EnumType = enumType;
        }
    }
}
