using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugins.Tests
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ServiceMethodNameAttribute : Attribute
    {
        public ServiceMethodNameAttribute(string methodName)
        {
            MethodName = methodName;
        }

        public string MethodName
        {
            get;
            private set;
        }
    }
}
