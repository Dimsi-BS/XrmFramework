using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugins
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ExecutionOrderAttribute : Attribute
    {
        public int Order { get; private set; }

        public ExecutionOrderAttribute(int order)
        {
            Order = order;
        }
    }
}
