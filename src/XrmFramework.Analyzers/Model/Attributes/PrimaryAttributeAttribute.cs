using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [AttributeUsage(AttributeTargets.Field)]
    public class PrimaryAttributeAttribute : Attribute
    {
        public PrimaryAttributeAttribute(PrimaryAttributeType type)
        {
            Type = type;
        }

        public PrimaryAttributeType Type { get; private set; }
    }

    public enum PrimaryAttributeType
    {
        Id,
        Name,
        Image
    }
}
