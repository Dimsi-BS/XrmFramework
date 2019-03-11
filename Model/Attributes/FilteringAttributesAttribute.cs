using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugins
{
    [AttributeUsage(AttributeTargets.Method)]
    public class FilteringAttributesAttribute : Attribute
    {
        public string[] Attributes { get; private set; }

        public FilteringAttributesAttribute(params string[] attributes)
        {
            Attributes = attributes;
        }
    }
}
