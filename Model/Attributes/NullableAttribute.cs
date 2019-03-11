using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugins
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class NullableAttribute : Attribute
    {
    }
}
